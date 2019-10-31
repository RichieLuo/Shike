using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.ViewModels.ApplicationOrganization;
using Microsoft.AspNetCore.Identity;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.DataAccess;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.ViewModels.BusinessOrganization;
using ShiKe.Web.Controllers.ApplicationOrganization;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Entities.Attachments;
using ShiKe.Common.JsonModels;
using ShiKe.Entities.WebSettingManagement;

namespace ShiKe.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleManager<ApplicationRole> _RoleManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private readonly IEntityRepository<SK_WM_Goods> _BoRepository;
        private readonly IEntityRepository<BusinessImage> _ImageRepository;
        private readonly IEntityRepository<SK_WM_ShopCar> _ShopCarRepository;
        private readonly IEntityRepository<SK_WM_ShopCarGoodsItem> _ShopCarGoodsItemRepository;
        private readonly IEntityRepository<SK_WM_Order> _OrderRepository;
        private readonly IEntityRepository<SK_WM_OrderItem> _OrderItemRepository;
        private readonly IEntityRepository<SK_WM_Goods> _GoodsRepository;
        private readonly IEntityRepository<SK_WM_Shop> _ShopRepository;
        private readonly IEntityRepository<SK_WM_GoodsCategory> _CateRepository;
        private readonly IEntityRepository<SK_WM_GoodsCollection> _GoodsCollection;
        private readonly IEntityRepository<SK_SiteSettings> _SiteSettingsRepository;




        public HomeController(RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEntityRepository<SK_WM_Goods> repository,
            IEntityRepository<BusinessImage> imageRepository,
            IEntityRepository<SK_WM_ShopCar> shopCarRepository,
            IEntityRepository<SK_WM_ShopCarGoodsItem> shopCarGoodsItemRepository,
            IEntityRepository<SK_WM_Order> orderRepository,
            IEntityRepository<SK_WM_OrderItem> orderItemRepository,
            IEntityRepository<SK_WM_Goods> goodsRepository,
            IEntityRepository<SK_WM_Shop> shopRepository,
            IEntityRepository<SK_WM_GoodsCategory> cateRepository,
            IEntityRepository<SK_WM_GoodsCollection> goodsCollection,
               IEntityRepository<SK_SiteSettings> siteSettings)
            
        {
            _ShopCarRepository = shopCarRepository;
            _ShopCarGoodsItemRepository = shopCarGoodsItemRepository;
            _SignInManager = signInManager;
            _RoleManager = roleManager;
            _UserManager = userManager;
            _BoRepository = repository;
            _ImageRepository = imageRepository;
            _ShopRepository = shopRepository;
            _GoodsRepository = goodsRepository;
            _OrderRepository = orderRepository;
            _OrderItemRepository = orderItemRepository;
            _CateRepository = cateRepository;
            _GoodsCollection = goodsCollection;
            _SiteSettingsRepository = siteSettings;
        }

        public SK_SiteSettings SiteSettingsDetail()
        {
         
            var siteSettings = _SiteSettingsRepository.GetAllIncluding(x => x.Logo);
            var siteSetting = siteSettings.FirstOrDefault();
            if (siteSetting.Logo == null)
            {
                siteSetting.Logo = null;
            }
            return siteSetting;

           
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

        /// <summary>
        ///  判断用户是否已经拥有店铺
        /// </summary>
        public async void HasShopForCurrUser()
        {

            var userName = User.Identity.Name;
            if (userName == null)
            {
                ViewBag.CurrUserHasShop = false;
            }
            else
            {
                var user = await _UserManager.FindByNameAsync(userName);
                var shop = await _ShopRepository.FindByAsyn(x => x.BelongToUserID == user.Id);
                var has = shop.Count() == 0 ? ViewBag.CurrUserHasShop = false : ViewBag.CurrUserHasShop = true;
            }

        }

        /// <summary>
        /// 热门搜索(按销量排序后在视图取前6条)
        /// </summary>
        public void GetGoodsForSearchHot()
        {

            ViewBag.HotSearch = _GoodsRepository.GetAll().OrderByDescending(x => x.SalesVolume);
        }
        public async Task<IActionResult> Index()
        {
            var pageIndex = 1;
            var pageSize = 24;

            var boCollection = await _BoRepository.PaginateAsyn(pageIndex, pageSize, x => x.SortCode, null, x => x.SK_WM_GoodsCategory);
            var boVMCollection = new List<SK_WM_GoodsVM>();
            var goodsCategory = _CateRepository.GetAll();
            var orderNumber = 0;
            foreach (var bo in boCollection)
            {
                var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);

                foreach (var image in images)
                {
                    if (image.IsForTitle == true)
                    {
                        bo.GoodsIMG = image;
                    }
                }

                var boVM = new SK_WM_GoodsVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                boVMCollection.Add(boVM);
            }

            //获取用户的姓名
            ViewBag.UserLogonInformation = GetUserName();
            //获取网站标题
            ViewBag.SiteTitle = SiteSettingsDetail().Name;

            GetGoodsForSearchHot();
            string[] colorStart = new string[] { "#6bdea7", "#ff9229", "#b474fe", "#ff90a0" };
            string[] colorEnd = new string[] { "#68937f", "#d17b28", "#594e90", "#cf7986" };
            //ABOS_Admin();
            ViewBag.colorStart = colorStart;
            ViewBag.colorEnd = colorEnd;

            //分类查询
            //小吃快餐
            var snVMCollection = new List<SK_WM_GoodsVM>();
            var snCollection = boCollection.Where(x => x.SK_WM_GoodsCategory.Name == "小吃快餐").OrderByDescending(x => x.SalesVolume);
            foreach (var bo in snCollection)
            {
                var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);

                foreach (var image in images)
                {
                    if (image.IsForTitle == true)
                    {
                        bo.GoodsIMG = image;
                    }
                }

                var boVM = new SK_WM_GoodsVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                snVMCollection.Add(boVM);
            }
            ViewBag.snackFood = snVMCollection;

            //烧烤烤肉           
            var baVMCollection = new List<SK_WM_GoodsVM>();
            var baCollection = boCollection.Where(x => x.SK_WM_GoodsCategory.Name == "烧烤烤肉").OrderByDescending(x => x.SalesVolume);
            foreach (var bo in baCollection)
            {
                var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);

                foreach (var image in images)
                {
                    if (image.IsForTitle == true)
                    {
                        bo.GoodsIMG = image;
                    }
                }

                var boVM = new SK_WM_GoodsVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                baVMCollection.Add(boVM);
            }
            ViewBag.barbecue = baVMCollection;

            //甜点饮品
            var dsVMCollection = new List<SK_WM_GoodsVM>();
            var dsCollection = boCollection.Where(x => x.SK_WM_GoodsCategory.Name == "甜点饮品").OrderByDescending(x => x.SalesVolume);
            foreach (var bo in dsCollection)
            {
                var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);

                foreach (var image in images)
                {
                    if (image.IsForTitle == true)
                    {
                        bo.GoodsIMG = image;
                    }
                }

                var boVM = new SK_WM_GoodsVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                dsVMCollection.Add(boVM);
            }
            ViewBag.dessert = dsVMCollection;
            //其他美食
            var otVMCollection = new List<SK_WM_GoodsVM>();
            var otCollection = boCollection.Where(x => x.SK_WM_GoodsCategory.Name == "其他美食").OrderByDescending(x => x.SalesVolume);
            foreach (var bo in otCollection)
            {
                var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);

                foreach (var image in images)
                {
                    if (image.IsForTitle == true)
                    {
                        bo.GoodsIMG = image;
                    }
                }

                var boVM = new SK_WM_GoodsVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                otVMCollection.Add(boVM);
            }
            ViewBag.otherDelicacies = otVMCollection;


            ViewBag.categorys = goodsCategory;
            return View("Index", boVMCollection);

        }


        public async Task<IActionResult> ComDetail(Guid id)
        {
            ViewBag.UserLogonInformation = GetUserName();

            var goods = await _BoRepository.GetAllIncludingAsyn(x => x.Shop, x => x.ShopForUser, x => x.SK_WM_GoodsCategory, x => x.GoodsIMG);
            var bo = goods.Where(x => x.ID == id).FirstOrDefault();
            var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);

            ViewBag.Imgs = images;
            var boVM = new SK_WM_GoodsVM(bo);
            return View("ComDetail", boVM);
        }
        public IActionResult Logon()
        {
            return View();
        }

        public IActionResult Register()
        {
            bool isNew = true;
            var user = new ApplicationUser();
            var boVM = new ApplicationUserVM(user)
            {
                IsNew = isNew
            };

            #region 处理用户归属的用户组的数据
            #region 1.待选择的用户组数据
            //var role = _RoleManager.Roles;
            //var roleItems = new List<PlainFacadeItem>();
            //foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
            //{
            //    var rItem = new PlainFacadeItem() { ID = item.Id, Name = item.Name, DisplayName = item.DisplayName, SortCode = item.SortCode };
            //    roleItems.Add(rItem);
            //}
            //boVM.RoleItemColection = roleItems;
            //#endregion
            //#region 2.已经归属的用户组部分
            //boVM.RoleItemIDCollection = new List<string>();
            //foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
            //{
            //    var h = await _UserManager.IsInRoleAsync(user, item.Name);
            //    if (h)
            //    {
            //        boVM.RoleItemIDCollection.Add(item.Id);
            //    }
            //}
            #endregion
            //ViewBag.RoleID = null;
            #endregion

            return View("Register", boVM);
        }

        public async Task<IActionResult> ShoppingCar()
        {
            var username = User.Identity.Name;
            if (username == null)
            {
                return View("../../Views/Home/Logon");
            }
            var user = await _UserManager.FindByNameAsync(username);
            if (user == null)
            {
                return View("../../Views/Home/Logon");
            }
            var shopCar = await _ShopCarRepository.FindByAsyn(x => x.BelongToUserID == user.Id);
            var shopCarItem = await _ShopCarGoodsItemRepository.FindByAsyn(x => x.BelongToShopCarID == shopCar.FirstOrDefault().ID.ToString());

            var shopDemo = from a in shopCarItem
                           group a by a.ShopName into g
                           select new { aKey = g.Key, Items = g };
            ViewBag.GoodsShop = shopCarItem;

            var shopCarItemVM = new List<SK_WM_ShopCarGoodsItemVM>();
            foreach (var item in shopCarItem)
            {
                var sitem = new SK_WM_ShopCarGoodsItemVM(item);
                shopCarItemVM.Add(sitem);
            }


            //var shop = _ShopRepository.GetSingleBy(x => x.ID == Guid.Parse(goods.BelongToShopID));



            ViewBag.UserLogonInformation = GetUserName();

            return View("../Home/ShoppingCar", shopCarItemVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddToShoppingCar(SK_WM_ShopCarGoodsItem scItem)
        {
            try
            {
                //var status = false;
                var username = User.Identity.Name;
                if (username == null)
                {
                    //return View("../../Views/Home/Logon");
                    Json(new { isOK = false, message = "请登录后再执行操作" });
                }
                var user = await _UserManager.FindByNameAsync(username);
                if (user == null)
                {
                    //return View("../../Views/Home/Logon");
                    Json(new { isOK = false, message = "请登录后再执行操作" });
                }

                var shopcar = await _ShopCarRepository.FindByAsyn(x => x.BelongToUserID == user.Id);
                if (shopcar.Count() == 0)
                {
                    var shopCar = new SK_WM_ShopCar()
                    {
                        BelongToUserID = user.Id,
                        ShopCarForUser = user
                    };
                    await _ShopCarRepository.AddOrEditAndSaveAsyn(shopCar);
                    shopcar = await _ShopCarRepository.FindByAsyn(x => x.BelongToUserID == user.Id);
                }

                var goods = _GoodsRepository.GetSingleBy(x => x.ID == Guid.Parse(scItem.GoodsID));

                var imgs = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == goods.ID);
                var img = imgs.Where(x => x.IsForTitle == true).FirstOrDefault();

                var shop = _ShopRepository.GetSingleBy(x => x.ID == Guid.Parse(goods.BelongToShopID));

                //查询该用户的购物车下的所有商品
                var goodsItem = await _ShopCarGoodsItemRepository.FindByAsyn(x => x.BelongToShopCarID == shopcar.FirstOrDefault().ID.ToString());

                //查询该用户购物车内的商品内是否有正要添加的商品
                var hasGoods = goodsItem.Where(x => x.GoodsName == scItem.GoodsName);

                var sItem = new SK_WM_ShopCarGoodsItem();
                if (hasGoods.Count() == 0)//判断原购物车是否有要添加的商品、如果有就在原来的基础上修改数量及总价、否则直接添加新的
                {
                    sItem = new SK_WM_ShopCarGoodsItem()
                    {
                        BelongToShopCarID = shopcar.FirstOrDefault().ID.ToString(),
                        shopCar = shopcar.FirstOrDefault(),
                        GoodsID = scItem.GoodsID,
                        Count = scItem.Count,
                        Price = scItem.Price,
                        TotalPrice = (scItem.Count * decimal.Parse(scItem.Price)).ToString(),
                        ShopName = shop.Name,
                        GoodsName = goods.Name,
                        ImgPath = img.UploadPath,
                        CreateOrderTime = DateTime.Now,
                    };
                    _ShopCarGoodsItemRepository.AddOrEditAndSave(sItem);
                }
                else
                {
                    var hItem = hasGoods.FirstOrDefault();
                    hItem.Count = scItem.Count + hItem.Count;
                    hItem.TotalPrice = (hItem.Count * decimal.Parse(scItem.Price)).ToString();
                    _ShopCarGoodsItemRepository.AddOrEditAndSave(hItem);
                }


                return Json(new { isOK = true, message = "添加购物车成功" });

            }
            catch (Exception)
            {

                return Json(new { isOK = false, message = "添加失败，请检查登录状态后再执行操作" });
            }

        }

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddGoodsCollection(string id)
        {
            var goodsCollection = _GoodsCollection.GetSingleBy(x => x.Goods.ID == Guid.Parse(id));
            if (goodsCollection != null)
            {
                return Json(new { isOK = true, message = "您已添加有收藏" });
            }
            var GoodsData = await _GoodsRepository.GetSingleAsyn(Guid.Parse(id));
            var user = User.Claims.FirstOrDefault();
            var userData = await _UserManager.FindByIdAsync(user.Value);
            
            if (userData == null)
            {
                return Json(new { isOK = false, message = "请先登录" });
            }
            if (userData != null)
            {
                var collection = new SK_WM_GoodsCollection()
                {
                    ApplicationUser = userData,
                    Goods = GoodsData,
                };
                await _GoodsCollection.AddOrEditAndSaveAsyn(collection);
                return Json(new { isOK = true, message = "收藏成功" });
            }
            return Json(new { isOK = false, message = "请先登录" });
        }
        public async Task<IActionResult> DeleteShopCar(Guid id)
        {

            var username = User.Identity.Name;
            if (username == null)
            {
                //return View("../../Views/Home/Logon");
                Json(new { isOK = false, isLogin = false, message = "请登录后再执行操作" });
            }
            var user = await _UserManager.FindByNameAsync(username);
            if (user == null)
            {
                //return View("../../Views/Home/Logon");
                Json(new { isOK = false, isLogin = false, message = "请登录后再执行操作" });
            }

            var status = await _ShopCarGoodsItemRepository.DeleteAndSaveAsyn(id);

            return Json(status);
        }
        /// <summary>
        /// 订单确认页面
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OrderSure(List<SK_WM_OrderItem> orderItems, string orders)
        {
            //获取用户的姓名
            ViewBag.UserLogonInformation = GetUserName();
            if (GetUserName() == null)
            {
                return View("../../Views/Home/Logon");
            }
            //var username = User.Identity.Name;
            //var user = await _UserManager.FindByNameAsync(username);
            //if (user == null)
            //{
            //    return View("../../Views/Home/Logon");
            //}

            if (orders != null)
            {
                orderItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SK_WM_OrderItem>>(orders);
            }
            else
            {
                orderItems = null;
                return View();
            }
            //if (order != null)
            //{
            //    orderItems.Add(order);
            //}
            ViewBag.GoodsShop = orderItems;

            var orderItemVM = new List<SK_WM_OrderItemVM>();
            foreach (var item in orderItems)
            {
                var goods = _GoodsRepository.GetSingleBy(x => x.ID == Guid.Parse(item.GoodsID));
                var imgs = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == goods.ID);
                var img = imgs.Where(x => x.IsForTitle == true).FirstOrDefault();
                var shop = _ShopRepository.GetSingleBy(x => x.ID == Guid.Parse(goods.BelongToShopID));


                item.GoodsID = goods.ID.ToString();
                item.GoodsName = goods.Name;
                item.Description = goods.Description;
                item.ShopName = shop.Name;
                item.ImgPath = img.UploadPath;
                item.Price = goods.Price;
                item.TotalPrice = (Convert.ToDecimal(goods.Price) * item.Count).ToString();

                var orderItem = new SK_WM_OrderItemVM(item);
                orderItemVM.Add(orderItem);
            }

            return View("../Home/OrderSure", orderItemVM);
            //return Json(new { isOK = true, Orderitem = orderItemVM });
        }

        /// <summary>
        /// 确认下单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AddOrderSure(List<SK_WM_OrderItem> orderItem, string orders, string ordersID)
        {
            //状态 成功与否
            var status = false;
            var order = await _OrderItemRepository.GetAllIncludingAsyn(x => x.ItemForOrder);
            if (orders != null)
            {
                orderItem = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SK_WM_OrderItem>>(orders);
            }
            if (ordersID != null)
            {

                orderItem = order.Where(x => x.ItemForOrder.ID.ToString() == ordersID).ToList();
            }

            //获取当前登录的用户信息
            var userName = User.Identity.Name;
            if (userName == null)
            {
                return View("../../Views/Home/Logon");
            }
            var user = await _UserManager.FindByNameAsync(userName);

            #region 暂时保留不用的代码
            //var order = await _OrderRepository.FindByAsyn(x => x.BelongToUserID == user.Id);
            //if (order.Count() == 0)
            //{
            //    var orderNew = new SK_WM_Order()
            //    {
            //        BelongToUserID = user.Id,
            //        OrderForUser = user,
            //    };
            //    //创建一个订单容器
            //    await _OrderRepository.AddOrEditAndSaveAsyn(orderNew);
            //    order = await _OrderRepository.FindByAsyn(x => x.BelongToUserID == user.Id);
            //}
            //foreach (var item in orderItem)
            //{
            //    //查询加入订单的商品信息
            //    var goods = _GoodsRepository.GetSingleBy(x => x.Name == item.GoodsName);
            //    var imgs = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == goods.ID);
            //    var img = imgs.Where(x => x.IsForTitle == true).FirstOrDefault();
            //    var shop = _ShopRepository.GetSingleBy(x => x.ID == Guid.Parse(goods.BelongToShopID));

            //    //查询该用户当前未完成的所有订单     
            //    var orderItemNo = await _OrderItemRepository.FindByAsyn(x => x.BelongToOrderID == order.FirstOrDefault().ID.ToString() && x.State == (SK_WM_OrderState.Orderstate.待创建).ToString());
            //    //查询该用户的订单内是否存在当前要添加的商品
            //    var hasGoods = orderItemNo.Where(x => x.GoodsID == item.GoodsID);
            //    if (hasGoods.Count() == 0)
            //    {
            //        hasGoods = null;
            //    }
            //    //创建一个订单项对象并实例化
            //    var oItem = new SK_WM_OrderItem();

            //    if (hasGoods == null)  //未完成的订单中不存在当前正添加的商品
            //    {
            //        oItem = new SK_WM_OrderItem()
            //        {
            //            BelongToOrderID = order.FirstOrDefault().ID.ToString(),
            //            ItemForOrder = order.FirstOrDefault(),
            //            CreateOrderTime = DateTime.Now,
            //            DeliveryAdderss = "", //用户暂时没有给配送地址字段
            //            DeliveryFee = 0.00m, //商品暂时没有给配送费字段
            //            ShopName = shop.Name,
            //            GoodsName = goods.Name,
            //            GoodsID = goods.ID.ToString(),
            //            Description = goods.Description,
            //            Price = goods.Price,
            //            Count = orderItem.Count,
            //            TotalPrice = (orderItem.Count * decimal.Parse(goods.Price)).ToString(),
            //            ImgPath = img.UploadPath,
            //            State = (SK_WM_OrderState.Orderstate.待创建).ToString()
            //        };
            //        //创建订单
            //        status = await _OrderItemRepository.AddOrEditAndSaveAsyn(oItem);
            //    }
            //    else //未完成的订单中存在当前正添加的商品
            //    {
            //        //若商品存在
            //        var orderItemEdit = hasGoods.FirstOrDefault();
            //        orderItemEdit.Count = orderItemEdit.Count + orderItem.Count;
            //        orderItemEdit.TotalPrice = (orderItemEdit.Count * decimal.Parse(goods.Price)).ToString();
            //        status = await _OrderItemRepository.AddOrEditAndSaveAsyn(orderItemEdit);
            //    }
            #endregion


            //创建一个订单容器
            var boVM = new List<SK_WM_OrderItemVM>();
            var goods = await _BoRepository.GetAllIncludingAsyn(x => x.Shop);         

            var orderNew = new SK_WM_Order()
            {
                OrderForUser = user,
                State = (SK_WM_OrderState.Orderstate.待付款).ToString(),
                Goods = null,
                Count = orderItem.FirstOrDefault().Count,
                Name = ""
            };
            if (ordersID != null)
            {
                orderNew.ID = Guid.Parse(ordersID);

            }
            else
            {
                await _OrderRepository.AddOrEditAndSaveAsyn(orderNew);
            }


            var TotalPriceForOrder = 0.00m;
            var orderItemCount = 0;
            
            var goodsNameStr = "";// 用于存放 取前3条商品的名称作为订单的名称
            var goodIDFrist = new SK_WM_Goods();//商品 用于取第一条订单列表项的商品图片作为封面
            var goodNameCount = 1; //用于判断
            var goodIDFristeCount = 1;//用于判断

            foreach (var item in orderItem)
            {
                //查询加入订单的商品信息 
                var currGoods = goods.Where(x => x.ID == Guid.Parse(item.GoodsID)).FirstOrDefault();
                if (goodIDFristeCount<2)
                {
                    goodIDFrist = currGoods;
                    goodIDFristeCount++;
                }
                if (currGoods != null)
                {
                    var shop = _ShopRepository.GetSingleBy(x => x.ID == currGoods.Shop.ID);
                    var imgs = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == currGoods.ID);
                    var img = imgs.Where(x => x.IsForTitle == true).FirstOrDefault();
                    var oItem = new SK_WM_OrderItem();
                    if (ordersID == null)
                    {

                        //订单名称
                        if (goodNameCount<4)
                        {                          
                            goodsNameStr += currGoods.Name + "、";
                            goodNameCount++;
                        }

                        //创建一个订单项对象
                        oItem = new SK_WM_OrderItem()
                        {
                            ItemForOrder = orderNew,
                            CreateOrderTime = DateTime.Now,
                            DeliveryAdderss = "", //用户暂时没有给配送地址字段
                            DeliveryFee = 0.00m, //商品暂时没有给配送费字段
                            ShopName = shop.Name,
                            GoodsName = currGoods.Name,
                            GoodsID = currGoods.ID.ToString(),
                            Description = currGoods.Description,
                            Price = currGoods.Price,
                            Count = item.Count,
                            ImgPath = img.UploadPath,
                            TotalPrice = (item.Count * decimal.Parse(currGoods.Price)).ToString(),
                            State = (SK_WM_OrderState.Orderstate.待付款).ToString()
                        };
                    }
                    else
                    {
                        oItem = item;
                    }
                    var oItemVM = new SK_WM_OrderItemVM(oItem);
                    //创建订单
                    if (ordersID != null)
                    {
                        status = true;
                    }
                    else
                    {
                        status = await _OrderItemRepository.AddOrEditAndSaveAsyn(oItem);
                    }
                    //订单的总价格计算
                    TotalPriceForOrder += decimal.Parse(oItem.TotalPrice);
                    orderItemCount++;
                    boVM.Add(oItemVM);
                }
            }
            ViewBag.UserLogonInformation = GetUserName();
            if (status)
            {
                orderNew.Name = goodsNameStr;//重新修改订单的名称
                orderNew.Goods = goodIDFrist;//
                orderNew.CreateOrderTime = DateTime.Now;
                orderNew.Count = orderItemCount;
                orderNew.TotalPrice = TotalPriceForOrder;
                orderNew.State = (SK_WM_OrderState.Orderstate.待付款).ToString();
                if (ordersID == null)
                {
                    await _OrderRepository.AddOrEditAndSaveAsyn(orderNew);
                }
                return View("../Home/CheckStand", boVM);
            }
            else
            {
                return Json(new { isOK = false });
            }
        }

        public async Task<IActionResult> Payment(string info)
        {
            var pay = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment>>(info);
            var payVM = pay.FirstOrDefault();
            var userName = User.Identity.Name;
            if (userName == null)
            {
                return View("../../Views/Home/Logon");
            }
            var user = await _UserManager.FindByNameAsync(userName);
            var result = await _SignInManager.PasswordSignInAsync(user.UserName, payVM.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var allOrderItem = await _OrderItemRepository.GetAllIncludingAsyn(x => x.ItemForOrder);

                var orderItem = allOrderItem.Where(x => x.ItemForOrder.ID.ToString() == payVM.OrderID);
                var orders = await _OrderRepository.FindByAsyn(x => x.ID.ToString() == payVM.OrderID);
                foreach (var item in orderItem)
                {
                    item.State = (SK_WM_OrderState.Orderstate.待发货).ToString();
                    _OrderItemRepository.AddOrEditAndSave(item);
                }
                if (orders.FirstOrDefault() != null)
                {
                    var order = orders.FirstOrDefault();
                    order.State = (SK_WM_OrderState.Orderstate.待发货).ToString();
                    _OrderRepository.AddOrEditAndSave(order);
                }

                return Json(new { result = true, messsage = "支付成功" });
            }
            else
            {
                return Json(new { result = false, message = "支付密码错误，请检查后重新处理。" });
            }
        }
        public async Task<IActionResult> MyOrder()
        {
            var userName = User.Identity.Name;
            if (userName == null)
            {
                return View("../../Views/Home/Logon");
            }
            var user = await _UserManager.FindByNameAsync(userName);

            //查询订单详细所关联的商品列表
            var ordersItem = await _OrderItemRepository.GetAllIncludingAsyn(x => x.ItemForOrder, x => x.ItemForOrder.OrderForUser);
            var orders = new List<SK_WM_OrderItem>();

            foreach (var order in ordersItem)
            {
                //var OrderForUser = await _OrderRepository.GetAllIncludingAsyn(x => x.OrderForUser);
                //order.ItemForOrder = OrderForUser.Where(x => x.ID == order.ItemForOrder.ID).FirstOrDefault();
                orders.Add(order);
            }
            var BelongToUserOrders = orders.Where(x => x.ItemForOrder.OrderForUser.Id == user.Id);

            ViewBag.OrderItem = BelongToUserOrders;

            var orderItemVM = new List<SK_WM_OrderItemVM>();

            foreach (var item in BelongToUserOrders)
            {
                var sitem = new SK_WM_OrderItemVM(item);

                var goods = await _GoodsRepository.FindByAsyn(x => x.Name == item.GoodsName);
                var fGoods = goods.FirstOrDefault();
                if (fGoods == null)
                {
                    sitem.HasGoods = false;
                }

                orderItemVM.Add(sitem);
            }

            ViewBag.UserLogonInformation = GetUserName();

            return View("../Home/MyOrder", orderItemVM);
        }


        /// <summary>
        ///  订单确认页面
        /// </summary>
        /// <returns></returns>
        //public async Task<IActionResult> CheckStand(List<SK_WM_OrderItem> orderItems, string orders)
        //{
        //    ////获取用户的姓名
        //    //ViewBag.UserLogonInformation = GetUserName();

        //    //var username = User.Identity.Name;
        //    //var user = await _UserManager.FindByNameAsync(username);
        //    //if (user == null)
        //    //{
        //    //    return View("../../Views/Home/Logon");
        //    //}
        //    //if (orders != "" || orders != null)
        //    //{
        //    //    orderItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SK_WM_OrderItem>>(orders);
        //    //}
        //    //else
        //    //{
        //    //    orderItems = null;
        //    //    return View();
        //    //}

        //    //ViewBag.GoodsShop = orderItems;

        //    //var orderItemVM = new List<SK_WM_OrderItemVM>();
        //    //foreach (var item in orderItems)
        //    //{
        //    //    var goods = _GoodsRepository.GetSingleBy(x => x.ID == Guid.Parse(item.GoodsID));
        //    //    var imgs = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == goods.ID);
        //    //    var img = imgs.Where(x => x.IsForTitle == true).FirstOrDefault();
        //    //    var shop = _ShopRepository.GetSingleBy(x => x.ID == Guid.Parse(goods.BelongToShopID));


        //    //    item.GoodsID = goods.ID.ToString();
        //    //    item.ShopName = shop.Name;
        //    //    item.ImgPath = img.UploadPath;
        //    //    item.Price = goods.Price;
        //    //    item.TotalPrice = (Convert.ToDecimal(goods.Price) * item.Count).ToString();

        //    //    var orderItem = new SK_WM_OrderItemVM(item);
        //    //    orderItemVM.Add(orderItem);
        //    //}

        //    return View("../Home/OrderSure" /*orderItemVM*/);
        //}

        /// <summary>
        /// 店铺查询（客户访问）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Shop(Guid id)
        {
            //获取用户的姓名
            ViewBag.UserLogonInformation = GetUserName();

            //获取所有的店铺 根据当前用户的ID查询当对应的店铺信息
            var shops = await _ShopRepository.GetAllIncludingAsyn(x => x.ShopForUser, x => x.ShopAvatar, x => x.ShopBanner, x => x.ShopForExecuteIllegal);
            var shopForCurrUser = shops.Where(x => x.ID == id && x.ShopForExecuteIllegal.ShopState == (SK_WM_ShopState.ShopState.已开启).ToString());
            var currShopID = shopForCurrUser.FirstOrDefault().ID;
            ViewBag.CurrShop = shopForCurrUser.FirstOrDefault();

            //取出店铺对应的商品集合
            var goodsCollection = await _BoRepository.GetAllIncludingAsyn(x => x.Shop);
            var goodsCollectionForCurrShop = goodsCollection.Where(x => x.Shop.ID == currShopID);
            var goodsVMCollection = new List<SK_WM_GoodsVM>();

            var orderNumber = 0;
            foreach (var bo in goodsCollectionForCurrShop)
            {
                var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID).FirstOrDefault();
                bo.GoodsIMG = image;
                var boVM = new SK_WM_GoodsVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                goodsVMCollection.Add(boVM);
            }
            return View("Shop", goodsVMCollection);
        }

        /// <summary>
        /// 店铺查询（店家访问）
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ShopForBusiness()
        {
            if (GetUserName() == null)
            {
                return View("../../Views/Home/Logon");
            }
            //获取用户的姓名
            ViewBag.UserLogonInformation = GetUserName();

            //获取当前登录的用户信息
            var userName = User.Identity.Name;
            var user = await _UserManager.FindByNameAsync(userName);


            //获取所有的店铺 根据当前用户的ID查询当对应的店铺信息
            var shops = await _ShopRepository.GetAllIncludingAsyn(x => x.ShopForUser, x => x.ShopAvatar, x => x.ShopBanner, x => x.ShopForExecuteIllegal);

            //店铺被封禁 暂时返回主页（未做其他处理）
            //var isIllegal = shops.Where(x => x.ShopForUser.Id == user.Id).FirstOrDefault().ShopForExecuteIllegal.ShopState;
            //if (shops.Where(x => x.ShopForUser.Id == user.Id).FirstOrDefault().ShopForExecuteIllegal.ShopState == (SK_WM_ShopState.ShopState.已停封).ToString())
            //{
            //    return Redirect("../../Home/Index");
            //}
            var shopForCurrUser = shops.Where(x => x.ShopForUser.Id == user.Id && x.ShopForExecuteIllegal.ShopState == (SK_WM_ShopState.ShopState.已开启).ToString());

            //判断当前用户是否已经开了店铺（如果没有则跳转至申请入驻页面）     
            if (shopForCurrUser.Count() == 0)
            {
                return Redirect("../ShopManager/Settled");
            }
            var currShopID = shopForCurrUser.FirstOrDefault().ID;
            ViewBag.CurrShop = shopForCurrUser.FirstOrDefault();

            //取出店铺对应的商品集合
            var goodsCollection = await _BoRepository.GetAllIncludingAsyn(x => x.Shop);
            var goodsCollectionForCurrShop = goodsCollection.Where(x => x.Shop.ID == currShopID);
            var goodsVMCollection = new List<SK_WM_GoodsVM>();

            var orderNumber = 0;
            foreach (var bo in goodsCollectionForCurrShop)
            {
                var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID).FirstOrDefault();
                bo.GoodsIMG = image;
                var boVM = new SK_WM_GoodsVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                goodsVMCollection.Add(boVM);
            }
            return View("Shop", goodsVMCollection);
        }



        public IActionResult Error()
        {
            return View();
        }
        public IActionResult MyCollection()
        {
            ViewBag.UserLogonInformation = GetUserName();
            return View();
        }

        public IActionResult Feedback()
        {
            //获取用户的姓名
            ViewBag.UserLogonInformation = GetUserName();

            return View();
        }
        public IActionResult Unfinished()
        {
            ViewBag.UserLogonInformation = GetUserName();
            return View();
        }

    }
}
