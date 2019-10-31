using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.DataAccess;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.ViewModels.BusinessOrganization;
using System.Linq.Expressions;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.ApplicationOrganization;
using Microsoft.AspNetCore.Identity;
using ShiKe.Web.Controllers.ApplicationOrganization;
using ShiKe.Common.ViewModelComponents;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using ShiKe.DataAccess.SqlServer.Ultilities;
using ShiKe.Common.JsonModels;
using ShiKe.DataAccess.Common;

namespace ShiKe.Web.Controllers.BusinessShopAndSpecialty
{
    [Authorize]
    public class ShopManagerController : Controller
    {
        private readonly IEntityRepository<SK_WM_Shop> _ShopRepository;
        private readonly IEntityRepository<BusinessImage> _ImageRepository;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IEntityRepository<SK_WM_ShopSttled> _ShopSelltedRepository;
        private readonly IEntityRepository<SK_WM_ShopExecuteIllegal> _ShopEIllegalRepository;
        private IHostingEnvironment _HostingEnv;

        public ShopManagerController(IEntityRepository<SK_WM_Shop> repository,
            IEntityRepository<BusinessImage> imageRepository,
            UserManager<ApplicationUser> userManager,
            IEntityRepository<SK_WM_ShopSttled> shopSettled,
            IEntityRepository<SK_WM_ShopExecuteIllegal> shopExecuteIllegal,
            IHostingEnvironment hostingEnv)
        {
            _ShopRepository = repository;
            _ImageRepository = imageRepository;
            _HostingEnv = hostingEnv;
            _UserManager = userManager;
            _HostingEnv = hostingEnv;
            _ShopSelltedRepository = shopSettled;
            _ShopEIllegalRepository = shopExecuteIllegal;
        }

        /// <summary>
        /// ��ȡ�û���
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
        /// <summary>
        /// ���ݹ�������
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.UserLogonInformation = GetUserName();

            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = "Shop",            // ��Ӧ�Ĺ�������ID
                PageIndex = 1,                     // ��ǰҳ��
                PageSize = 8,                     // ÿҳ�������� Ϊ"0"ʱ��ʾ����
                PageAmount = 0,                    // ��ض����б��ҳ�����ҳ����
                ObjectAmount = 0,                  // ��صĶ��������
                Keyword = "",                      // ��ǰ�Ĺؼ���
                SortProperty = "SortCode",         // ��������
                SortDesc = "default",              // ������ȱʡֵ���� Default��ǰ���ÿ��ط�ʽתΪ����Descend
                SelectedObjectID = "",             // ��ǰҳ�洦���д���Ľ������ ID
                IsSearch = false,                  // ��ǰ�Ƿ�Ϊ����
            };

            var typeID = "";
            var keyword = "";
            if (!String.IsNullOrEmpty(listPagePara.ObjectTypeID))
                typeID = listPagePara.ObjectTypeID;
            if (!String.IsNullOrEmpty(listPagePara.Keyword))
                keyword = listPagePara.Keyword;

            //var shCollection = await _ShopRepository.GetAllIncludingAsyn(x => x.ShopForUser,x=>x.ShopAvatar, x => x.ShopBanner);
            var shCollection = await _ShopRepository.PaginateAsyn(listPagePara.PageIndex, listPagePara.PageSize, x => x.SortCode, null, x => x.ShopAvatar, x => x.ShopBanner,x=>x.ShopForExecuteIllegal,x=>x.ShopForUser);

            var shVMCollection = new List<SK_WM_ShopVM>();
            foreach (var sh in shCollection)
            {
                if (sh.ShopForExecuteIllegal.ShopState == (SK_WM_ShopState.ShopState.�ѿ���).ToString())
                {
                    var shVM = new SK_WM_ShopVM(sh);
                    var images = _ImageRepository.GetAll().Where(x => x.RelevanceObjectID == sh.ID);
                    foreach (var img in images)
                    {
                        if (img.Description == "shopAvatar")
                        {
                            shVM.ShopAvatarPath = img.UploadPath;
                        }
                        if (img.Description == "shopBanner")
                        {
                            shVM.ShopBannerPath = img.UploadPath;
                        }
                    }
                    shVMCollection.Add(shVM);
                }
            }
            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_Shop>(shCollection, 10, listPagePara.PageIndex);
            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;
            ViewBag.GoodsType = "Admin";
            return View("../../Views/Shop/ShopManager/Index", shVMCollection);
        }
        /// <summary>
        /// ���ڷ�ҳ
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="objectTypeID"></param>
        /// <returns></returns>
        public async Task<IActionResult> IndexPageList(int pageIndex, string objectTypeID)
        {
            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = objectTypeID,           // ��Ӧ�Ĺ�������ID
                PageIndex = pageIndex,                 // ��ǰҳ��
                PageSize = 8,                  // ÿҳ�������� Ϊ"0"ʱ��ʾ����
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

            //var shCollection = await _ShopRepository.GetAllIncludingAsyn(x => x.ShopForUser,x=>x.ShopAvatar, x => x.ShopBanner);
            var shCollection = await _ShopRepository.PaginateAsyn(listPagePara.PageIndex, listPagePara.PageSize, x => x.SortCode, null, x => x.ShopAvatar, x => x.ShopBanner);

            var shVMCollection = new List<SK_WM_ShopVM>();
            foreach (var sh in shCollection)
            {
                var shVM = new SK_WM_ShopVM(sh);
                var images = _ImageRepository.GetAll().Where(x => x.RelevanceObjectID == sh.ID);
                foreach (var img in images)
                {
                    if (img.Description == "shopAvatar")
                    {
                        shVM.ShopAvatarPath = img.UploadPath;
                    }
                    if (img.Description == "shopBanner")
                    {
                        shVM.ShopBannerPath = img.UploadPath;
                    }
                }
                shVMCollection.Add(shVM);
            }
            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_Shop>(shCollection, 10, listPagePara.PageIndex);
            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;
            ViewBag.GoodsType = "Admin";
            return PartialView("../../Views/Shop/ShopManager/_List", shVMCollection);
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Settled()
        {
            var username = User.Identity.Name;
            var user = await _UserManager.FindByNameAsync(username);
            if (user == null)
            {
                return View("../../Views/Home/Logon");
            }
            //bool isNew = false;
            var hasDuplicateNameShop = await _ShopSelltedRepository.HasInstanceAsyn(x => x.ShopForUser == user);
            var bo = _ShopSelltedRepository.GetAll().Where(x => x.ShopForUser == user).FirstOrDefault();
            if (hasDuplicateNameShop)
            {
                bo.IsNew = false;
            }
            else
            {
                bo = new SK_WM_ShopSttled();
                bo.IsNew = true;
                bo.Step = 0;
            }
            var boVM = new SK_WM_ShopSttledVM(bo);
            ViewBag.UserLogonInformation = GetUserName();
            if (hasDuplicateNameShop)
            {
                return View("../../Views/Shop/ShopManager/HasSettled", boVM);
            }
            else
            {
                return View("../../Views/Shop/ShopManager/Settled", boVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettled([Bind("ID,IsNew,Step,Name,Address,UserName,IDCar,MobilePhone,Telephone,LicenceID")]SK_WM_ShopSttledVM boVM)
        {
            var validateMessage = new ValidateMessage();
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var user = await _UserManager.FindByNameAsync(username);
                var hasDuplicateNameShop = await _ShopSelltedRepository.HasInstanceAsyn(x => x.Name == boVM.Name);
                if (hasDuplicateNameShop && boVM.IsNew)
                {
                    validateMessage.IsOK = false;
                    validateMessage.ValidateMessageItems.Add(
                        new ValidateMessageItem()
                        { IsPropertyName = false, MessageName = "Name", Message = "�Ѿ��е��̽�������ˣ���һ�����Կ���" });
                    return Json(validateMessage);
                }

                var bo = new SK_WM_ShopSttled();
                bo.ID = boVM.ID;
                if (!boVM.IsNew)
                {
                    bo = await _ShopSelltedRepository.GetSingleAsyn(boVM.ID);

                }

                // ����һ�����������
                boVM.MapToShop(bo);

                var saveStatus = false;

                //�����ϴ��ļ�
                var serverPath = "";
                long size = 0;
                var files = Request.Form.Files;
                var boImg = _ImageRepository.GetAll().Where(x => x.RelevanceObjectID == bo.ID);
                var FrontIDCarimg = boImg.Where(x => x.Description == "FrontIDCar").FirstOrDefault();
                var backIDCarimg = boImg.Where(x => x.Description == "BackIDCar").FirstOrDefault();
                var Environmentimg = boImg.Where(x => x.Description == "Environment").FirstOrDefault();
                var Licenceimg = boImg.Where(x => x.Description == "Licence").FirstOrDefault();

                var fName = files.Where(x => x.Name == "FrontIDCar").FirstOrDefault();
                var bName = files.Where(x => x.Name == "BackIDCar").FirstOrDefault();
                var eName = files.Where(x => x.Name == "Environment").FirstOrDefault();
                var lName = files.Where(x => x.Name == "Environment").FirstOrDefault();

                if (boVM.IsNew == false)
                {
                    if (/*(backIDCarimg == null || FrontIDCarimg == null)&& */(fName == null || bName == null) && boVM.Step == 1)
                    {
                        validateMessage.IsOK = false;
                        validateMessage.ValidateMessageItems.Add(
                            new ValidateMessageItem()
                            { IsPropertyName = false, MessageName = "IdImg", Message = "�뽫�����Ϣ����������ִ����һ��" });
                        return Json(validateMessage);
                    }
                    if (/*(backIDCarimg != null || FrontIDCarimg != null)*//*&&(Environmentimg == null || Licenceimg == null) && */(eName == null || lName == null) && (fName != null || bName != null) && boVM.Step == 2)
                    {
                        validateMessage.IsOK = false;
                        validateMessage.ValidateMessageItems.Add(
                            new ValidateMessageItem()
                            { IsPropertyName = false, MessageName = "SettledImg", Message = "�뽫������Ϣ����������ִ����һ��" });
                        return Json(validateMessage);
                    }
                }
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        var fileName = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"')
                                .Substring(files[0].FileName.LastIndexOf("\\") + 1);

                        fileName = bo.ID + "_" + fileName;

                        var boPath = "../../images/ShopSettledImg/" + fileName;
                        fileName = _HostingEnv.WebRootPath + $@"\images\ShopSettledImg\{fileName}";
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
                            UploadPath = boPath,

                            Description = file.Name
                        };

                        if (file.Name == "Licence")
                        {
                            bo.Licence = businessIamge;
                            ViewBag.LiImg = businessIamge.UploadPath;
                        }
                        else if (file.Name == "Environment")
                        {
                            bo.Environment = businessIamge;
                            ViewBag.EnIMg = businessIamge.UploadPath;
                        }
                        else if (file.Name == "BackIDCar")
                        {

                            if (backIDCarimg == null)
                            {
                                bo.BackIDCar = businessIamge;
                                ViewBag.BaImg = businessIamge.UploadPath;
                            }
                            else
                            {
                                backIDCarimg.UploadPath = businessIamge.UploadPath;
                                bo.BackIDCar = backIDCarimg;
                                ViewBag.BaImg = backIDCarimg.UploadPath;
                            }

                        }
                        else if (file.Name == "FrontIDCar")
                        {

                            if (FrontIDCarimg == null)
                            {
                                bo.FrontIDCar = businessIamge;
                                ViewBag.FCimg = businessIamge.UploadPath;
                            }
                            else
                            {
                                FrontIDCarimg.UploadPath = businessIamge.UploadPath;
                                bo.FrontIDCar = FrontIDCarimg;
                                ViewBag.FCimg = FrontIDCarimg.UploadPath;
                            }
                        }

                    }
                }

                bo.ShopForUser = user;
                bo.BelongToUserID = user.Id;
                saveStatus = await _ShopSelltedRepository.AddOrEditAndSaveAsyn(bo);

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


        [HttpPost]
        public async Task<IActionResult> CancelSettled()
        {
            var status = new DeleteStatusModel();
            var username = User.Identity.Name;
            var user = await _UserManager.FindByNameAsync(username);
            var bo = _ShopSelltedRepository.GetSingleBy(x => x.BelongToUserID == user.Id);
            if (bo != null)
            {
                var imgs = await _ImageRepository.FindByAsyn(x => x.RelevanceObjectID == bo.ID);
                foreach (var img in imgs)
                {
                    await _ImageRepository.DeleteAndSaveAsyn(img.ID);
                }

                status = await _ShopSelltedRepository.DeleteAndSaveAsyn(bo.ID);
                status.Message = "�����ɹ�";
            }
            return Json(status);

        }

        /// <summary>
        /// ��ѯ���д������פ������
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> NoExamineForAllSettled()
        {
            var SettledCollection = await _ShopSelltedRepository.FindByAsyn(x => x.State == 0 && x.Step == 2);

            if (SettledCollection.Count() == 0)
            {
                ViewBag.SettledCollection = null;
                return View("../../Views/Shop/ShopManager/NoExamineForAllSettled");
            }
            var SettledVMCollection = new List<SK_WM_ShopSttledVM>();

            foreach (var settled in SettledCollection)
            {
                var settledUser = _UserManager.Users.Where(x => x.Id == settled.BelongToUserID).FirstOrDefault();
                settled.ShopForUser = settledUser;

                var SettledVM = new SK_WM_ShopSttledVM(settled);
                SettledVMCollection.Add(SettledVM);

            }
            ViewBag.SettledCollection = SettledCollection.Count();
            return View("../../Views/Shop/ShopManager/NoExamineForAllSettled", SettledVMCollection);

        }

        /// <summary>
        /// ���ݹؼ��ʼ����������ݼ��ϣ����ظ�ǰ��ҳ��
        /// </summary>
        /// <param name="keyword">�ؼ���</param>
        /// <returns></returns>
        public async Task<IActionResult> NoExamineForAllSettledList(string keyword)
        {
            var shVMCollection = new List<SK_WM_ShopSttledVM>();
            if (!String.IsNullOrEmpty(keyword))
            {
                Expression<Func<SK_WM_ShopSttled, bool>> condition = x =>
                x.Name.Contains(keyword) ||
                x.UserName.Contains(keyword) ||
                  x.Telephone.Contains(keyword) ||
                x.ShelvesTime.ToString().Contains(keyword) ||
                x.Description.Contains(keyword) ||
                x.SortCode.Contains(keyword);

                var shCollection = await _ShopSelltedRepository.FindByAsyn(condition);
                foreach (var sh in shCollection)
                {
                    shVMCollection.Add(new SK_WM_ShopSttledVM(sh));
                }
            }
            else
            {
                var shCollection = await _ShopSelltedRepository.GetAllAsyn();
                foreach (var sh in shCollection)
                {
                    var user = await _UserManager.FindByIdAsync(sh.BelongToUserID);
                    sh.ShopForUser = user;
                    shVMCollection.Add(new SK_WM_ShopSttledVM(sh));
                }
            }
            return PartialView("../../Views/Shop/ShopManager/_NoExamineForAllSettledList", shVMCollection);

        }


        /// <summary>
        /// ���ݹؼ��ʼ����������ݼ��ϣ����ظ�ǰ��ҳ��
        /// </summary>
        /// <param name="keyword">�ؼ���</param>
        /// <returns></returns>
        public async Task<IActionResult> List(string keyword)
        {
            var shVMCollection = new List<SK_WM_ShopVM>();
            if (!String.IsNullOrEmpty(keyword))
            {
                Expression<Func<SK_WM_Shop, bool>> condition = x =>
                x.Name.Contains(keyword) ||
                x.Telephone.Contains(keyword) ||
                x.Adress.Contains(keyword) ||
                x.SettledDateTime.ToString().Contains(keyword) ||
                x.Description.Contains(keyword) ||
                x.SortCode.Contains(keyword);

                var shCollection = await _ShopRepository.FindByAsyn(condition);
                foreach (var sh in shCollection)
                {
                    shVMCollection.Add(new SK_WM_ShopVM(sh));
                }
            }
            else
            {
                var shCollection = await _ShopRepository.GetAllAsyn();
                foreach (var sh in shCollection)
                {
                    shVMCollection.Add(new SK_WM_ShopVM(sh));
                }
            }
            return PartialView("../../Views/Shop/ShopManager/_List", shVMCollection);
        }


        /// <summary>
        /// ��������פͨ�����̼��½�һ��Ĭ�ϵĵ���
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> AddDefaultShop(string id)
        {
            var status = new DeleteStatusModel();
            //��ѯ��ǰ������פ�û�����Ϣ   
            var currUser = await _UserManager.FindByIdAsync(id);

            //��ѯ��ǰ������פ���û��Ƿ��Ѿ�ӵ�е���
            var shopForUsers = await _ShopRepository.FindByAsyn(x => x.ShopForUser.Id == currUser.Id);
            var shopForCurrUser = shopForUsers.FirstOrDefault();
            //���жϽ������ڵ���
            if (shopForCurrUser != null)
            {
                status.DeleteSatus = false;
                status.Message = "��ǰ�û��Ѿ���������";
                return Json(status);
            }
            else
            {
                //�����̼�������פʱ�������ȡ�����ֵ�����Ϣ
                var settShops = await _ShopSelltedRepository.FindByAsyn(x => x.BelongToUserID == id);
                var settShop = settShops.FirstOrDefault();

                //���ڴ������Υ�������ʹ��
                var shopIllegal = new SK_WM_ShopExecuteIllegal()
                {
                    ModifyTime = DateTime.Now,
                    ShopState = (SK_WM_ShopState.ShopState.�ѿ���).ToString(),
                    IllegalCategory = "",
                    Description = "����ʹ��",
                };
                var defaultShop = new SK_WM_Shop()
                {
                    ShopForUser = currUser,
                    BelongToUserID = currUser.Id,
                    State = (SK_WM_ShopState.ShopState.�ѿ���).ToString(),
                    Name = settShop.Name,
                    Grade = 5.0m,
                    Collection = 0,
                    SettledDateTime = DateTime.Now,
                    Telephone = settShop.MobilePhone,
                    Adress = settShop.Address,
                    Description = "��ӭ��Ϊʳ�̵ĵ����������ǵ�������",
                    ShopForExecuteIllegal = shopIllegal,

                };
                var defaultShopAvatar = new BusinessImage()
                {
                    ID = Guid.NewGuid(),
                    RelevanceObjectID = defaultShop.ID,
                    UploadPath = "/images/logo-For-Seller.png",
                    Description = "shopAvatar"

                };
                var defaultShopBanner = new BusinessImage()
                {
                    ID = Guid.NewGuid(),
                    RelevanceObjectID = defaultShop.ID,
                    UploadPath = "/images/hf.jpg",
                    Description = "shopBanner"
                };

                await _ShopEIllegalRepository.AddOrEditAndSaveAsyn(shopIllegal);
                var shop = await _ShopRepository.AddOrEditAndSaveAsyn(defaultShop);
                var shopIMG1 = await _ImageRepository.AddOrEditAndSaveAsyn(defaultShopAvatar);
                var shopIMG2 = await _ImageRepository.AddOrEditAndSaveAsyn(defaultShopBanner);
                if (shop)
                {
                    //���ͨ�����޸������
                    //���Ա��Ϣ
                    var currExamineName = User.Identity.Name;
                    var currExamine = await _UserManager.FindByNameAsync(currExamineName);

                    //ȡ����ǰ������˵�������޸�״̬ stateΪ1��0����� 1ͨ���� ����Ϊ3���Ѿ���ɣ� 
                    var currSettled = _ShopSelltedRepository.GetAll().Where(x => x.State == 0 && x.Step == 2 && x.BelongToUserID == currUser.Id).FirstOrDefault();
                    /* var currSettled = currSettleds.FirstOrDefault()*/
                    ;
                    currSettled.State = 1;
                    currSettled.Step = 3;
                    currSettled.BelongToExamineID = currExamine.Id;
                    currSettled.ShopForExamine = currExamine;
                    var sett = await _ShopSelltedRepository.AddOrEditAndSaveAsyn(currSettled);

                    //������˲����ɹ�����Ϣ
                    status.DeleteSatus = true;
                    status.Message = "���ͨ�������ɹ�,�Ѿ�Ϊ�û����һ��Ĭ�ϵ��̣�";
                }
                return Json(status);
            }

        }
        /// <summary>
        /// �������߱༭�������ݵĴ���
        /// </summary>
        /// <param name="id">���̶����ID����ֵ��������ֵ��ϵͳ���Ҳ�������Ķ����򿴳����½�����</param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(Guid id)
        {
            var isNew = false;
            var sh = await _ShopRepository.GetSingleAsyn(id);
            if (sh == null)
            {
                sh = new SK_WM_Shop();
                sh.Name = "";
                sh.Telephone = "";
                sh.Adress = "";
                sh.Description = "";
                sh.SortCode = "";
                isNew = true;
            }
            var shVM = new SK_WM_ShopVM(sh);
            shVM.IsNew = isNew;
            return View("../../Views/Shop/ShopManager/CreateOrEdit", shVM);
        }

        /// <summary>
        /// �Ծֲ�ҳ�ķ�ʽ�ķ�ʽ��������ϸ���ݵĴ���
        /// </summary>
        /// <param name="id">���̶����ID����ֵ</param>
        /// <returns></returns>
        public async Task<IActionResult> Detail(Guid id)
        {
            var sh = await _ShopRepository.GetSingleAsyn(id);
            var shVM = new SK_WM_ShopVM(sh);
            return PartialView("../../Views/Shop/ShopManager/_Detail", shVM);
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Save([Bind("ID,IsNew,Name,Telephone,SettledDateTime,Adress,Description,SortCode")]SK_WM_ShopVM shVM)
        {
            //var hasDuplicateNmaeShop = await _ShopRepository.HasInstanceAsyn(x => x.Name == shVM.Name);
            //if (hasDuplicateNmaeShop && shVM.IsNew)
            //{
            //    ModelState.AddModelError("", "���������ظ����޷���ӡ�");
            //    return View("../../Views/Shop/ShopManager/CreateOrEdit", shVM);
            //}

            var sh = new SK_WM_Shop();
            if (!shVM.IsNew)
            {
                sh = await _ShopRepository.GetSingleAsyn(shVM.ID);
            }
            sh.SettledDateTime = shVM.SettledDateTime;
            shVM.MapToSh(sh);
            var saveStatus = await _ShopRepository.AddOrEditAndSaveAsyn(sh);
            if (saveStatus)
                return RedirectToAction("Index");
            else
            {
                ModelState.AddModelError("", "���ݱ�������쳣���޷���������ϵ������Ա��");
                return View("../../Views/Shop/ShopManager/CreateOrEdit", shVM);
            }
        }

        /// <summary>
        /// ɾ�����ݣ���ɾ�������Ľ�����ص� DeleteStatus ����Ȼ��ת�� json ���ݷ��ظ�ǰ�ˡ�
        /// </summary>
        /// <param name="id">��ɾ���Ķ��� ID ����ֵ</param>
        /// <returns>��ɾ����������Ľ��תΪ json ���ݷ��ص�ǰ�ˣ���ǰ�˸����������</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            var status = await _ShopRepository.DeleteAndSaveAsyn(id);
            return Json(status);
        }


        #region Υ���������

        /// <summary>
        /// ��ѯΥ��ĵ���
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SelectIllegalShop()
        {
            ViewBag.UserLogonInformation = GetUserName();

            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = "Shop",            // ��Ӧ�Ĺ�������ID
                PageIndex = 1,                     // ��ǰҳ��
                PageSize = 8,                     // ÿҳ�������� Ϊ"0"ʱ��ʾ����
                PageAmount = 0,                    // ��ض����б��ҳ�����ҳ����
                ObjectAmount = 0,                  // ��صĶ��������
                Keyword = "",                      // ��ǰ�Ĺؼ���
                SortProperty = "SortCode",         // ��������
                SortDesc = "default",              // ������ȱʡֵ���� Default��ǰ���ÿ��ط�ʽתΪ����Descend
                SelectedObjectID = "",             // ��ǰҳ�洦���д���Ľ������ ID
                IsSearch = false,                  // ��ǰ�Ƿ�Ϊ����
            };

            var typeID = "";
            var keyword = "";
            if (!String.IsNullOrEmpty(listPagePara.ObjectTypeID))
                typeID = listPagePara.ObjectTypeID;
            if (!String.IsNullOrEmpty(listPagePara.Keyword))
                keyword = listPagePara.Keyword;

            var shCollection = await _ShopRepository.PaginateAsyn(listPagePara.PageIndex, listPagePara.PageSize, x => x.SortCode, null, x => x.ShopAvatar, x => x.ShopBanner, x => x.ShopForExecuteIllegal,x=>x.ShopForUser);
            var shVMCollection = new List<SK_WM_ShopVM>();
            foreach (var sh in shCollection)
            {
                if (sh.ShopForExecuteIllegal.ShopState == (SK_WM_ShopState.ShopState.��ͣ��).ToString())
                {
                    var shVM = new SK_WM_ShopVM(sh);
                    var images = _ImageRepository.GetAll().Where(x => x.RelevanceObjectID == sh.ID);
                    foreach (var img in images)
                    {
                        if (img.Description == "shopAvatar")
                        {
                            shVM.ShopAvatarPath = img.UploadPath;
                        }
                        if (img.Description == "shopBanner")
                        {
                            shVM.ShopBannerPath = img.UploadPath;
                        }
                    }
                    shVMCollection.Add(shVM);
                }

            }
            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_Shop>(shCollection, 10, listPagePara.PageIndex);
            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;
            ViewBag.GoodsType = "Admin";
            return View("../../Views/Shop/ShopManager/SelectIllegalShop", shVMCollection);
        }

        /// <summary>
        /// ���ڷ�ҳ
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="objectTypeID"></param>
        /// <returns></returns>
        public async Task<IActionResult> IllegalShopPageList(int pageIndex, string objectTypeID)
        {
            var listPagePara = new ListPageParameter()
            {
                ObjectTypeID = objectTypeID,           // ��Ӧ�Ĺ�������ID
                PageIndex = pageIndex,                 // ��ǰҳ��
                PageSize = 8,                  // ÿҳ�������� Ϊ"0"ʱ��ʾ����
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
            var shCollection = await _ShopRepository.PaginateAsyn(listPagePara.PageIndex, listPagePara.PageSize, x => x.SortCode, null, x => x.ShopAvatar, x => x.ShopBanner, x => x.ShopForExecuteIllegal,x=>x.ShopForUser);

            var shVMCollection = new List<SK_WM_ShopVM>();
            foreach (var sh in shCollection)
            {
                var shVM = new SK_WM_ShopVM(sh);
                var images = _ImageRepository.GetAll().Where(x => x.RelevanceObjectID == sh.ID);
                foreach (var img in images)
                {
                    if (img.Description == "shopAvatar")
                    {
                        shVM.ShopAvatarPath = img.UploadPath;
                    }
                    if (img.Description == "shopBanner")
                    {
                        shVM.ShopBannerPath = img.UploadPath;
                    }
                }
                shVMCollection.Add(shVM);
            }
            var pageGroup = PagenateGroupRepository.GetItem<SK_WM_Shop>(shCollection, 10, listPagePara.PageIndex);
            ViewBag.PageGroup = pageGroup;
            ViewBag.PageParameter = listPagePara;
            ViewBag.GoodsType = "Admin";
            return PartialView("../../Views/Shop/ShopManager/_List", shVMCollection);
        }

        /// <summary>
        /// ִ��Υ����������̣�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExecuteIllegal(Guid id)
        {
            var isNew = false;
            var bo = await _ShopRepository.GetSingleAsyn(id, x => x.ShopForUser);
            var shVM = new SK_WM_ShopVM(bo);
            shVM.IsNew = isNew;
            return View("../../Views/Shop/ShopManager/_ShopIllegalExecute", shVM);
        }
        /// <summary>
        /// ����ִ��Υ����������̣�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveExecuteIllegal([Bind("ID,IsNew,ShopID,ShopState,IllegalCategory,Description")]SK_WM_ShopExecuteIllegal shopEI)
        {
            var shops = await _ShopRepository.GetAllIncludingAsyn(x => x.ShopForExecuteIllegal);
            var shop = shops.Where(x => x.ID == Guid.Parse(shopEI.ShopID)).FirstOrDefault();
            var Illegal = await _ShopEIllegalRepository.FindByAsyn(x => x.ID == shop.ShopForExecuteIllegal.ID);
            var shopIllegal = Illegal.FirstOrDefault();

            shopIllegal.IllegalTime = DateTime.Now;
            shopIllegal.ModifyTime = DateTime.Now;
            shopIllegal.ShopState = shopEI.ShopState;
            shopIllegal.IllegalCategory = shopEI.IllegalCategory;
            shopIllegal.Description = shopEI.Description;


           var Status= await _ShopEIllegalRepository.AddOrEditAndSaveAsyn(shopIllegal);
            return RedirectToAction("Index","ShopManager");

        }

        #endregion
    }
}