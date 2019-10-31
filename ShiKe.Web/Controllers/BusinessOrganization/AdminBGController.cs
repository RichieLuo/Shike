using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.DataAccess;
using ShiKe.ViewModels.BusinessOrganization;
using System.Linq.Expressions;
using ShiKe.DataAccess.Utilities;
using ShiKe.Common.ViewModelComponents;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using ShiKe.Entities.Attachments;
using Microsoft.AspNetCore.Http;
using ShiKe.Web.Controllers.ApplicationOrganization;
using ShiKe.ViewModels.Attachments;
using ShiKe.Common.JsonModels;
using Microsoft.AspNetCore.Identity;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.WebSettingManagement;
using ShiKe.DataAccess.Common;
using ShiKe.ViewModels.WebSettingManagement;

namespace ShiKe.Web.Controllers.BusinessOrganization
{
    [Authorize(Roles ="Admin")]
    public class AdminBGController : Controller
    {
        private readonly RoleManager<ApplicationRole> _RoleManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;

        private readonly IEntityRepository<SK_WM_Goods> _BoRepository;
        private readonly IEntityRepository<BusinessImage> _ImageRepository;
        private readonly IEntityRepository<SK_WM_Shop> _ShopRepository;
        private readonly IEntityRepository<SK_WM_ShopSttled> _ShopSelltedRepository;
        private readonly IEntityRepository<SK_SiteSettings> _SiteSettingsRepository;
        private IHostingEnvironment _HostingEnv;

        public AdminBGController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEntityRepository<SK_WM_Goods> repository,
            IHostingEnvironment hostingEnv,         
            IEntityRepository<BusinessImage> imageRepository,
            IEntityRepository<SK_WM_Shop> shopRepository,
             IEntityRepository<SK_WM_ShopSttled> shopSettled,
                 IEntityRepository<SK_SiteSettings> siteSettings
            )
        {
            _RoleManager = roleManager;
            _UserManager = userManager;
            _SignInManager = signInManager;
            _BoRepository = repository;
            _HostingEnv = hostingEnv;
            _ImageRepository = imageRepository;
            _ShopRepository = shopRepository;
            _ShopSelltedRepository = shopSettled;
            _SiteSettingsRepository = siteSettings;
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            var userName = User.Identity.Name;

            if (userName != null)
            {
                var user = _UserManager.FindByNameAsync(userName);
                return user.Result.ChineseFullName;
            }
            else
            {
                return null;
            }
        }

        ///// <summary>
        ///// 系统管理员登录界面
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public IActionResult AdminLogon()
        //{
        //    return View("../../Views/BusinessOrganization/AdminBG/AdminLogon");
        //}

        ///// <summary>
        ///// 系统管理员登录界面
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> AdminLogon(string jsonLogonInformation)
        //{
        //    var logonVM = Newtonsoft.Json.JsonConvert.DeserializeObject<LogonInformation>(jsonLogonInformation);
        //    var user = await _UserManager.FindByNameAsync(logonVM.UserName);
        //    if (user != null)
        //    {
        //        var result = await _SignInManager.PasswordSignInAsync(logonVM.UserName, logonVM.Password, false, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {
        //            // 下面的登录成功后的导航应该具体依赖用户所在的角色组来进行处理的。
        //            var returnUrl = Url.Action("Index", "AdminBG");
        //            return Json(new { result = true, messsage = returnUrl });
        //            //return RedirectToAction("../Account/EditProfile");
        //        }
        //        else
        //        {
        //            return Json(new { result = false, message = "用户名密码错误，请检查后重新处理。" });
        //        }
        //    }
        //    else
        //        return Json(new { result = false, message = "无法执行登录操作，请仔细检查后重新处理。" });

        //}

        /// <summary>
        /// 系统管理后台首页
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            //查询当前管理员的信息
            var currAdminUserName = User.Identity.Name;
            if (currAdminUserName == null)
            {
                return View("../../Views/BusinessOrganization/AdminBG/AdminLogon");
            }
            var currAdmin = await _UserManager.FindByNameAsync(currAdminUserName);
            var uID = Guid.Parse(currAdmin.Id);
            var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == uID).FirstOrDefault();
            if (image != null) { ViewBag.AvatarPath = image.UploadPath; }
            ViewBag.CurrAdminName = currAdmin.ChineseFullName;

            //统计用户数量(按时间统计暂未做)
            var AllUser = _UserManager.Users.Count();
            if (AllUser == 0) { ViewBag.AllUserCount = 0; }
            else { ViewBag.AllUserCount = AllUser; }

            //统计商品的数量(按时间统计暂未做)
            var AllGoods = await _BoRepository.GetAllAsyn();
            if (AllGoods.Count() == 0) { ViewBag.AllGoodsCount = 0; }
            else { ViewBag.AllGoodsCount = AllGoods.Count(); }

            //统计店铺数量(按时间统计暂未做)
            var AllShop = await _ShopRepository.GetAllAsyn();
            if (AllShop.Count() == 0)
            { ViewBag.AllShopCount = 0; }
            else { ViewBag.AllShopCount = AllShop.Count(); }

            //统计商家数量

            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/AdminBG/Index");
        }

        /// <summary>
        /// 获取网站信息
        /// </summary>
        /// <returns></returns>
        public SK_SiteSettingsVM SiteSettingsDetail()
        {
            ViewBag.UserLogonInformation = GetUserName();
            var siteSettings =  _SiteSettingsRepository.GetAllIncluding(x => x.Logo);
            var siteSetting = siteSettings.FirstOrDefault();
            if (siteSetting.Logo == null)
            {
                siteSetting.Logo = null;
            }
            var siteSettingVM = new SK_SiteSettingsVM(siteSetting);

            return siteSettingVM;
        }

        /// <summary>
        /// 网站信息配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SiteSettings()
        {  
            //查询当前管理员的信息
            var currAdminUserName = User.Identity.Name;
            if (currAdminUserName == null)
            {
                return View("../../Views/BusinessOrganization/AdminBG/AdminLogon");
            }
            //ViewBag.UserLogonInformation = GetUserName();

            //var siteSettings = await _SiteSettingsRepository.GetAllIncludingAsyn(x=>x.Logo);
            //var siteSetting = siteSettings.FirstOrDefault();
            //if (siteSetting.Logo==null)
            //{
            //    siteSetting.Logo = null;
            //}
            //var siteSettingVM = new SK_SiteSettingsVM(siteSetting);            
            return View("../../Views/BusinessOrganization/AdminBG/SiteSettings", SiteSettingsDetail());
        }

        [HttpPost]
        public async Task<IActionResult> SiteSettings([Bind("ID,Name,Suffix,DomainName,KeyWords,Description,Statistics,Logo,Copyright,ICP")]SK_SiteSettingsVM ssVM)
        {
            var siteVMs = await _SiteSettingsRepository.FindByAsyn(x => x.ID == ssVM.ID);
            SK_SiteSettings siteVM=siteVMs.FirstOrDefault();
            siteVM.Name = ssVM.Name;
            siteVM.Suffix = ssVM.Suffix;
            siteVM.DomainName = ssVM.DomainName;
            siteVM.KeyWords = ssVM.KeyWords;
            siteVM.Description = ssVM.Description;
            siteVM.Statistics = ssVM.Statistics;
            siteVM.Logo = null;
            siteVM.Copyright = ssVM.Copyright;
            siteVM.ICP = ssVM.ICP;

             _SiteSettingsRepository.EditAndSave(siteVM);

             return View("../../Views/BusinessOrganization/AdminBG/SiteSettings", SiteSettingsDetail());
        }

            /// <summary>
            /// 数据管理的入口
            /// </summary>
            /// <returns></returns>
            public async Task<IActionResult> AllGoods(Guid id)
        {
            ViewBag.UserLogonInformation = GetUserName();

            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = "Goods",           // 对应的归属类型ID
                PageIndex = 1,                 // 当前页码
                PageSize = 8,                  // 每页数据条数 为"0"时显示所有
                PageAmount = 0,                 // 相关对象列表分页处理分页数量
                ObjectAmount = 0,             // 相关的对象的总数
                Keyword = "",             // 当前的关键词
                SortProperty = "SortCode",         // 排序属性
                SortDesc = "default",            // 排序方向，缺省值正向 Default，前端用开关方式转为逆向：Descend
                SelectedObjectID = "",     // 当前页面处理中处理的焦点对象 ID
                IsSearch = false,             // 当前是否为检索
            };

            var typeID = "";
            var keyword = "";
            if (!String.IsNullOrEmpty(listPagePara.ObjectTypeID))
                typeID = listPagePara.ObjectTypeID;
            if (!String.IsNullOrEmpty(listPagePara.Keyword))
                keyword = listPagePara.Keyword;

            var username = User.Identity.Name;
            var user = await _UserManager.FindByNameAsync(username);
            if (user == null)
            {
                return View("../../Views/Home/Logon");
            }
            //var pageIndex = 1;
            //var pageSize = 50;
            //var boCollection = new List<SK_WM_Goods>();

            var boCollection = await _BoRepository.PaginateAsyn(listPagePara.PageIndex, listPagePara.PageSize, x => x.SortCode, null, x => x.SK_WM_GoodsCategory);

           var GoodsCount =await _BoRepository.GetAllAsyn();
            ViewBag.GoodsCount = GoodsCount.Count();

            var shop = await _ShopRepository.FindByAsyn(x => x.BelongToUserID == user.Id);

            var boVMCollection = new List<SK_WM_GoodsVM>();

            var orderNumber = 0;

            //var goodsCollectionPageList = boCollection.ToPaginatedList(listPagePara.PageIndex, listPagePara.PageSize);

            foreach (var bo in boCollection)
            {
                //if (bo.BelongToShopID == shop.FirstOrDefault().ID.ToString())
                //{
                var boVM = new SK_WM_GoodsVM(bo);
                var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);
                if (images != null)
                {
                    foreach (var img in images)
                    {
                        if (img.IsForTitle == true)
                        {
                            boVM.AvatarPath = img.UploadPath;
                        }
                    }
                }
                boVM.OrderNumber = (++orderNumber).ToString();
                boVMCollection.Add(boVM);
                //}
            }
            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_Goods>(boCollection, 10, listPagePara.PageIndex);
            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;
            ViewBag.GoodsType = "Admin";

            return View("../../Views/BusinessOrganization/AdminBG/AllGoods", boVMCollection);

        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> List(int pageIndex)
        {
            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = "Goods",           // 对应的归属类型ID
                PageIndex = pageIndex,                 // 当前页码
                PageSize = 8,                  // 每页数据条数 为"0"时显示所有
                PageAmount = 0,                 // 相关对象列表分页处理分页数量
                ObjectAmount = 0,             // 相关的对象的总数
                Keyword = "",             // 当前的关键词
                SortProperty = "SortCode",         // 排序属性
                SortDesc = "default",            // 排序方向，缺省值正向 Default，前端用开关方式转为逆向：Descend
                SelectedObjectID = "",     // 当前页面处理中处理的焦点对象 ID
                IsSearch = false,             // 当前是否为检索
            };

            var typeID = "";
            var keyword = "";
            if (!String.IsNullOrEmpty(listPagePara.ObjectTypeID))
                typeID = listPagePara.ObjectTypeID;
            if (!String.IsNullOrEmpty(listPagePara.Keyword))
                keyword = listPagePara.Keyword;

            var username = User.Identity.Name;
            var user = await _UserManager.FindByNameAsync(username);
            if (user == null)
            {
                return View("../../Views/Home/Logon");
            }
            //var pageIndex = 1;
            //var pageSize = 50;
            //var boCollection = new List<SK_WM_Goods>();

            var boCollection = await _BoRepository.PaginateAsyn(listPagePara.PageIndex, listPagePara.PageSize, x => x.SortCode, null, x => x.SK_WM_GoodsCategory);


            var shop = await _ShopRepository.FindByAsyn(x => x.BelongToUserID == user.Id);

            var boVMCollection = new List<SK_WM_GoodsVM>();

            var orderNumber = 0;



            foreach (var bo in boCollection)
            {
                //if (bo.BelongToShopID == shop.FirstOrDefault().ID.ToString())
                //{
                var boVM = new SK_WM_GoodsVM(bo);
                var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);
                if (images != null)
                {
                    foreach (var img in images)
                    {
                        if (img.IsForTitle == true)
                        {
                            boVM.AvatarPath = img.UploadPath;
                        }
                    }
                    //}
                    boVM.OrderNumber = (++orderNumber).ToString();
                    boVMCollection.Add(boVM);
                }
            }
            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_Goods>(boCollection, 10, listPagePara.PageIndex);
            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;
            //return View("../../Views/BusinessOrganization/BusinessBG/Index", boVMCollection);
            //}
            return PartialView("../../Views/BusinessOrganization/PublicView/_List", boVMCollection);
        }

        /// <summary>
        /// 商家店铺管理首页
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> IndexForShop(/*Guid id*/)
        {
            //var shopVM = await _ShopRepository.GetSingleAsyn(id);

            var shops = await _ShopRepository.GetAllAsyn();
            var shop = shops.FirstOrDefault();

            var shopVM = new SK_WM_ShopVM(shop);

            //var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == shopVM.ID).FirstOrDefault();
            //if (image != null)
            //{
            //    shopVM.ShopAvatarPath = image.UploadPath;
            //}
            //shop.ShopAvatar = image;

            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/AdminBG/IndexForShop", shopVM);
        }
        /// <summary>
        /// 商家店铺信息设置
        /// </summary>
        /// <returns></returns>
        public IActionResult ShopSet()
        {
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/AdminBG/ShopSet");
        }

        /// <summary>
        /// 所有的商家
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AllBusiness()
        {
            ViewBag.UserLogonInformation = GetUserName();
            var AllShopSttled = await _ShopSelltedRepository.GetAllIncludingAsyn(x=>x.ShopForUser);
            var ShopSttleds= AllShopSttled.Where(x => x.State == 1 && x.Step == 3);
            if (AllShopSttled.Count()==0|| ShopSttleds.Count()==0)
            {
                ViewBag.Business = null;
            }
            else
            {
                ViewBag.Business = ShopSttleds;
            }
            return View("../../Views/BusinessOrganization/AdminBG/AllBusiness");
        }


        /// <summary>
        /// 所有违规的店铺
        /// </summary>
        /// <returns></returns>
        public IActionResult AllIllegalShop()
        {
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/AdminBG/AllIllegalShop");
        }

        /// <summary>
        /// 站点数据统计首页
        /// </summary>
        /// <returns></returns>
        public IActionResult DataStatistics()
        {
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/AdminBG/DataStatistics");
        }

        ///// <summary>
        ///// 根据店铺ID获取 店铺的商品
        ///// </summary>
        ///// <returns></returns>
        //public async Task<IActionResult> Index(Guid id)
        //{
        //    Guid id2 = new Guid();
        //    var spCollection = await _BoRepository.GetAllAsyn();

        //    var spVMCollection = new List<SK_WM_GoodsVM>();

        //    foreach (var sp in spCollection)
        //    {
        //        if (sp.Shop == null)
        //        {
        //            var Sh = await _ShopRepository.GetAllAsyn();
        //            foreach (var item in Sh)
        //            {
        //                if (item.Name == sp.Name)
        //                {
        //                    sp.Shop.ID = item.ID;
        //                }
        //            }
        //        }
        //        if (id != null && id != id2)
        //        {
        //            if (sp.Shop.ID == id)
        //            {
        //                var spVM = new SK_WM_GoodsVM(sp);
        //                spVMCollection.Add(spVM);
        //            }
        //        }
        //        else
        //        {
        //            var spVM = new SK_WM_GoodsVM(sp);
        //            spVMCollection.Add(spVM);
        //        }
        //    }
        //    return View("../../Views/BusinessOrganization/BusinessBG/Index", spVMCollection);
        //}


        //public async Task<IActionResult> List(Guid id)
        //{
        //    //var boVMCollection = new List<SK_WM_GoodsVM>();
        //    //if (!String.IsNullOrEmpty(keyword))
        //    //{
        //    //    Expression<Func<SK_WM_Goods, bool>> condition = x =>
        //    //    x.Name.Contains(keyword) ||
        //    //    x.Description.Contains(keyword) ||
        //    //    x.SortCode.Contains(keyword);

        //    //    var boCollection = await _BoRepository.FindByAsyn(condition);
        //    //    boCollection.OrderByDescending(x => x.ShelvesTime);
        //    //    foreach (var bo in boCollection)
        //    //    {
        //    //        boVMCollection.Add(new SK_WM_GoodsVM(bo));
        //    //    }
        //    //}
        //    //else
        //    //{
        //    var pageIndex = 1;
        //    var pageSize = 50;
        //    var boCollection = new List<SK_WM_Goods>();

        //    boCollection = await _BoRepository.PaginateAsyn(pageIndex, pageSize, x => x.SortCode, null, x => x.SK_WM_GoodsCategory);

        //    var shop = await _ShopRepository.GetSingleAsyn(id);


        //    var boVMCollection = new List<SK_WM_GoodsVM>();
        //    boCollection.OrderByDescending(x => x.ShelvesTime);
        //    var orderNumber = 0;
        //    if (id == new Guid())
        //    {

        //        foreach (var bo in boCollection)
        //        {
        //            var boVM = new SK_WM_GoodsVM(bo);
        //            var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID).FirstOrDefault();
        //            if (image != null)
        //            {
        //                boVM.AvatarPath = image.UploadPath;
        //            }
        //            boVM.OrderNumber = (++orderNumber).ToString();
        //            boVMCollection.Add(boVM);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var bo in boCollection)
        //        {
        //            if (bo.Shop == shop)
        //            {
        //                var boVM = new SK_WM_GoodsVM(bo);
        //                var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID).FirstOrDefault();
        //                if (image != null)
        //                {
        //                    boVM.AvatarPath = image.UploadPath;
        //                }
        //                boVM.OrderNumber = (++orderNumber).ToString();
        //                boVMCollection.Add(boVM);
        //            }
        //        }
        //    }
        //    //return View("../../Views/BusinessOrganization/BusinessBG/Index", boVMCollection);
        //    //}
        //    return PartialView("../../Views/BusinessOrganization/PublicView/_List", boVMCollection);
        //}

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(Guid id)
        {
            var isNew = false;
            var bo = await _BoRepository.GetSingleAsyn(id, x => x.SK_WM_GoodsCategory);
            if (bo == null)
            {
                bo = new SK_WM_Goods();
                bo.Name = "";
                bo.ShelvesTime = DateTime.Now;
                bo.ModifyTime = DateTime.Now;
                bo.Description = "";
                //bo.IMG = "";
                isNew = true;
            }
            bo.ModifyTime = DateTime.Now;
            var boVM = new SK_WM_GoodsVM(bo);
            var goodsCategorys = _BoRepository.EntitiesContext.SK_WM_GoodsCategory.ToList();

            var selectItems = new List<PlainFacadeItem>();
            foreach (var item in goodsCategorys)
            {
                selectItems.Add(new PlainFacadeItem() { ID = item.ID.ToString(), Name = item.Name, SortCode = item.SortCode, DisplayName = item.Name });
            }
            boVM.GoodsCategoryCollection = selectItems;
            boVM.IsNew = isNew;

            return View("../../Views/BusinessOrganization/AdminBG/CreateOrEdit", boVM);
        }

        //[HttpGet]
        //[HttpPost]
        //public async Task<IActionResult> EditForGoods(Guid id)
        //{
        //    var isNew = false;
        //    var bo = await _BoRepository.GetSingleAsyn(id, x => x.SK_WM_GoodsCategory);
        //    if (bo == null)
        //    {
        //        bo = new SK_WM_Goods();
        //        bo.Name = "";
        //        bo.ShelvesTime = DateTime.Now;
        //        bo.ModifyTime = DateTime.Now;
        //        bo.Description = "";
        //        //bo.IMG = "";
        //        isNew = true;
        //    }
        //    bo.ModifyTime = DateTime.Now;
        //    var boVM = new SK_WM_GoodsVM(bo);
        //    var goodsCategorys = _BoRepository.EntitiesContext.SK_WM_GoodsCategory.ToList();

        //    var selectItems = new List<PlainFacadeItem>();
        //    foreach (var item in goodsCategorys)
        //    {
        //        selectItems.Add(new PlainFacadeItem() { ID = item.ID.ToString(), Name = item.Name, SortCode = item.SortCode, DisplayName = item.Name });
        //    }
        //    boVM.GoodsCategoryCollection = selectItems;
        //    boVM.IsNew = isNew;

        //    return View("../../Views/BusinessOrganization/PublicView/_CreateOrEditForGoods", boVM);
        //}


        //[HttpPost]
        //public async Task<IActionResult> Save([Bind("ID,IsNew,Name,GoodsCategoryID,Description,SortCode,ShelvesTime,ModifyTime,Price,FacadePrice,Unit,SalesVolume,Stock,State")]SK_WM_GoodsVM boVM)
        //{
        //    var validateMessage = new ValidateMessage();
        //    if (ModelState.IsValid)
        //    {
        //        var hasDuplicateNmaeGoods = await _BoRepository.HasInstanceAsyn(x => x.Name == boVM.Name);
        //        if (hasDuplicateNmaeGoods && boVM.IsNew)
        //        {
        //            validateMessage.IsOK = false;
        //            validateMessage.ValidateMessageItems.Add(
        //                new ValidateMessageItem()
        //                { IsPropertyName = false, MessageName = "RemoteErr", Message = "商品名重复，换一个试试看吧" });
        //            return Json(validateMessage);
        //        }

        //        var bo = new SK_WM_Goods();
        //        if (!boVM.IsNew)
        //        {
        //            bo = await _BoRepository.GetSingleAsyn(boVM.ID);

        //        }

        //        // 处理一般的属性数据
        //        boVM.MapToBo(bo);

        //        // 处理关联数据
        //        if (!String.IsNullOrEmpty(boVM.GoodsCategoryID))
        //        {
        //            var skwmgclssID = Guid.Parse(boVM.GoodsCategoryID);
        //            var skwmgclss = _BoRepository.EntitiesContext.SK_WM_GoodsCategory.FirstOrDefault(x => x.ID == skwmgclssID);
        //            bo.SK_WM_GoodsCategory = skwmgclss;
        //        }

        //        var saveStatus = false;

        //        //处理上传文件
        //        var serverPath = "";
        //        long size = 0;
        //        var files = Request.Form.Files;
        //        if (files.Count() > 0)
        //        {
        //            foreach (var file in files)
        //            {
        //                var fileName = ContentDispositionHeaderValue
        //                        .Parse(file.ContentDisposition)
        //                        .FileName
        //                        .Trim('"')
        //                        .Substring(files[0].FileName.LastIndexOf("\\") + 1);

        //                fileName = bo.ID + "_" + fileName;

        //                var boPath = "../../images/goodsImg/" + fileName;
        //                fileName = _HostingEnv.WebRootPath + $@"\images\goodsImg\{fileName}";
        //                serverPath = fileName;
        //                size += file.Length;
        //                using (FileStream fs = System.IO.File.Create(fileName))
        //                {
        //                    file.CopyTo(fs);
        //                    fs.Flush();
        //                }

        //                var businessIamge = new BusinessImage()
        //                {
        //                    DisplayName = bo.Name,
        //                    RelevanceObjectID = bo.ID,
        //                    UploadPath = boPath
        //                };

        //                bo.GoodsIMG = businessIamge;
        //            }
        //        }


        //        saveStatus = await _BoRepository.AddOrEditAndSaveAsyn(bo);

        //        if (saveStatus)
        //        {
        //            validateMessage.IsOK = true;
        //            validateMessage.ValidateMessageItems.Add(
        //                new ValidateMessageItem
        //                {
        //                    IsPropertyName = false,
        //                    MessageName = "Succeed",
        //                    Message = ""
        //                });

        //            return Json(validateMessage);
        //        }
        //        else
        //        {
        //            validateMessage.IsOK = false;
        //            validateMessage.ValidateMessageItems.Add(
        //                new ValidateMessageItem()
        //                { IsPropertyName = false, MessageName = "RemoteErr", Message = "数据保存异常" });
        //            return Json(validateMessage);
        //        }


        //    }
        //    else
        //    {
        //        //return View("../../Views/BusinessOrganization/BusinessBG/CreateOrEdit", boVM);
        //        validateMessage.IsOK = false;
        //        var errCollection = from errKey in ModelState.Keys
        //                            from errMessage in ModelState[errKey].Errors
        //                            where ModelState[errKey].Errors.Count > 0
        //                            select (new { errKey, errMessage.ErrorMessage });

        //        foreach (var errItem in errCollection)
        //        {
        //            var vmItem = new ValidateMessageItem()
        //            {
        //                IsPropertyName = true,
        //                MessageName = errItem.errKey,
        //                Message = errItem.ErrorMessage
        //            };
        //            validateMessage.ValidateMessageItems.Add(vmItem);
        //        }
        //        return Json(validateMessage);
        //    }
        //}
        /// <summary>
        /// 以局部页的方式的方式，构建明细数据的处理
        /// </summary>
        /// <param name="id">商品对象的ID属性值</param>
        //public async Task<IActionResult> Detail(Guid id)
        //{
        //    var bo = await _BoRepository.GetSingleAsyn(id, x => x.SK_WM_GoodsCategory);
        //    var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);
        //    var imgCount = 0;
        //    foreach (var img in images)
        //    {
        //        bo.GoodsIMG = img;
        //        imgCount++;
        //    }
        //    ViewBag.imgCount = imgCount;
        //    ViewBag.DetailImgs = images;
        //    var boVM = new SK_WM_GoodsVM(bo);
        //    return PartialView("../../Views/BusinessOrganization/PublicView/_Detail", boVM);
        //}

        /// <summary>
        /// 删除数据，将删除操作的结果加载到 DeleteStatus 对象，然后转成 json 数据返回给前端。
        /// </summary>
        /// <param name="id">待删除的对象 ID 属性值</param>
        /// <returns>将删除操作处理的结果转为 json 数据返回到前端，供前端根据情况处理</returns>
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var status = await _BoRepository.DeleteAndSaveAsyn(id);
        //    return Json(status);
        //}


        public async Task<IActionResult> SaveImg(Guid ID)
        {
            var serverPath = "";
            long size = 0;
            var files = Request.Form.Files;
            if (files.Count == 0)
            {
                return Json(new { isOK = false, fileCount = 0, size = size, serverPath = "没有选择任何文件，请选择文件后，再提交上传。" });
            }
            else
            {
                var goodsID = ID;
                var goods = await _BoRepository.GetSingleAsyn(goodsID);
                foreach (var file in files)
                {
                    var fileName = ContentDispositionHeaderValue
                                .Parse(files[0].ContentDisposition)
                                .FileName
                                .Trim('"')
                                .Substring(files[0].FileName.LastIndexOf("\\") + 1);

                    fileName = goods.ID + "_" + fileName;

                    var boPath = "../../images/goodsImg/" + fileName;
                    fileName = _HostingEnv.WebRootPath + $@"\images\goodsImg\{fileName}";
                    serverPath = fileName;
                    size += files[0].Length;
                    using (FileStream fs = System.IO.File.Create(fileName))
                    {
                        files[0].CopyTo(fs);
                        fs.Flush();
                    }

                    var businessIamge = new BusinessImage()
                    {
                        DisplayName = goods.Name,
                        RelevanceObjectID = goods.ID,
                        UploadPath = boPath
                    };
                    var uID = goods.ID;
                    var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == uID).FirstOrDefault();
                    if (image != null)
                    {
                        image.UploadPath = boPath;
                        _ImageRepository.EditAndSave(image);
                    }
                    else
                    {
                        goods.GoodsIMG = businessIamge;
                        _BoRepository.Edit(goods);
                    }
                }
                return Json(new { isOK = true, fileCount = files.Count, size = size, serverPath = serverPath });
            }
        }

        public async Task<IActionResult> ImagesEdit(Guid id)
        {
            var boCollection = new List<BusinessImage>();

            boCollection = await _ImageRepository.GetAllListAsyn();

            var orderNumber = 0;
            var boVMCollection = new List<BusinessImageVM>();

            var Goods = _BoRepository.GetSingleBy(x => x.ID == id);
            ViewBag.Goods = Goods;

            foreach (var bo in boCollection)
            {
                if (bo.RelevanceObjectID == id)
                {
                    var boVM = new BusinessImageVM(bo);
                    boVM.OrderNumber = (++orderNumber).ToString();
                    boVMCollection.Add(boVM);
                }
            }

            return View("../../Views/BusinessOrganization/AdminBG/ImagesEdit", boVMCollection);
        }

        /// <summary>
        /// 网页未完成
        /// </summary>
        /// <returns></returns>
        public IActionResult Unfinished()
        {
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/AdminBG/Unfinished");
        }
    }
}