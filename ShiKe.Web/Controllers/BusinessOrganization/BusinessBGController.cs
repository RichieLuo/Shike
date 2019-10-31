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
            if (type == "������")
            {
                orderItem = _OrderItemRepository.FindBy(x => x.State == type);
                orderItem = orderItem.Where(x => x.ShopName == shop);
                count = orderItem.Count().ToString();

            }
            else if (type == "������")
            {
                orderItem = _OrderItemRepository.FindBy(x => x.State == type);
                orderItem = orderItem.Where(x => x.ShopName == shop);
                count = orderItem.Count().ToString();

            }
            else if (type == "���ջ�")
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
                if (state.ToString() == "������" || state.ToString() == "������" || state.ToString() == "���ջ�")
                {
                    if (state.ToString() == "������")
                    {
                        boVM.waitPay = GetCount(state.ToString(), shop.FirstOrDefault().Name);
                    }
                    else if (state.ToString() == "������")
                    {
                        boVM.waitSend = GetCount(state.ToString(), shop.FirstOrDefault().Name);
                    }
                    else if (state.ToString() == "���ջ�")
                    {
                        boVM.waitReceipt = GetCount(state.ToString(), shop.FirstOrDefault().Name);
                    }
                }
            }
            ViewBag.UserLogonInformation = GetUserName();

            return View("../../Views/BusinessOrganization/BusinessBG/Index", boVM);
        }
        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <returns></returns>

        public async Task<IActionResult> ShopInformation()
        {
            int numorder = 0;//�洢��������
            int NumGoods = 0;//�洢��������
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
        /// ���ݹ�������
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GoodsList()
        {
            ViewBag.UserLogonInformation = GetUserName();

            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = "Goods",            // ��Ӧ�Ĺ�������ID
                PageIndex = 1,                     // ��ǰҳ��
                PageSize = 10,                     // ÿҳ�������� Ϊ"0"ʱ��ʾ����
                PageAmount = 0,                    // ��ض����б��ҳ�����ҳ����
                ObjectAmount = 0,                  // ��صĶ��������
                Keyword = "",                      // ��ǰ�Ĺؼ���
                SortProperty = "SortCode",         // ��������
                SortDesc = "default",              // ������ȱʡֵ���� Default��ǰ���ÿ��ط�ʽתΪ����Descend
                SelectedObjectID = "",   // ��ǰҳ�洦���д���Ľ������ ID
                IsSearch = false,                  // ��ǰ�Ƿ�Ϊ����
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
                ObjectTypeID = "Order",            // ��Ӧ�Ĺ�������ID
                PageIndex = 1,                     // ��ǰҳ��
                PageSize = 1,                     // ÿҳ�������� Ϊ"0"ʱ��ʾ����
                PageAmount = 0,                    // ��ض����б��ҳ�����ҳ����
                ObjectAmount = 0,                  // ��صĶ��������
                Keyword = "",                      // ��ǰ�Ĺؼ���
                SortProperty = "SortCode",         // ��������
                SortDesc = "default",              // ������ȱʡֵ���� Default��ǰ���ÿ��ط�ʽתΪ����Descend
                SelectedObjectID = "",   // ��ǰҳ�洦���д���Ľ������ ID
                IsSearch = false,                  // ��ǰ�Ƿ�Ϊ����
            };
            listPagePara.PageIndex = selectNnm != null ? Convert.ToInt32(selectNnm) : 1;
            var user = User.Claims.FirstOrDefault();
            if (user.Value == new Guid().ToString())
            {
                return View("../../Views/Home/Logon");
            }
            var shop = _ShopRepository.GetSingleBy(x => x.ShopForUser.Id == user.Value);
            //��ѯ������ϸ����������Ʒ�б�
            var GoodsItems = _GoodsRepository.FindBy(x => x.Shop.ID == shop.ID);//��ȡ�����Ͷ�����Ӧ���û�����Ʒ
            var orVMCollection = new List<SK_WM_GoodsVM>();//����û����ж���

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
        /// �̼ҵ�����Ϣ����
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
                ObjectTypeID = objectTypeID,           // ��Ӧ�Ĺ�������ID
                PageIndex = pageIndex,                 // ��ǰҳ��
                PageSize = 10,                  // ÿҳ�������� Ϊ"0"ʱ��ʾ����
                PageAmount = 0,                 // ��ض����б��ҳ�����ҳ����
                ObjectAmount = 0,             // ��صĶ��������
                Keyword = "",             // ��ǰ�Ĺؼ���
                SortProperty = "SortCode",         // ��������
                SortDesc = "default",            // ������ȱʡֵ���� Default��ǰ���ÿ��ط�ʽתΪ����Descend
                SelectedObjectID = "",     // ��ǰҳ�洦���д���Ľ������ ID
                IsSearch = false,             // ��ǰ�Ƿ�Ϊ����
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
                        { IsPropertyName = false, MessageName = "Name", Message = "��Ʒ���ظ�����һ�����Կ���" });
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
                        { IsPropertyName = false, MessageName = "Name", Message = "����Ʒ������ģ���û��Ȩ�ޱ༭" });
                        return Json(validateMessage);
                    }
                }

                // ����һ�����������
                boVM.MapToBo(bo);

                // �����������
                if (!String.IsNullOrEmpty(boVM.GoodsCategoryID))
                {
                    var skwmgclssID = Guid.Parse(boVM.GoodsCategoryID);
                    var skwmgclss = _BoRepository.EntitiesContext.SK_WM_GoodsCategory.FirstOrDefault(x => x.ID == skwmgclssID);
                    bo.SK_WM_GoodsCategory = skwmgclss;
                }

                var saveStatus = false;

                //�����ϴ��ļ�
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
                        { IsPropertyName = false, MessageName = "RemoteErr", Message = "���ݱ����쳣" });
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
        /// �Ծֲ�ҳ�ķ�ʽ�ķ�ʽ��������ϸ���ݵĴ���
        /// </summary>
        /// <param name="id">��Ʒ�����ID����ֵ</param>
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
        /// ɾ�����ݣ���ɾ�������Ľ�����ص� DeleteStatus ����Ȼ��ת�� json ���ݷ��ظ�ǰ�ˡ�
        /// </summary>
        /// <param name="id">��ɾ���Ķ��� ID ����ֵ</param>
        /// <returns>��ɾ����������Ľ��תΪ json ���ݷ��ص�ǰ�ˣ���ǰ�˸����������</returns>
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
                    status.Message = "��ͼƬ�Ƿ��棬����ɾ��";
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
                return Json(new { isOK = false, fileCount = 0, size = size, serverPath = "û��ѡ���κ��ļ�����ѡ���ļ������ύ�ϴ���" });
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
                ObjectTypeID = "Order",            // ��Ӧ�Ĺ�������ID
                PageIndex = 1,                     // ��ǰҳ��
                PageSize = 4,                     // ÿҳ�������� Ϊ"0"ʱ��ʾ����
                PageAmount = 0,                    // ��ض����б��ҳ�����ҳ����
                ObjectAmount = 0,                  // ��صĶ��������
                Keyword = "",                      // ��ǰ�Ĺؼ���
                SortProperty = "SortCode",         // ��������
                SortDesc = "default",              // ������ȱʡֵ���� Default��ǰ���ÿ��ط�ʽתΪ����Descend
                SelectedObjectID = "",   // ��ǰҳ�洦���д���Ľ������ ID
                IsSearch = false,                  // ��ǰ�Ƿ�Ϊ����
            };
            listPagePara.PageIndex = selectNnm != null ? Convert.ToInt32(selectNnm) : 1;
            var user = User.Claims.FirstOrDefault();
            if (user.Value == new Guid().ToString())
            {
                return View("../../Views/Home/Logon");
            }
            var shop = _ShopRepository.GetSingleBy(x => x.ShopForUser.Id == user.Value);
            //��ѯ������ϸ����������Ʒ�б�
            var orderItems = _OrderItemRepository.FindBy(x => x.ShopName == shop.Name);//��ȡ�����Ͷ�����Ӧ���û�����Ʒ
            var orVMCollection = new List<SK_WM_OrderItemVM>();//����û����ж���

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