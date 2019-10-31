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

namespace ShiKe.News.Web.Controllers.ApplicationOrganization
{
    /// <summary>
    /// 用户和用户组管理的简单例子（也可以当成初始化用户和角色组的应用）
    /// </summary>
    public class ApplicationRoleAndUserController : Controller
    {
        private readonly RoleManager<ApplicationRole> _RoleManager;
        private readonly UserManager<ApplicationUser> _UserManager;

        public ApplicationRoleAndUserController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _RoleManager = roleManager;
            _UserManager = userManager;
        }

        #region 角色管理部分
        /// <summary>
        /// 用户组管理入口
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
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

            ViewBag.WhatItIs = "Roles";     // 用于为 _RoleAndUserLayout 判断调用那个 PartialView 使用的
            ViewBag.RolesItems = boVMCollection;

            return View("../../Views/ApplicationOrganization/ApplicationRoleAndUser/Index", boVMCollection);
        }

        /// <summary>
        /// 创建待编辑的角色组数据，返回到前端页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateOrEditForApplicationRole(Guid id)
        {
            var bo = await _RoleManager.FindByIdAsync(id.ToString());
            if (bo == null)
            {
                bo = new ApplicationRole();
            }
            var boVM = new ApplicationRoleVM(bo);
            return PartialView("../../Views/ApplicationOrganization/ApplicationRoleAndUser/_CreateOrEdit", boVM);
        }

        /// <summary>
        /// 保存用户组数据
        /// </summary>
        /// <param name="boVM"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveApplicationRole([Bind("ID,Name,DisplayName,Description,SortCode")]ApplicationRoleVM boVM)
        {
            if (ModelState.IsValid)
            {
                var bo = await _RoleManager.FindByIdAsync(boVM.ID.ToString());
                if (bo == null)
                {
                    bo = new ApplicationRole();
                    boVM.MapToBo(bo);
                    await _RoleManager.CreateAsync(bo);
                }
                else
                {
                    boVM.MapToBo(bo);
                    await _RoleManager.UpdateAsync(bo);
                }
                var saveStatus = new EditAndSaveStatus() { SaveOk = true, StatusMessage = "../../ApplicationRoleAndUser/Index" };
                return Json(saveStatus);
            }
            else
                return PartialView("../../Views/ApplicationOrganization/ApplicationRoleAndUser/_CreateOrEdit", boVM);

        }
        #endregion

        #region 用户列表部分
        /// <summary>
        /// 用户管理入口
        /// </summary>
        /// <returns></returns>
        public IActionResult IndexForUser()
        {
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

            var userVMCollection = new List<ApplicationUserVM>();
            counter = 0;
            foreach (var user in userCollection)
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

            ViewBag.WhatItIs = "Users";   // 用于为 _RoleAndUserLayout 判断调用那个 PartialView 使用的
            ViewBag.RolesItems = boVMCollection;

            return View("../../Views/ApplicationOrganization/ApplicationRoleAndUser/IndexForUser", boVMCollection);
        }

        /// <summary>
        /// 用户列表数据
        /// </summary>
        /// <param name="id">角色 id</param>
        /// <returns></returns>
        public async Task<IActionResult> ListForApplicationUser(string id)
        {
            var roleID = (String.IsNullOrEmpty(id)) ? "" : id;
            IQueryable<ApplicationUser> userCollection;
            if (String.IsNullOrEmpty(roleID))
            {
                userCollection = from u in _UserManager.Users
                                 where u.Roles.Count == 0
                                 select u;
            }
            else
            {
                userCollection = from u in _UserManager.Users
                                 where u.Roles.Select(x => x.RoleId).Contains(roleID)
                                 select u;
            }

            var userVMCollection = new List<ApplicationUserVM>();
            var counter = 0;
            foreach (var user in userCollection)
            {
                var userVM = new ApplicationUserVM(user)
                {
                    OrderNumber = (++counter).ToString()
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
            return PartialView("../../Views/ApplicationOrganization/ApplicationRoleAndUser/_ListForUser", userVMCollection);
        }
        #endregion

        #region 新建用户数据处理部分
        /// <summary>
        /// 新建用户数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateApplicationUser()
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
            foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
            {
                var h = await _UserManager.IsInRoleAsync(user, item.Name);
                if (h)
                {
                    boVM.RoleItemIDCollection.Add(item.Id);
                }
            }
            #endregion
            ViewBag.RoleID = null;
            #endregion

            return PartialView("../../Views/ApplicationOrganization/ApplicationRoleAndUser/_CreateApplicationUser", boVM);
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
                        { IsPropertyName=false, MessageName="RemoteErr", Message="用户选择的用户名已经被使用了" });
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
                await _UserManager.CreateAsync(user);
                await _UserManager.AddPasswordAsync(user, boVM.Password);  // 添加密码
                var roleItem = await _RoleManager.FindByNameAsync("Adimn");
                
                foreach (var item in boVM.RoleItemIDCollection)             // 遍历所选择的用户组的ID集合，加入到相关的角色组
                {
                    var role = await _RoleManager.FindByIdAsync(item);
                    if (role != null)
                    {
                        await _UserManager.AddToRoleAsync(user, role.Name);
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

                return Json(validateMessage);

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
        public async Task<IActionResult> EditApplicationUser(Guid id, Guid roleID)
        {
            var user = await _UserManager.FindByIdAsync(id.ToString());
            var boVM = new ApplicationUserForEditVM(user, roleID);

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
            foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
            {
                var h = await _UserManager.IsInRoleAsync(user, item.Name);
                if (h)
                {
                    boVM.RoleItemIDCollection.Add(item.Id);
                }
            }
            #endregion
            #endregion

            return PartialView("../../Views/ApplicationOrganization/ApplicationRoleAndUser/_CreateOrEditForUser", boVM);
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
                foreach (var roleItem in roleCollection)
                {
                    // 当前的用户是否归属角色组：roleItem
                    var h = await _UserManager.IsInRoleAsync(user, roleItem.Name);
                    if (!h)
                    {
                        // 不归属，但前端已经选择了相应的角色组，则将当前用户加入相应的角色组。
                        if (boVM.RoleItemIDCollection.Contains(roleItem.Id))
                        {
                            await _UserManager.AddToRoleAsync(user, roleItem.Name);
                        }
                    }
                    else
                    {
                        // 归属，但前端并未选择相应的角色组，则将当前用户从相关的组中移除。
                        if (!boVM.RoleItemIDCollection.Contains(roleItem.Id))
                        {
                            await _UserManager.RemoveFromRoleAsync(user, roleItem.Name);
                        }
                    }
                }
                #endregion

                #region 3.重置密码
                #endregion
                validateMessage.IsOK = true;
                validateMessage.ValidateMessageItems.Add(
                    new ValidateMessageItem
                    {
                        IsPropertyName = false,
                        MessageName = "",
                        Message = boVM.RoleID.ToString()
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
                boVM.RoleItemIDCollection = new List<string>();
                foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
                {
                    var h = await _UserManager.IsInRoleAsync(user, item.Name);
                    if (h)
                    {
                        boVM.RoleItemIDCollection.Add(item.Id);
                    }
                }
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

        public IActionResult ResetUserPassword(string id)
        {
            var boVM = new ApplicationUserResetPasswordVM() {ID=id, Password="",ConfirmPassword="" };
            return PartialView("../../Views/ApplicationOrganization/ApplicationRoleAndUser/_ResetUserPassword", boVM);
        }

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


        #endregion
    }
}
