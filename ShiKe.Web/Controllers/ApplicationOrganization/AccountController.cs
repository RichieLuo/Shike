using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.Common.JsonModels;
using ShiKe.Entities.ApplicationOrganization;
using Microsoft.AspNetCore.Identity;
using ShiKe.ViewModels.ApplicationOrganization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using ShiKe.Entities.Attachments;
using ShiKe.DataAccess;
using System.IO;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.ViewModels.BusinessOrganization;

namespace ShiKe.Web.Controllers.ApplicationOrganization
{
    public class AccountController : Controller
    {
        private readonly RoleManager<ApplicationRole> _RoleManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private readonly IEntityRepository<BusinessImage> _ImageRepository;
        private readonly IEntityRepository<SK_WM_Shop> _ShopRepository;
        private readonly IEntityRepository<SK_WM_Order> _SK_WM_Order;
        private readonly IEntityRepository<SK_WM_Goods> _SK_WM_Goods;
        private readonly IEntityRepository<SK_WM_ShopExecuteIllegal> _ShopEIllegalRepository;
        private IHostingEnvironment _HostingEnv;
        private readonly IEntityRepository<SK_WM_GoodsCollection> _GoodsCollection;
        private readonly IEntityRepository<SK_WM_ShopCar> _ShopCarRepository;
        private readonly IEntityRepository<SK_WM_ShopCarGoodsItem> _ShopCarGoodsItemRepository;

        public AccountController(
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnv,
            IEntityRepository<BusinessImage> imageRepository,
             IEntityRepository<SK_WM_ShopExecuteIllegal> shopExecuteIllegal,
            IEntityRepository<SK_WM_Shop> shopRepository,
            IEntityRepository<SK_WM_Order> sk_wm_order,
             IEntityRepository<SK_WM_GoodsCollection> goodsCollection,
             IEntityRepository<SK_WM_Goods> sk_wm_goods,
             IEntityRepository<SK_WM_ShopCar> shopCarRepository,
             IEntityRepository<SK_WM_ShopCarGoodsItem> shopCarGoodsItemRepository)
        {
            _RoleManager = roleManager;
            _SignInManager = signInManager;
            _UserManager = userManager;
            _HostingEnv = hostingEnv;
            _ImageRepository = imageRepository;
            _ShopRepository = shopRepository;
            _ShopEIllegalRepository = shopExecuteIllegal;
            _SK_WM_Order = sk_wm_order;
            _GoodsCollection = goodsCollection;
            _SK_WM_Goods = sk_wm_goods;
            _ShopCarRepository = shopCarRepository;
            _ShopCarGoodsItemRepository = shopCarGoodsItemRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Save([Bind("ID,IsNew,RoleItemIDCollection,UserName,Name,MobileNumber,EMail,Password,UserAddress,ConfirmPassword,Description")]ApplicationUserVM boVM)
        {
            var user = await _UserManager.FindByIdAsync(boVM.ID.ToString());
            #region 1.�û��������ϸ���
            //user.FirstName = "";
            user.ChineseFullName = boVM.ChineseFullName;
            user.UserName = boVM.UserName;
            user.Email = boVM.EMail;
            user.MobileNumber = boVM.MobileNumber;
            user.UserAddress = boVM.UserAddress;
            await _UserManager.UpdateAsync(user);
            #endregion
            return RedirectToAction("Personal");
        }


        /// <summary>
        /// �û���¼
        /// </summary>
        /// <param name="jsonLogonInformation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logon(string jsonLogonInformation)
        {
            var logonVM = Newtonsoft.Json.JsonConvert.DeserializeObject<LogonInformation>(jsonLogonInformation);
            var user = await _UserManager.FindByNameAsync(logonVM.UserName);
            if (user != null)
            {
                var result = await _SignInManager.PasswordSignInAsync(logonVM.UserName, logonVM.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // ����ĵ�¼�ɹ���ĵ���Ӧ�þ��������û����ڵĽ�ɫ�������д���ġ�
                    var returnUrl = Url.Action("Personal", "Account");
                    return Json(new { result = true, messsage = returnUrl });
                    //return RedirectToAction("../Account/EditProfile");
                }
                else
                {
                    return Json(new { result = false, message = "�û��������������������´���" });
                }
            }
            else
                return Json(new { result = false, message = "�޷�ִ�е�¼����������ϸ�������´���" });

        }

        public async Task<IActionResult> AddDemoUser()
        {
            //ȡ�����еĵ���
            var shops = await _ShopRepository.GetAllIncludingAsyn(x => x.ShopForUser);
            var shopForUserNull = shops.Where(x => x.ShopForUser == null).OrderByDescending(x=>x.Name);
            List<SK_WM_Shop> list = new List<SK_WM_Shop>();
            foreach (var item in shopForUserNull)
            {
                list.Add(item);
            }
            //���Ĭ���û�
            string[] userLastStr = new string[] { "", "a", "b", "c", "d" };
            var password = "123@Aa";
            for (int i = 0; i < userLastStr.Length+1; i++)
            {
                if (i == 0)
                { 
                    var user = new ApplicationUser()
                    {
                        UserName = "admin",
                        FirstName = "ʳ�̳���",
                        ChineseFullName = "��������Ա",
                        Email = "admin@shike.com",
                        MobileNumber = "15578806785",
                    };
                    var a1 = await _UserManager.CreateAsync(user);
                    var a2 = await _UserManager.AddPasswordAsync(user, password);


                    // ��������ӵ����Ȩ�޵��û�����

                    IdentityResult roleResult;
                    //�ж��û����Ƿ����
                    var roleExists = await _RoleManager.RoleExistsAsync("Admin"); 
                    if (!roleExists)
                    {
                        ApplicationRole newRole=  new ApplicationRole() { Name = "Admin", DisplayName = "ϵͳ������Ա", Description = "������ϵͳ������Ա", ApplicationRoleType = ApplicationRoleTypeEnum.������ϵͳ������Ա, SortCode = "69a5f56g" };
                        roleResult = await _RoleManager.CreateAsync(newRole);
                    }
                    //��ѯ�û��Ƿ��Ѿ������Ȩ�� ��������ӽ��û���
                    if (!await _UserManager.IsInRoleAsync(user,"Admin"))
                    {
                        var roleOK = await _UserManager.AddToRoleAsync(user, "Admin");
                    }

                    //var roleAverageUser =  _RoleManager.Roles;
                    //if (roleAverageUser != null)
                    //{
                    //    foreach (var item in roleAverageUser)
                    //    {
                    //        if (item.Name=="Admin")
                    //        {
                    //            await _UserManager.AddToRoleAsync(user, item.Name.ToString());
                    //        }                           
                    //    }                      
                    //}
                    //var roleAverageUser = await _RoleManager.FindByNameAsync("Admin");
                    //if (roleAverageUser != null)
                    //{                      
                    //    await _UserManager.AddToRoleAsync(user, roleAverageUser.Name); 
                    //}
                }
                else
                {                    
                    var user = new ApplicationUser()
                    {
                        UserName = "sk" + userLastStr[i - 1].ToString(),
                        FirstName = "ʳ��",
                        ChineseFullName = "ʾ���û�" + userLastStr[i - 1].ToString(),
                        Email = "0123456" + userLastStr[i - 1].ToString() + "@qq.com",
                        MobileNumber = "15578806785",
                    };                 
                    var a1 = await _UserManager.CreateAsync(user);
                    var a2 = await _UserManager.AddPasswordAsync(user, password);

                    //����ǰ�û�����δӵ�е����ĵ���
                    list[i - 1].ShopForUser = user;
                    list[i - 1].BelongToUserID = user.Id;                
                    await _ShopRepository.AddOrEditAndSaveAsyn(list[i - 1]);
                }

            }
            return Redirect("../../Home/Logon");
        }

        /// <summary>
        /// ��ͨ�û�����ע�����
        /// </summary>
        /// <param name="jsonRegisetrInformation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register([Bind("ID,IsNew,RoleItemIDCollection,UserName,Name,MobileNumber,EMail,Password,ConfirmPassword,Description")]ApplicationUserVM boVM)
        {
            var validateMessage = new ValidateMessage();

            if (ModelState.IsValid)
            {
                // ������� 
                var isNewUser = await _UserManager.FindByNameAsync(boVM.UserName);


                if (_HasTheSameUser(boVM.UserName))
                {
                    validateMessage.IsOK = false;
                    validateMessage.ValidateMessageItems.Add(
                        new ValidateMessageItem()
                        {
                            IsPropertyName = false,
                            MessageName = "UserName",
                            Message = "�û�ѡ����û����Ѿ���ʹ����"
                        });
                    return Json(validateMessage);
                }


                var user = new ApplicationUser(boVM.UserName)
                {
                    FirstName = "",
                    ChineseFullName = boVM.Name,
                    Email = boVM.EMail,
                    MobileNumber = boVM.MobileNumber
                };

                var a1 = await _UserManager.CreateAsync(user);
                var a2 = await _UserManager.AddPasswordAsync(user, boVM.Password);        // �������  
                                                                                          //var roleItem = await _RoleManager.FindByNameAsync("Admin");

                //if (roleItem != null)
                //{
                //    await _UserManager.AddToRoleAsync(user, roleItem.Name);
                //}
                //var a3 = await _UserManager.AddToRoleAsync(user, "AverageUser");    // ���뵽 AverageUser ��ɫ��

                var roleAverageUser = await _RoleManager.FindByNameAsync("AverageUser");

                if (roleAverageUser != null)
                {
                    await _UserManager.AddToRoleAsync(user, roleAverageUser.Name);  // ���û���ӵ���Ӧ���û�����
                }


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

            #region ���ڵĴ���
            //var userVM = Newtonsoft.Json.JsonConvert.DeserializeObject<ApplicationUserVM>(jsonRegisterInformation);
            //var user = await _UserManager.FindByNameAsync(userVM.UserName);
            //if (user == null)
            //{
            //    user = new ApplicationUser(userVM.UserName);
            //    user.FirstName = "";
            //    user.ChineseFullName = userVM.Name;
            //    user.Email = userVM.EMail;
            //    user.MobileNumber = userVM.MobileNumber;
            //    var a1 = await _UserManager.CreateAsync(user);
            //    var a2 = await _UserManager.AddPasswordAsync(user, userVM.Password);        // �������
            //    /*var a3 = await _UserManager.AddToRoleAsync(user, "AverageUser");  */  // ���뵽 AverageUser ��ɫ��
            //    //var a3 = await _UserManager.AddToRoleAsync(user, "RegisterReadersRole");    // ���뵽 RegisterReadersRole ��ɫ��
            //    return Json(new { result = true, message = "ע��ɹ�����ϲ����Ϊʳ���е�һԱ��ף��������죡" });
            //}
            //else
            //{
            //    return Json(new { result = false, message = "�Ѿ�������ͬ�û������û���������ѡ��һ���û�����" });
            //}

            #endregion

        }
        /// <summary>
        /// �����Ѿ��û��Ѿ���¼��������ж��Ƿ��Ѿ����Ի�ȡ�û��ĵ�¼��Ϣ
        /// </summary>
        /// <returns>�û���</returns>
        [Authorize]
        public IActionResult GetUserName()
        {
            var userName = User.Identity.Name;
            return Json(new { UserName = userName });
        }


        /// <summary>
        /// �༭�û�����
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Personal()
        {
            var userName = User.Identity.Name;
            var user = await _UserManager.FindByNameAsync(userName);
            ViewBag.UserLogonInformation = "";

            var userVM = new ApplicationUserVM(user);
            if (user != null)
            {
                ViewBag.UserLogonInformation = userVM.Name;
            }

            var uID = Guid.Parse(user.Id);
            var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == uID).FirstOrDefault();
            if (image != null)
                userVM.AvatarPath = image.UploadPath;

            return View("../../Views/Account/UserChildren/Personal", userVM);
            //var userName = User.Identity.Name;
            //var user = await _UserManager.FindByNameAsync(userName);
            //ViewBag.UserLogonInformation = "";
            //if (user != null)
            //{
            //    ViewBag.UserLogonInformation = userName;
            //}
            //return View(new ApplicationUserVM(user));
        }

        public IActionResult Safety()//��ȫ����
        {
            return View("../../Views/Account/UserChildren/Safety");
        }


        public IActionResult Addressmt()//��ַ����
        {
            return View("../../Views/Account/UserChildren/Addressmt");
        }

        public async Task<IActionResult> Collection()//�ղ�
        {
            var userValue = User.Claims.FirstOrDefault();
            //var userdata = await _UserManager.FindByIdAsync(userValue.Value);
            var GoodsCollections = _GoodsCollection.GetAllMultiCondition(x => x.ApplicationUser.Id == userValue.Value, y => y.Goods);
            var GoodsCollectionList = new List<SK_WM_GoodsCollection>();
            foreach (var GoodsCollection in GoodsCollections)
            {
                var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == GoodsCollection.Goods.ID);
                foreach (var image in images)
                {
                    if (image.IsForTitle == true)
                    {
                        GoodsCollection.Goods.GoodsIMG = image;
                    }
                }
                GoodsCollectionList.Add(GoodsCollection);
            }
            return View("../../Views/Account/UserChildren/Collection", GoodsCollectionList);
        }

        public async Task<IActionResult> DeleteCollection(Guid id)//�ղ�
        {
            await _GoodsCollection.DeleteAndSaveAsyn(id);
            return RedirectToAction("Collection");
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> SavaShoppingCart(Guid id)
        {
            try
            {
                var userValue = User.Claims.FirstOrDefault();
                var user = await _UserManager.FindByIdAsync(userValue.Value.ToString());
                if (user == null)
                {
                    //return View("../../Views/Home/Logon");
                    Json(new { isOK = false, message = "���¼����ִ�в���" });
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

                var goods = await _SK_WM_Goods.GetSingleAsyn(id, x => x.Shop, y => y.ShopForUser);
                var imgs = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == goods.ID);
                var img = imgs.Where(x => x.IsForTitle == true).FirstOrDefault();
                //��ѯ���û��Ĺ��ﳵ�µ�������Ʒ
                var goodsItem = await _ShopCarGoodsItemRepository.FindByAsyn(x => x.BelongToShopCarID == shopcar.FirstOrDefault().ID.ToString());
                //��ѯ���û����ﳵ�ڵ���Ʒ���Ƿ�����Ҫ��ӵ���Ʒ
                var hasGoods = goodsItem.Where(x => x.GoodsName == goods.Name);
                if (hasGoods.Count() == 0)//�ж�ԭ���ﳵ�Ƿ���Ҫ��ӵ���Ʒ������о���ԭ���Ļ������޸��������ܼۡ�����ֱ������µ�
                {
                    var ShopCarGoodsItem = new SK_WM_ShopCarGoodsItem()
                    {
                        ShopName = goods.Shop.Name,
                        GoodsName = goods.Name,
                        Count = 1,
                        Price = goods.Price,
                        TotalPrice = goods.Price,
                        BelongToShopCarID = shopcar.FirstOrDefault().ID.ToString(),
                        shopCar = shopcar.FirstOrDefault(),
                        GoodsID = goods.ID.ToString(),
                        ImgPath = img.UploadPath,
                        CreateOrderTime = DateTime.Now,
                    };
                    _ShopCarGoodsItemRepository.AddAndSave(ShopCarGoodsItem);
                }
                else
                {
                    var hItem = hasGoods.FirstOrDefault();
                    hItem.Count = 1 + hItem.Count;
                    hItem.TotalPrice = (hItem.Count * decimal.Parse(goods.Price)).ToString();
                    _ShopCarGoodsItemRepository.AddOrEditAndSave(hItem);
                }
                return Json(new { isOK = true, message = "��ӹ��ﳵ�ɹ�" });
            }
            catch (Exception)
            {
                return Json(new { isOK = false, message = "���ʧ�ܣ������¼״̬����ִ�в���" });
            }

        }

        public IActionResult Order()//��������
        {
            var user = User.Claims.FirstOrDefault();
            var Orders =_SK_WM_Order.GetAllMultiCondition(x=>x.OrderForUser.Id== user.Value,y=>y.Goods);//��ȡ�����Ͷ�����Ӧ���û�����Ʒ
            var onVMCollection = new List<SK_WM_OrderVM>();//����û����ж���
           
            foreach (var order in Orders)
            {
                var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == order.Goods.ID);
                foreach (var image in images)
                {
                    if (image.IsForTitle == true)
                    {
                        order.Goods.GoodsIMG = image;
                    }
                }
                order.SortCode = order.SortCode.Substring(12);
                var omVM = new SK_WM_OrderVM(order);
                onVMCollection.Add(omVM);
            }
            return View("../../Views/Account/UserChildren/Order", onVMCollection);
        }

        public IActionResult Refund()//�˿�
        {
            return View();
        }

        public IActionResult Evaluate()//����
        {
            return View();
        }

        public IActionResult Billdetails()//�˵���ϸ
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// �������ϴ����û�ͷ��
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> SaveAvatar()
        {
            var serverPath = "";
            long size = 0;
            var files = Request.Form.Files;
            if (files.Count == 0)
            {
                return Json(new { isOK = false, fileCount = 0, size = size, serverPath = "û��ѡ���κ��ļ�����ѡ���ļ������ύ�ϳ���" });
            }
            else
            {
                var userName = User.Identity.Name;
                var user = await _UserManager.FindByNameAsync(userName);
                //var fileName = file.FileName;
                var fileName = ContentDispositionHeaderValue
                                .Parse(files[0].ContentDisposition)
                                .FileName
                                .Trim('"')
                                .Substring(files[0].FileName.LastIndexOf("\\") + 1);

                fileName = user.Id + "_" + fileName;
                var boPath = "../../images/Avatars/" + fileName;
                fileName = _HostingEnv.WebRootPath + $@"\images\Avatars\{fileName}";
                serverPath = fileName;
                size += files[0].Length;
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    files[0].CopyTo(fs);
                    fs.Flush();
                }

                var businessIamge = new BusinessImage()
                {
                    DisplayName = user.UserName,
                    RelevanceObjectID = Guid.Parse(user.Id),
                    UploadPath = boPath
                };
                var uID = Guid.Parse(user.Id);
                var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == uID).FirstOrDefault();
                if (image != null)
                {
                    image.UploadPath = boPath;
                    _ImageRepository.EditAndSave(image);
                }
                else
                {
                    user.Avatar = businessIamge;
                    await _UserManager.UpdateAsync(user);
                }

                return Json(new { isOK = true, fileCount = files.Count, size = size, serverPath = serverPath });
            }
        }

        [Authorize]
        public async Task<IActionResult> RefreshAvatar()
        {
            var userName = User.Identity.Name;
            var user = await _UserManager.FindByNameAsync(userName);
            var boVM = new ApplicationUserVM(user);

            var uID = Guid.Parse(user.Id);
            var image = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == uID).FirstOrDefault();
            if (image != null)
                boVM.AvatarPath = image.UploadPath;

            return PartialView("../../Views/Account/_AvatorPartial", boVM);

        }

        /// <summary>
        /// �Ƿ����ָ���û������û�
        /// </summary>
        /// <param name="userName">�û���</param>
        /// <returns></returns>
        private bool _HasTheSameUser(string userName) => _UserManager.Users.Any(x => x.UserName == userName);
    }
}