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
using ShiKe.DataAccess.SqlServer.Ultilities;
using ShiKe.Entities.ApplicationOrganization;
using Microsoft.AspNetCore.Identity;
using ShiKe.DataAccess.Common;
using ShiKe.Common.JsonModels;

namespace ShiKe.Web.Controllers.BusinessOrganization
{
    [Authorize]
    public class BusinessBGController : Controller
    {
        private readonly IEntityRepository<SK_WM_Goods> _BoRepository;
        private readonly IEntityRepository<BusinessImage> _ImageRepository;
        private readonly IEntityRepository<SK_WM_Shop> _ShopRepository;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IEntityRepository<SK_WM_Order> _OrderRepository;
        private readonly IEntityRepository<SK_WM_OrderItem> _OrderItemRepository;
        private IHostingEnvironment _HostingEnv;
        private readonly IEntityRepository<SK_WM_ShopCar> _ShopCarRepository;
        private readonly IEntityRepository<SK_WM_ShopCarGoodsItem> _ShopCarGoodsItemRepository;
        private readonly IEntityRepository<SK_WM_Goods> _GoodsRepository;
        private readonly IEntityRepository<SK_WM_Order> _Order;


        public BusinessBGController(
            IEntityRepository<SK_WM_Goods> repository,
            IHostingEnvironment hostingEnv,
            UserManager<ApplicationUser> userManager,
            IEntityRepository<BusinessImage> imageRepository,
            IEntityRepository<SK_WM_Order> orderRepository,
            IEntityRepository<SK_WM_OrderItem> orderItemRepository,
            IEntityRepository<SK_WM_ShopCar> shopCarRepository,
            IEntityRepository<SK_WM_ShopCarGoodsItem> shopCarGoodsItemRepository,
            IEntityRepository<SK_WM_Shop> shopRepository,
            IEntityRepository<SK_WM_Goods> goodsRepository,
            IEntityRepository<SK_WM_Order> order
            )
        {
            _UserManager = userManager;
            _BoRepository = repository;
            _HostingEnv = hostingEnv;
            _ShopCarRepository = shopCarRepository;
            _ShopCarGoodsItemRepository = shopCarGoodsItemRepository;
            _ImageRepository = imageRepository;
            _ShopRepository = shopRepository;
            _OrderRepository = orderRepository;
            _OrderItemRepository = orderItemRepository;
            _GoodsRepository = goodsRepository;
            _Order = order;
        }


        public string GetUserName()
        {
            var userName = User.Identity.Name;

            if (userName != null)
            {
                var user = _UserManager.FindByNameAsync(userName);
                if (user != null)
                {
                    return user.Result.ChineseFullName;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public string GetCount(string type, string shop)
        {
            IQueryable<SK_WM_OrderItem> orderItem;
            var count = "0";
            if (type == "待付款")
            {
                orderItem = _OrderItemRepository.FindBy(x => x.State == type);
                orderItem = orderItem.Where(x => x.ShopName == shop);
                count = orderItem.Count().ToString();

            }
            else if (type == "待发货")
            {
                orderItem = _OrderItemRepository.FindBy(x => x.State == type);
                orderItem = orderItem.Where(x => x.ShopName == shop);
                count = orderItem.Count().ToString();

            }
            else if (type == "待收货")
            {
                orderItem = _OrderItemRepository.FindBy(x => x.State == type);
                orderItem = orderItem.Where(x => x.ShopName == shop);
                count = orderItem.Count().ToString();

            }

            return count;
        }

        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            var user = await _UserManager.FindByNameAsync(username);
            if (user == null)
            {
                return View("../../Views/Home/Logon");
            }
            var shop = await _ShopRepository.FindByAsyn(x => x.BelongToUserID == user.Id);

            if (shop.Count() == 0)
            {
                return Redirect("../ShopManager/Settled");
            }

            var boVM = new SK_WM_ShopVM(shop.FirstOrDefault());
            foreach (var state in Enum.GetValues(typeof(SK_WM_OrderState.Orderstate)))
            {
                if (state.ToString() == "待付款" || state.ToString() == "待发货" || state.ToString() == "待收货")
                {
                    if (state.ToString() == "待付款")
                    {
                        boVM.waitPay = GetCount(state.ToString(), shop.FirstOrDefault().Name);
                    }
                    else if (state.ToString() == "待发货")
                    {
                        boVM.waitSend = GetCount(state.ToString(), shop.FirstOrDefault().Name);
                    }
                    else if (state.ToString() == "待收货")
                    {
                        boVM.waitReceipt = GetCount(state.ToString(), shop.FirstOrDefault().Name);
                    }
                }
            }
            ViewBag.UserLogonInformation = GetUserName();

            return View("../../Views/BusinessOrganization/BusinessBG/Index", boVM);
        }
        /// <summary>
        /// 店铺信息
        /// </summary>
        /// <returns></returns>

        public async Task<IActionResult> ShopInformation()
        {
            int numorder = 0;//存储订单数量
            int NumGoods = 0;//存储订单数量
            var user = User.Claims.FirstOrDefault();
            if (user.Value == new Guid().ToString())
            {
                return View("../../Views/Home/Logon");
            }
            ViewBag.UserLogonInformation = GetUserName();
            //var userData = _UserManager.FindByIdAsync(user.Value);
            var ShopRepository = _ShopRepository.GetSingleBy(x => x.ShopForUser.Id == user.Value);
            var OrderItem = await _OrderItemRepository.FindByAsyn(x => x.ShopName == ShopRepository.Name);
            if (Convert.ToInt32(OrderItem.Count()) != 0)
            {
                foreach (var order in OrderItem)
                {
                    numorder = numorder + order.Count;
                }
            }
            var imgRepository = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == ShopRepository.ID);
            ViewBag.imgshopBanner = null;
            ViewBag.imgshopAvatar = null;
            foreach (var img in imgRepository)
            {
                if (img.Description == "shopBanner")
                {
                    ViewBag.imgshopBanner = img.UploadPath;
                }
                if (img.Description == "shopAvatar")
                {
                    ViewBag.imgshopAvatar = img.UploadPath;
                }
            }
            ViewBag.numorder = numorder;
            var GoodsRepository = await _GoodsRepository.FindByAsyn(x => x.Shop.ID == ShopRepository.ID);
            NumGoods = GoodsRepository.Count();
            ViewBag.NumGoods = NumGoods.ToString();
            return View("../../Views/BusinessOrganization/BusinessBG/ShopInformation", ShopRepository);
        }


        /// <summary>
        /// 数据管理的入口
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GoodsList()
        {
            ViewBag.UserLogonInformation = GetUserName();

            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = "Goods",            // 对应的归属类型ID
                PageIndex = 1,                     // 当前页码
                PageSize = 10,                     // 每页数据条数 为"0"时显示所有
                PageAmount = 0,                    // 相关对象列表分页处理分页数量
                ObjectAmount = 0,                  // 相关的对象的总数
                Keyword = "",                      // 当前的关键词
                SortProperty = "SortCode",         // 排序属性
                SortDesc = "default",              // 排序方向，缺省值正向 Default，前端用开关方式转为逆向：Descend
                SelectedObjectID = "",   // 当前页面处理中处理的焦点对象 ID
                IsSearch = false,                  // 当前是否为检索
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

            var boCollection = await _BoRepository.PaginateAsyn(listPagePara.PageIndex, listPagePara.PageSize, x => x.SortCode, null, x => x.SK_WM_GoodsCategory);

            var shop = await _ShopRepository.FindByAsyn(x => x.BelongToUserID == user.Id);

            var boVMCollection = new List<SK_WM_GoodsVM>();

            var orderNumber = 0;

            //var goodsCollectionPageList = boCollection.ToPaginatedList(listPagePara.PageIndex, listPagePara.PageSize);

            foreach (var bo in boCollection)
            {
                if (bo.BelongToShopID == shop.FirstOrDefault().ID.ToString())
                {
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
                }
            }
            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_Goods>(boCollection, 10, listPagePara.PageIndex);
            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;
            ViewBag.GoodsType = "User";
            return PartialView("../../Views/BusinessOrganization/BusinessBG/GoodsList", boVMCollection);


        }
        public IActionResult SpecialtyExamine(string selectNnm)
        {
            ViewBag.UserLogonInformation = GetUserName();
            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = "Order",            // 对应的归属类型ID
                PageIndex = 1,                     // 当前页码
                PageSize = 1,                     // 每页数据条数 为"0"时显示所有
                PageAmount = 0,                    // 相关对象列表分页处理分页数量
                ObjectAmount = 0,                  // 相关的对象的总数
                Keyword = "",                      // 当前的关键词
                SortProperty = "SortCode",         // 排序属性
                SortDesc = "default",              // 排序方向，缺省值正向 Default，前端用开关方式转为逆向：Descend
                SelectedObjectID = "",   // 当前页面处理中处理的焦点对象 ID
                IsSearch = false,                  // 当前是否为检索
            };
            listPagePara.PageIndex = selectNnm != null ? Convert.ToInt32(selectNnm) : 1;
            var user = User.Claims.FirstOrDefault();
            if (user.Value == new Guid().ToString())
            {
                return View("../../Views/Home/Logon");
            }
            var shop = _ShopRepository.GetSingleBy(x => x.ShopForUser.Id == user.Value);
            //查询订单详细所关联的商品列表
            var GoodsItems = _GoodsRepository.FindBy(x => x.Shop.ID == shop.ID);//获取订单和订单对应的用户和商品
            var orVMCollection = new List<SK_WM_GoodsVM>();//存放用户所有订单

            var OrderAll = GoodsItems.AsQueryable<SK_WM_Goods>();
            var GoodsCollectionPageList = IQueryableExtensions.ToPaginatedList(OrderAll, listPagePara.PageIndex, listPagePara.PageSize);
            foreach (var order in GoodsCollectionPageList)
            {
                var omVM = new SK_WM_GoodsVM(order);
                orVMCollection.Add(omVM);
            }

            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_Goods>(GoodsCollectionPageList, 3, listPagePara.PageIndex);

            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;

            return View("../../Views/BusinessOrganization/BusinessBG/SpecialtyExamine", orVMCollection);

        }



        /// <summary>
        /// 商家店铺信息设置
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ShopSet()
        {
            var user = User.Claims.FirstOrDefault();
            if (user.Value == new Guid().ToString())
            {
                return View("../../Views/Home/Logon");
            }
            ViewBag.UserLogonInformation = GetUserName();
            //var userData = _UserManager.FindByIdAsync(user.Value);
            var ShopRepository = _ShopRepository.GetSingleBy(x => x.ShopForUser.Id == user.Value);
            var imgRepository = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == ShopRepository.ID);
            ViewBag.imgshopBanner = null;
            ViewBag.imgshopAvatar = null;
            foreach (var img in imgRepository)
            {
                if (img.Description == "shopBanner")
                {
                    ViewBag.imgshopBanner = img.UploadPath;
                }
                if (img.Description == "shopAvatar")
                {
                    ViewBag.imgshopAvatar = img.UploadPath;
                }
            }
            var shopVm = new SK_WM_ShopVM(ShopRepository);
            return View("../../Views/BusinessOrganization/BusinessBG/ShopSet", shopVm);
        }

        public async Task<IActionResult> SavaShop([Bind("ID,IsNew,Name,GoodsCategoryID,Description,SortCode,Grade,Collection,Telephone,Adress,SettledDateTime,State,ShopAvatar,ShopBanner")]SK_WM_Shop bo)
        {
            var files = Request.Form.Files;
            var shopData = await _ShopRepository.GetSingleAsyn(bo.ID,x=>x.ShopForUser,y=>y.ShopForExecuteIllegal);
            //shopData.ID = shopData.ID;
            shopData.Name = bo.Name;
            shopData.Description = bo.Description;
            shopData.Telephone = bo.Telephone;
            //shopData.ShopAvatar = null;
            //shopData.ShopBanner = null;
            if (files.Count() != 0)
            {
                foreach (var item in files)
                {
                    if (bo.ShopBanner != null)
                    {
                        shopData.ShopBanner = bo.ShopBanner;
                    }
                }
            }
            await _ShopRepository.AddOrEditAndSaveAsyn(shopData);
            return RedirectToAction("ShopInformation");
        }


        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> List(int pageIndex, string objectTypeID)
        {
            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = objectTypeID,           // 对应的归属类型ID
                PageIndex = pageIndex,                 // 当前页码
                PageSize = 10,                  // 每页数据条数 为"0"时显示所有
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

            if (listPagePara.ObjectTypeID == "Admin")
            {
                foreach (var bo in boCollection)
                {
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
                }
                ViewBag.GoodsType = "Admin";
            }
            else
            {
                foreach (var bo in boCollection)
                {
                    if (bo.BelongToShopID == shop.FirstOrDefault().ID.ToString())
                    {
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
                    }
                }
                ViewBag.GoodsType = "User";
            }

            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_Goods>(boCollection, 10, listPagePara.PageIndex);
            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;

            ViewBag.UserLogonInformation = GetUserName();
            return PartialView("../../Views/BusinessOrganization/PublicView/_List", boVMCollection);
        }

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
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/BusinessBG/CreateOrEdit", boVM);
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> EditForGoods(Guid id)
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
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/PublicView/_CreateOrEditForGoods", boVM);
        }


        [HttpPost]
        public async Task<IActionResult> Save([Bind("ID,IsNew,Name,GoodsCategoryID,Description,SortCode,ShelvesTime,ModifyTime,Price,FacadePrice,Unit,SalesVolume,Stock,State")]SK_WM_GoodsVM boVM)
        {
            var validateMessage = new ValidateMessage();
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var user = await _UserManager.FindByNameAsync(username);
                var shop = await _ShopRepository.FindByAsyn(x => x.BelongToUserID == user.Id);
                var hasDuplicateNmaeGoods = await _BoRepository.HasInstanceAsyn(x => x.Name == boVM.Name);
                if (hasDuplicateNmaeGoods && boVM.IsNew)
                {
                    validateMessage.IsOK = false;
                    validateMessage.ValidateMessageItems.Add(
                        new ValidateMessageItem()
                        { IsPropertyName = false, MessageName = "Name", Message = "商品名重复，换一个试试看吧" });
                    return Json(validateMessage);
                }

                var bo = new SK_WM_Goods();
                if (!boVM.IsNew)
                {
                    bo = await _BoRepository.GetSingleAsyn(boVM.ID);
                    if (bo.BelongToUserID != user.Id)
                    {

                        validateMessage.IsOK = false;
                        validateMessage.ValidateMessageItems.Add(
                        new ValidateMessageItem()
                        { IsPropertyName = false, MessageName = "Name", Message = "该商品不是你的，你没有权限编辑" });
                        return Json(validateMessage);
                    }
                }

                // 处理一般的属性数据
                boVM.MapToBo(bo);

                // 处理关联数据
                if (!String.IsNullOrEmpty(boVM.GoodsCategoryID))
                {
                    var skwmgclssID = Guid.Parse(boVM.GoodsCategoryID);
                    var skwmgclss = _BoRepository.EntitiesContext.SK_WM_GoodsCategory.FirstOrDefault(x => x.ID == skwmgclssID);
                    bo.SK_WM_GoodsCategory = skwmgclss;
                }

                var saveStatus = false;

                //处理上传文件
                var serverPath = "";
                long size = 0;
                var files = Request.Form.Files;

                if (files.Count() > 0)
                {
                    int i = 0;
                    foreach (var file in files)
                    {
                        var fileName = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"')
                                .Substring(files[0].FileName.LastIndexOf("\\") + 1);

                        fileName = bo.ID + "_" + fileName;

                        var boPath = "../../images/goodsImg/" + fileName;
                        fileName = _HostingEnv.WebRootPath + $@"\images\goodsImg\{fileName}";
                        serverPath = fileName;
                        size += file.Length;
                        using (FileStream fs = System.IO.File.Create(fileName))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }

                        var businessIamge = new BusinessImage()
                        {
                            DisplayName = bo.Name,
                            RelevanceObjectID = bo.ID,
                            UploadPath = boPath
                        };
                        if (i == 0)
                        {
                            businessIamge.IsForTitle = true;
                        }

                        bo.GoodsIMG = businessIamge;
                        i++;
                        await _ImageRepository.AddOrEditAndSaveAsyn(businessIamge);
                    }
                }

                bo.ShopForUser = user;
                bo.BelongToUserID = user.Id;
                bo.BelongToShopID = shop.FirstOrDefault().ID.ToString();
                bo.Shop = shop.FirstOrDefault();
                saveStatus = await _BoRepository.AddOrEditAndSaveAsyn(bo);

                if (saveStatus)
                {
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
                    validateMessage.IsOK = false;
                    validateMessage.ValidateMessageItems.Add(
                        new ValidateMessageItem()
                        { IsPropertyName = false, MessageName = "RemoteErr", Message = "数据保存异常" });
                    return Json(validateMessage);
                }


            }
            else
            {
                //return View("../../Views/BusinessOrganization/BusinessBG/CreateOrEdit", boVM);
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
        /// <summary>
        /// 以局部页的方式的方式，构建明细数据的处理
        /// </summary>
        /// <param name="id">商品对象的ID属性值</param>
        public async Task<IActionResult> Detail(Guid id)
        {
            var bo = await _BoRepository.GetSingleAsyn(id, x => x.SK_WM_GoodsCategory);
            var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);
            var imgCount = 0;
            foreach (var img in images)
            {
                bo.GoodsIMG = img;
                imgCount++;
            }
            ViewBag.imgCount = imgCount;
            ViewBag.DetailImgs = images;
            var boVM = new SK_WM_GoodsVM(bo);
            ViewBag.UserLogonInformation = GetUserName();
            return PartialView("../../Views/BusinessOrganization/PublicView/_Detail", boVM);
        }

        /// <summary>
        /// 删除数据，将删除操作的结果加载到 DeleteStatus 对象，然后转成 json 数据返回给前端。
        /// </summary>
        /// <param name="id">待删除的对象 ID 属性值</param>
        /// <returns>将删除操作处理的结果转为 json 数据返回到前端，供前端根据情况处理</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            var status = new DeleteStatusModel();
            var image = await _ImageRepository.GetSingleAsyn(id);

            if (image == null)
            {
                var goods = await _BoRepository.FindByAsyn(x => x.ID == id);
                var shopcar = await _ShopCarGoodsItemRepository.FindByAsyn(x => x.GoodsName == goods.FirstOrDefault().Name);
                if (shopcar.FirstOrDefault() != null)
                {
                    foreach (var sc in shopcar)
                    {
                        await _ShopCarGoodsItemRepository.DeleteAndSaveAsyn(sc.ID);
                    }

                }

                status = await _BoRepository.DeleteAndSaveAsyn(id);

            }
            else
            {
                if (image.IsForTitle != true)
                {
                    status = await _ImageRepository.DeleteAndSaveAsyn(id);
                }
                else
                {
                    status.DeleteSatus = false;
                    status.Message = "该图片是封面，不可删除";
                }
            }

            return Json(status);
        }


        public async Task<IActionResult> ImagesEdit(Guid id)
        {
            var boCollection = await _ImageRepository.GetAllAsyn();

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
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/BusinessBG/ImagesEdit", boVMCollection);
        }


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



                    await _ImageRepository.AddOrEditAndSaveAsyn(businessIamge);

                }
                return Json(new { isOK = true, fileCount = files.Count, size = size, serverPath = serverPath });
            }
        }



        [HttpPost]
        public async Task<IActionResult> SaveCover(Guid ID)
        {
            var image = await _ImageRepository.GetSingleAsyn(ID);
            var goods = await _BoRepository.GetSingleAsyn(image.RelevanceObjectID);
            var images = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == image.RelevanceObjectID);
            foreach (var img in images)
            {
                img.IsForTitle = false;
                await _ImageRepository.AddOrEditAndSaveAsyn(img);
            }
            goods.GoodsIMG = image;
            await _BoRepository.AddOrEditAndSaveAsyn(goods);
            image.IsForTitle = true;
            var saveStatus = await _ImageRepository.AddOrEditAndSaveAsyn(image);
            if (saveStatus == true)
            {
                return Json(new { isOK = true });
            }
            else
            {
                return Json(new { isOK = false });
            }

        }

        public async Task<IActionResult> OrderList(string selectNnm)
        {
            ViewBag.UserLogonInformation = GetUserName();
            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = "Order",            // 对应的归属类型ID
                PageIndex = 1,                     // 当前页码
                PageSize = 4,                     // 每页数据条数 为"0"时显示所有
                PageAmount = 0,                    // 相关对象列表分页处理分页数量
                ObjectAmount = 0,                  // 相关的对象的总数
                Keyword = "",                      // 当前的关键词
                SortProperty = "SortCode",         // 排序属性
                SortDesc = "default",              // 排序方向，缺省值正向 Default，前端用开关方式转为逆向：Descend
                SelectedObjectID = "",   // 当前页面处理中处理的焦点对象 ID
                IsSearch = false,                  // 当前是否为检索
            };
            listPagePara.PageIndex = selectNnm != null ? Convert.ToInt32(selectNnm) : 1;
            var user = User.Claims.FirstOrDefault();
            if (user.Value == new Guid().ToString())
            {
                return View("../../Views/Home/Logon");
            }
            var shop = _ShopRepository.GetSingleBy(x => x.ShopForUser.Id == user.Value);
            //查询订单详细所关联的商品列表
            var orderItems = _OrderItemRepository.FindBy(x => x.ShopName == shop.Name);//获取订单和订单对应的用户和商品
            var orVMCollection = new List<SK_WM_OrderItemVM>();//存放用户所有订单

            var OrderAll = orderItems.AsQueryable<SK_WM_OrderItem>();
            var OrderCollectionPageList = IQueryableExtensions.ToPaginatedList(OrderAll, listPagePara.PageIndex, listPagePara.PageSize);
            foreach (var order in OrderCollectionPageList)
            {
                var omVM = new SK_WM_OrderItemVM(order);
                orVMCollection.Add(omVM);
            }

            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_OrderItem>(OrderCollectionPageList, 3, listPagePara.PageIndex);

            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;

            return View("../../Views/BusinessOrganization/BusinessBG/OrderList", orVMCollection);
        }

       
    }
}