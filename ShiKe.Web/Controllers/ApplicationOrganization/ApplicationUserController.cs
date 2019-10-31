using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ShiKe.Entities.ApplicationOrganization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using ShiKe.ViewModels.ApplicationOrganization;
using ShiKe.DataAccess.SqlServer.Utilities;
using Microsoft.AspNetCore.Authorization;
using ShiKe.Common.ViewModelComponents;
using ShiKe.DataAccess.Common;
using ShiKe.Common.JsonModels;
using System.Linq.Expressions;

namespace ShiKe.Web.Controllers.ApplicationOrganization
{
    /// <summary>
    /// 用户管理（也可以当成初始化用户和角色组的应用）
    /// </summary>
    public class ApplicationUserController : Controller
    {
        private readonly RoleManager<ApplicationRole> _RoleManager;
        private readonly UserManager<ApplicationUser> _UserManager;

        public ApplicationUserController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _RoleManager = roleManager;
            _UserManager = userManager;
        }

        #region 用户列表部分
        /// <summary>
        /// 用户管理入口
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var pageSize = 18; // 这参数未来要根据前端用户的显示器情况处理一下
            var pageIndex = 1;

            var boCollection = _RoleManager.Roles;
            var boVMCollection = new List<ApplicationRoleVM>();
            var counter = 0;
            foreach (var item in boCollection.OrderBy(x => x.SortCode))
            {
                var boVM = new ApplicationRoleVM(item)
                {
                    OrderNumber = (++counter).ToString()
                };
                boVMCollection.Add(boVM);
            }
            // 提取第一个角色组的用户作为待处理的用户对象
            var role = boCollection.OrderBy(x => x.SortCode).FirstOrDefault();
            var userCollection = from u in _UserManager.Users
                                 where u.Roles.Select(x => x.RoleId).Contains(role.Id)
                                 select u;

            // 提取分页的数据，当前这是第一页，而且没有任何限制条件的
            var userCollectionPageList = userCollection.ToPaginatedList(pageIndex,pageSize);
            var userVMCollection = new List<ApplicationUserVM>();
            counter = 0;
            foreach (var user in userCollectionPageList)
            {
                var userVM = new ApplicationUserVM(user)
                {
                    OrderNumber = (++counter).ToString()
                };
                userVMCollection.Add(userVM);
            }

            ViewBag.UserVMCollection = userVMCollection;   

            ViewBag.RoleName = role.DisplayName;
            ViewBag.RoleID = role.Id;

            ViewBag.WhatItIs = "Users";   // 用于为 _RoleAndUserLayout 判断调用那个 PartialView 使用的，相关的引用实例在：_RoleAndUserLayout.cshtml 大约 84-94 行
            ViewBag.RolesItems = boVMCollection;
            // 提取当前页面关联的分页器实例
            var pageGroup = PagenateGroupRepository.GetItem<ApplicationUser>(userCollectionPageList,10, pageIndex);
            ViewBag.PageGroup = pageGroup;

            var listPageParameter = new ListPageParameter()
            {
                PageIndex        = userCollectionPageList.PageIndex,
                Keyword          = "",
                PageSize         = userCollectionPageList.PageSize,
                ObjectTypeID     = role.Id,
                ObjectAmount     = userCollectionPageList.TotalCount,
                SortDesc         = "Default",
                SortProperty     = "UserName",
                PageAmount       = 0,
                SelectedObjectID = ""
            };
            ViewBag.PageParameter = listPageParameter;


            return View("../../Views/ApplicationOrganization/ApplicationUser/Index", boVMCollection);
        }

        /// <summary>
        /// 用户列表数据
        /// </summary>
        /// <param name="listPageParaJson">
        /// 用于简单定义从前端页面返回的数据列表相关的 Json 变量，变量的定义依赖 ShiKe.Common.JsonModels.ListPageParameter,
        /// 前端 json 数据构建相关的代码，参见：wwwroot/js/shikeCommon.js 其中的方法：function shikeGetListParaJson()
        /// </param>
        /// <returns></returns>
        public async Task<IActionResult> List(string listPageParaJson)
        {
            var listPagePara = Newtonsoft.Json.JsonConvert.DeserializeObject<ListPageParameter>(listPageParaJson);

            var typeID = "";
            var keyword = "";
            if (!String.IsNullOrEmpty(listPagePara.ObjectTypeID))
                typeID = listPagePara.ObjectTypeID;
            if (!String.IsNullOrEmpty(listPagePara.Keyword))
                keyword = listPagePara.Keyword;

            #region 1.构建与 keyword 相关的查询 lambda 表达式，用于对查询结果的过滤（给 Where 使用）
            Expression<Func<ApplicationUser, bool>> predicate = x =>
                x.UserName.Contains(keyword) ||
                x.ChineseFullName.Contains(keyword) ||
                x.FirstName.Contains(keyword) ||
                x.LastName.Contains(keyword) ||
                x.MobileNumber.Contains(keyword);
            #endregion

            #region 2.根据情况提取系统用户数据集和
            var id = listPagePara.ObjectTypeID;
            var roleID = (String.IsNullOrEmpty(id)) ? "" : id;
            IQueryable<ApplicationUser> userCollection;
            if (String.IsNullOrEmpty(roleID))
            {
                // 提取全部没有归属用户组的用户
                userCollection = from u in _UserManager.Users
                                 where u.Roles.Count == 0
                                 select u;
            }
            else
            {
                // 提取归属指定用户组的用户
                userCollection = from u in _UserManager.Users
                                 where u.Roles.Select(x => x.RoleId).Contains(roleID)
                                 select u;
            }

            // 处理条件过滤
            var filterUserCollection = userCollection.Where(predicate); 
            #endregion

            #region 3.根据属性名称确定排序的属性的 lambda 表达式
            var sortPropertyName = listPagePara.SortProperty;
            var type             = typeof(ApplicationUser);
            var target           = Expression.Parameter(typeof(object));
            var castTarget       = Expression.Convert(target, type);
            var getPropertyValue = Expression.Property(castTarget, sortPropertyName);
            var sortExpession    = Expression.Lambda<Func<ApplicationUser, object>>(getPropertyValue, target);
            #endregion

            #region 4.对过滤的用户数据进行排序
            // 处理排序
            IQueryable<ApplicationUser> sortedUserCollection;
            if (listPagePara.SortDesc == "")
            {
                sortedUserCollection = filterUserCollection.OrderByDescending(sortExpession);
            }
            else
            {
                sortedUserCollection = filterUserCollection.OrderBy(sortExpession);
            }
            // 处理分页
            var userCollectionPageList = sortedUserCollection.ToPaginatedList(listPagePara.PageIndex, listPagePara.PageSize);
            #endregion

            #region 5.构建相关的视图模型和所需要向前端提交一些与约束相关的参数
            var userVMCollection = new List<ApplicationUserVM>();
            var counter = 0;
            foreach (var user in userCollectionPageList)
            {
                var userVM = new ApplicationUserVM(user)
                {
                    OrderNumber = (++counter + ((listPagePara.PageIndex - 1) * listPagePara.PageSize)).ToString()
                };
                userVMCollection.Add(userVM);
            }

            ViewBag.RoleID = id;
            if (!String.IsNullOrEmpty(roleID))
            {
                var role = await _RoleManager.FindByIdAsync(roleID);
                ViewBag.RoleName = role.DisplayName;
            }
            else
            {
                ViewBag.RoleName = "没有归属任何角色组";
            }
            // 提取当前页面关联的分页器实例
            var pageGroup = PagenateGroupRepository.GetItem<ApplicationUser>(userCollectionPageList, 10, listPagePara.PageIndex);
            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara; 
            #endregion

            return PartialView("../../Views/ApplicationOrganization/ApplicationUser/_List", userVMCollection);

        }
        #endregion

        #region 新建用户数据处理部分
        /// <summary>
        /// 新建用户数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            bool isNew = true;
            var user = new ApplicationUser();
            var boVM = new ApplicationUserVM(user)
            {
                IsNew = isNew
            };

            #region 处理用户归属的用户组的数据
            #region 1.待选择的用户组数据
            var role = _RoleManager.Roles;
            var roleItems = new List<PlainFacadeItem>();
            foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
            {
                var rItem = new PlainFacadeItem() { ID = item.Id, Name = item.Name, DisplayName = item.DisplayName, SortCode = item.SortCode };
                roleItems.Add(rItem);
            }
            boVM.RoleItemColection = roleItems;
            #endregion
            #region 2.已经归属的用户组部分
            boVM.RoleItemIDCollection = new List<string>();

            //foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
            //{
            //    var h = await _UserManager.IsInRoleAsync(user, item.Name);
            //    if (h)
            //    {
            //        boVM.RoleItemIDCollection.Add(item.Id);
            //    }
            //}
            #endregion
            ViewBag.RoleID = null;
            #endregion

            return PartialView("../../Views/ApplicationOrganization/ApplicationUser/_CreateApplicationUser", boVM);
        }

        /// <summary>
        /// 保存新建用户数据处理部分
        /// </summary>
        /// <param name="boVM">用户视图模型的实例</param>
        /// <returns></returns>
        public async Task<IActionResult> SaveCreateApplicationUser([Bind("ID,IsNew,RoleItemIDCollection,UserName,Name,MobileNumber,EMail,Password,ConfirmPassword,Description")]ApplicationUserVM boVM)
        {
            var validateMessage = new ValidateMessage();

            if (ModelState.IsValid)
            {
                // 检查重名 
                var isNewUser = await _UserManager.FindByNameAsync(boVM.UserName);
                if (_HasTheSameUser(boVM.UserName))
                {
                    validateMessage.IsOK = false;
                    validateMessage.ValidateMessageItems.Add(
                        new ValidateMessageItem()
                        { IsPropertyName=true, MessageName="UserName", Message="用户选择的用户名已经被使用了。" });
                    return Json(validateMessage);
                }

                #region 新增用户的常规处理
                var user = new ApplicationUser(boVM.UserName)
                {
                    FirstName = "",
                    ChineseFullName = boVM.Name,
                    Email = boVM.EMail,
                    MobileNumber = boVM.MobileNumber
                };

                // 校验密码强度
                var passwordValidator = new Microsoft.AspNetCore.Identity.PasswordValidator<ApplicationUser>();
                var result = await passwordValidator.ValidateAsync(_UserManager, user, boVM.Password);
                if (!result.Succeeded)
                {
                    validateMessage.IsOK = false;
                    validateMessage.ValidateMessageItems.Add(
                        new ValidateMessageItem()
                        { IsPropertyName = true, MessageName = "Password", Message = "您输入的密码强度未满足要求：密码长度大于6个字符，且必须包含数字、大写字母、小写字母和非字母字符如 @ 。" });
                    return Json(validateMessage);
                }

                await _UserManager.CreateAsync(user);
                await _UserManager.AddPasswordAsync(user, boVM.Password);  // 添加密码
                if (boVM.RoleItemIDCollection != null)
                {
                    foreach (var item in boVM.RoleItemIDCollection)             // 遍历所选择的用户组的ID集合，加入到相关的角色组
                    {
                        var role = await _RoleManager.FindByIdAsync(item);
                        if (role != null)
                        {
                            await _UserManager.AddToRoleAsync(user, role.Name);  // 将用户添加到相应的用户组内
                        }
                    }
                }
                #endregion

                validateMessage.IsOK = true;
                validateMessage.ValidateMessageItems.Add(
                    new ValidateMessageItem
                    {
                        IsPropertyName = false,
                        MessageName = "Succeed",
                        Message = ""
                    });

                return Json(validateMessage);   // 返回处理结果

            }
            else
            {
                #region 1.待选择的用户组数据
                var user = await _UserManager.FindByIdAsync(boVM.ID.ToString());
                var role = _RoleManager.Roles;
                var roleItems = new List<PlainFacadeItem>();
                foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
                {
                    var rItem = new PlainFacadeItem() { ID = item.Id, Name = item.Name, DisplayName = item.DisplayName, SortCode = item.SortCode };
                    roleItems.Add(rItem);
                }
                boVM.RoleItemColection = roleItems;
                #endregion

                validateMessage.IsOK = false;
                var errCollection = from errKey in ModelState.Keys
                                    from errMessage in ModelState[errKey].Errors
                                    where ModelState[errKey].Errors.Count > 0
                                    select (new { errKey, errMessage.ErrorMessage });

                foreach (var errItem in errCollection)
                {
                    var vmItem = new ValidateMessageItem()
                    {
                        IsPropertyName = true,
                        MessageName = errItem.errKey,
                        Message = errItem.ErrorMessage
                    };
                    validateMessage.ValidateMessageItems.Add(vmItem);
                }
                return Json(validateMessage);
            }
        }
        #endregion

        #region 编辑用户基础数据管理部分
        /// <summary>
        /// 新建或者编辑用户数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(Guid id, Guid roleID)
        {
            var user = await _UserManager.FindByIdAsync(id.ToString());
            var boVM = new ApplicationUserForEditVM(user, roleID);

            #region 处理用户归属的用户组的数据
            #region 1.待选择的用户组数据
            //var role = _RoleManager.Roles;
            var roleItems = new List<PlainFacadeItem>();
            foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
            {
                var rItem = new PlainFacadeItem()
                {
                    ID = item.Id,
                    Name = item.Name,
                    DisplayName = item.DisplayName,
                    SortCode = item.SortCode
                };
                roleItems.Add(rItem);
            }
            boVM.RoleItemColection = roleItems;
            #endregion
            #region 2.已经归属的用户组部分
            boVM.RoleItemIDCollection = (
                from roleItem in _RoleManager.Roles
                where roleItem.Users.Select(x => x.UserId).Contains(user.Id)
                select roleItem.Id
                ).ToList(); ;
            #endregion
            #endregion

            return PartialView("../../Views/ApplicationOrganization/ApplicationUser/_EditApplicationUser", boVM);
        }

        /// <summary>
        /// 保存用户资料
        /// </summary>
        /// <param name="boVM">用户的视图模型</param>
        /// <returns></returns>
        public async Task<IActionResult> SaveEditApplicationUser([Bind("ID,RoleID,IsNew,RoleItemIDCollection,UserName,Name,MobileNumber,EMail,Password,ConfirmPassword,Description")]ApplicationUserForEditVM boVM)
        {
            var validateMessage = new ValidateMessage();
            if (ModelState.IsValid)
            {
                if (boVM.RoleItemIDCollection==null) { boVM.RoleItemIDCollection = new List<string>(); }
                var user = await _UserManager.FindByIdAsync(boVM.ID.ToString());
                #region 1.用户基本资料更新
                user.FirstName = "";
                user.ChineseFullName = boVM.Name;
                user.Email = boVM.EMail;
                user.MobileNumber = boVM.MobileNumber;
                await _UserManager.UpdateAsync(user);
                #endregion

                #region 2.归属角色组的处理
                var roleCollection = _RoleManager.Roles;
                var addRoles = new List<string>();
                var removeRoles = new List<string>();
                foreach (var roleItem in roleCollection)
                {
                    // 当前的用户是否归属角色组：roleItem
                    var h = await _UserManager.IsInRoleAsync(user, roleItem.Name);
                    if (!h)
                    {
                        // 不归属，但前端已经选择了相应的角色组，则将当前用户加入相应的角色组。
                        if (boVM.RoleItemIDCollection.Contains(roleItem.Id))
                        {
                            addRoles.Add(roleItem.Name);
                        }
                    }
                    else
                    {
                        // 归属，但前端并未选择相应的角色组，则将当前用户从相关的组中移除。
                        if (!boVM.RoleItemIDCollection.Contains(roleItem.Id))
                        {
                            removeRoles.Add(roleItem.Name);
                        }
                    }
                }
                await _UserManager.AddToRolesAsync(user, addRoles);
                await _UserManager.RemoveFromRolesAsync(user, removeRoles);

                #endregion
                var resultRoleID = "";
                if (boVM.RoleID.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    resultRoleID = boVM.RoleID.ToString();
                }
                validateMessage.IsOK = true;
                validateMessage.ValidateMessageItems.Add(
                    new ValidateMessageItem
                    {
                        IsPropertyName = false,
                        MessageName = "",
                        Message = resultRoleID
                    });

                return Json(validateMessage);
            }
            else
            {
                #region 处理用户归属的用户组的数据
                #region 1.待选择的用户组数据
                var user = await _UserManager.FindByIdAsync(boVM.ID.ToString());
                var role = _RoleManager.Roles;
                var roleItems = new List<PlainFacadeItem>();
                foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
                {
                    var rItem = new PlainFacadeItem() { ID = item.Id, Name = item.Name, DisplayName = item.DisplayName, SortCode = item.SortCode };
                    roleItems.Add(rItem);
                }
                boVM.RoleItemColection = roleItems;
                #endregion
                #region 2.已经归属的用户组部分
                boVM.RoleItemIDCollection = (
                    from roleItem in _RoleManager.Roles
                    where roleItem.Users.Select(x => x.UserId).Contains(user.Id)
                    select roleItem.Id
                    ).ToList(); ;

                #endregion
                #endregion

                validateMessage.IsOK = false;
                var errCollection = from errKey in ModelState.Keys
                                    from errMessage in ModelState[errKey].Errors
                                    where ModelState[errKey].Errors.Count > 0
                                    select (new { errKey, errMessage.ErrorMessage });
                foreach (var errItem in errCollection)
                {
                    var vmItem = new ValidateMessageItem()
                    {
                        IsPropertyName = true,
                        MessageName = errItem.errKey,
                        Message = errItem.ErrorMessage
                    };
                    validateMessage.ValidateMessageItems.Add(vmItem);
                }
                return Json(validateMessage);
            }
        }
        #endregion

        #region 一些用户数据处理公开的方法
        /// <summary>
        /// 是否存在指定用户名用户的方法
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public IActionResult HasTheSameUser(string userName) => Json(new { hasTheSameUser = _HasTheSameUser(userName) });

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> LockoutUser(Guid id,string roleID)
        {
            var user = await _UserManager.FindByIdAsync(id.ToString());
            await _UserManager.SetLockoutEnabledAsync(user, false);
            await _UserManager.SetLockoutEndDateAsync(user,DateTime.Today.AddYears(10));

            return Json(new { roleID = roleID });
        }

        /// <summary>
        /// 解锁用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public async Task<IActionResult> UnLockoutUser(Guid id, string roleID)
        {
            var user = await _UserManager.FindByIdAsync(id.ToString());
            await _UserManager.SetLockoutEnabledAsync(user, true);
            await _UserManager.SetLockoutEndDateAsync(user, DateTime.Today.AddYears(10));

            return Json(new { roleID = roleID });
        }

        /// <summary>
        /// 重置密码，响应前端的请求，构建一个简单的视图模型实例返回给前端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ResetUserPassword(string id)
        {
            var boVM = new ApplicationUserResetPasswordVM() {ID=id, Password="",ConfirmPassword="" };
            return PartialView("../../Views/ApplicationOrganization/ApplicationUser/_ResetUserPassword", boVM);
        }

        /// <summary>
        /// 处理重置密码的请求，并返回处理结果
        /// </summary>
        /// <param name="boVM"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveResetUserPassword([Bind("ID,Password,ConfirmPassword")]ApplicationUserResetPasswordVM boVM)
        {
            var validateMessage = new ValidateMessage();
            if (ModelState.IsValid)
            {
                var user = await _UserManager.FindByIdAsync(boVM.ID);
                // 获取重置密码的令牌
                string resetToken = await _UserManager.GeneratePasswordResetTokenAsync(user);
                // 重置密码
                await _UserManager.ResetPasswordAsync(user, resetToken, boVM.Password);

                validateMessage.IsOK = true;
                validateMessage.ValidateMessageItems.Add(
                    new ValidateMessageItem
                    {
                        IsPropertyName = false,
                        MessageName = "",
                        Message = "密码重置成功，请关闭对话框。"
                    });

                return Json(validateMessage);
            }
            else
            {
                validateMessage.IsOK = false;
                var errCollection = from errKey in ModelState.Keys
                                    from errMessage in ModelState[errKey].Errors
                                    where ModelState[errKey].Errors.Count > 0
                                    select (new { errKey, errMessage.ErrorMessage });
                foreach (var errItem in errCollection)
                {
                    var vmItem = new ValidateMessageItem()
                    {
                        IsPropertyName = true,
                        MessageName = errItem.errKey,
                        Message = errItem.ErrorMessage
                    };
                    validateMessage.ValidateMessageItems.Add(vmItem);
                }
                return Json(validateMessage);
            }
        }

        #endregion

        #region 一些内部使用的私有方法

        /// <summary>
        /// 是否存在指定用户名的用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        private bool _HasTheSameUser(string userName) => _UserManager.Users.Any(x => x.UserName == userName);

        #endregion

        #region 早期的原始代码
        /// <summary>
        /// 创建简单的用户组（角色组）
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SetupRole()
        {
            #region 创建具有最高权限的角色组：Admin 
            var adminRole = await _RoleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new ApplicationRole("Admin");
                await _RoleManager.CreateAsync(adminRole);

                await _RoleManager.AddClaimAsync(adminRole, new Claim("管理员", "projects.view"));
                await _RoleManager.AddClaimAsync(adminRole, new Claim("管理员", "projects.create"));
                await _RoleManager.AddClaimAsync(adminRole, new Claim("管理员", "projects.update"));
            }
            #endregion

            #region 创建统一的账号管理角色组：Account Manager
            var accountManagerRole = await _RoleManager.FindByNameAsync("Account Manager");
            if (accountManagerRole == null)
            {
                accountManagerRole = new ApplicationRole("Account Manager");
                await _RoleManager.CreateAsync(accountManagerRole);
                await _RoleManager.AddClaimAsync(accountManagerRole, new Claim("账号管理员", "account.manage"));
            }
            #endregion

            #region 创建具有评论发布等权限的普通读者角色组
            var registerReadersRole = await _RoleManager.FindByNameAsync("RegisterReadersRole");
            if (registerReadersRole == null)
            {
                registerReadersRole = new ApplicationRole("RegisterReadersRole");
                await _RoleManager.CreateAsync(registerReadersRole);
                await _RoleManager.AddClaimAsync(registerReadersRole, new Claim("普通注册读者", "Article.Read"));
                await _RoleManager.AddClaimAsync(registerReadersRole, new Claim("普通注册读者", "Article.Comment"));
            }
            #endregion

            #region 创建文章作者（记者）用户组：ArticleWritor
            var articleWritorRole = await _RoleManager.FindByNameAsync("ArticleWritor");
            if (articleWritorRole == null)
            {
                articleWritorRole = new ApplicationRole("ArticleWritor");
                await _RoleManager.CreateAsync(articleWritorRole);
                await _RoleManager.AddClaimAsync(articleWritorRole, new Claim("记者或者作者", "Article.Manage"));
            }
            #endregion

            #region 创建文章审核管理的用户组：ArticleAudit
            var articleAuditRole = await _RoleManager.FindByNameAsync("ArticleAudit");
            if (articleAuditRole == null)
            {
                articleAuditRole = new ApplicationRole("ArticleAudit");
                await _RoleManager.CreateAsync(articleAuditRole);
                await _RoleManager.AddClaimAsync(articleAuditRole, new Claim("记者或者作者", "Article.Manage"));
            }

            #endregion

            return Ok();
        }

        /// <summary>
        /// 创建简单的用户
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SetupUsers()
        {
            #region 一个具有 Admin 和 Account Manager 权限的用户
            var rabbit = await _UserManager.FindByNameAsync("rabbit");
            if (rabbit == null)
            {
                rabbit = new ApplicationUser("rabbit")
                {
                    ChineseFullName = "郑晓琳"
                };
                await _UserManager.CreateAsync(rabbit);
                await _UserManager.AddPasswordAsync(rabbit, "123@Abc");      // 添加密码
                await _UserManager.AddToRoleAsync(rabbit, "Admin");          // 加入到 Admin 角色组
                await _UserManager.AddToRoleAsync(rabbit, "Account Manager");// 加入到 Account Manage 角色组
            }
            #endregion

            #region 一个具有普通读者权限的用户：
            var elephant = await _UserManager.FindByNameAsync("elephant");
            if (elephant == null)
            {
                elephant = new ApplicationUser("elephant")
                {
                    FirstName = "",
                    ChineseFullName = "张伟丽"
                };
                await _UserManager.CreateAsync(elephant);
                await _UserManager.AddPasswordAsync(elephant, "123@Abc");              // 添加密码
                await _UserManager.AddToRoleAsync(elephant, "RegisterReadersRole");    // 加入到 RegisterReadersRole 角色组
            }
            #endregion

            #region 一个具有记者或者作者权限的用户：
            var wolf = await _UserManager.FindByNameAsync("wolf");
            if (wolf == null)
            {
                wolf = new ApplicationUser("wolf")
                {
                    FirstName = "",
                    ChineseFullName = "韦晓林"
                };
                await _UserManager.CreateAsync(wolf);
                await _UserManager.AddPasswordAsync(wolf, "123@Abc");              // 添加密码
                await _UserManager.AddToRoleAsync(wolf, "ArticleWritor");          // 加入到 ArticleWritor 角色组
            }
            #endregion

            #region 一个具有审核权限的用户：
            var fox = await _UserManager.FindByNameAsync("fox");
            if (fox == null)
            {
                fox = new ApplicationUser("fox")
                {
                    FirstName = "",
                    ChineseFullName = "韦艳芳"
                };
                await _UserManager.CreateAsync(fox);
                await _UserManager.AddPasswordAsync(fox, "123@Abc");       // 添加密码
                await _UserManager.AddToRoleAsync(fox, "ArticleAudit");    // 加入到 ArticleAudit 角色组
            }
            #endregion

            return Ok();
        }

        public async Task<string> CreateSomeUsers()
        {
            for (int i = 500; i < 1000; i++)
            {
                var user = new ApplicationUser("Kaiser" + i.ToString())
                {
                    FirstName = "",
                    ChineseFullName = "Kaiser" + i.ToString(),
                    Email = "Kaiser" + i.ToString() + "@outlook.com",
                    MobileNumber = ""
                };
                await _UserManager.CreateAsync(user);
                await _UserManager.AddPasswordAsync(user, "123@Abc");  // 添加密码

            }
            return "创建数据完成";
        }
        #endregion
    }
}
