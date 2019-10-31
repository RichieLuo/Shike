using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.DataAccess;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.ViewModels.BusinessOrganization;
using System.Linq.Expressions;
using ShiKe.DataAccess.Utilities;
using ShiKe.Common.ViewModelComponents;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ShiKe.Entities.Attachments;
using Microsoft.AspNetCore.Identity;
using ShiKe.Entities.ApplicationOrganization;

namespace ShiKe.Web.Controllers.BusinessOrganization
{
    public class GoodsManagerController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IEntityRepository<SK_WM_Goods> _GoodsRepository;
        private readonly IEntityRepository<SK_WM_Shop> _ShopRepository;
        private readonly IEntityRepository<SK_WM_GoodsCategory> _ScRepository;
        private readonly IEntityRepository<BusinessImage> _ImageRepository;
        private IHostingEnvironment _HostingEnv;
        public GoodsManagerController(
                 UserManager<ApplicationUser> userManager,
            IEntityRepository<SK_WM_Goods> prepository,
            IEntityRepository<SK_WM_Shop> hrepository,
            IHostingEnvironment hostingEnv,
            IEntityRepository<BusinessImage> imageRepository,
           
            IEntityRepository<SK_WM_GoodsCategory> scRepository)
        {
            _UserManager = userManager;
            _GoodsRepository = prepository;
            _ShopRepository = hrepository;
            _HostingEnv = hostingEnv;
            _ImageRepository = imageRepository;
            _ScRepository = scRepository;
        
        }
        /// <summary>
        /// ��ȡ��ǰ��¼���û���
        /// </summary>
        /// <returns></returns>
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
        /// ��վ����
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<IActionResult> Search(string keyword)
        {
            var boVMCollection = new List<SK_WM_GoodsVM>();
           
            if (!String.IsNullOrEmpty(keyword))
            {
                Expression<Func<SK_WM_Goods, bool>> condition = x =>
                x.Name.Contains(keyword) ||
                x.Description.Contains(keyword);
                var boCollection = await _GoodsRepository.GetAllIncludingAsyn(x => x.GoodsIMG, x => x.Shop, x => x.ShopForUser, x => x.SK_WM_GoodsCategory);

                foreach (var bo in boCollection.Where(condition))
                {
                    var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);

                    foreach (var image in images)
                    {
                        if (image.IsForTitle == true)
                        {
                            bo.GoodsIMG = image;
                        }
                    }
                    boVMCollection.Add(new SK_WM_GoodsVM(bo));
                }
                ViewBag.Keyword = keyword;
            }
            else
            {
                var boCollection = await _GoodsRepository.GetAllIncludingAsyn(x => x.GoodsIMG, x => x.Shop, x => x.ShopForUser, x => x.SK_WM_GoodsCategory);
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

                    boVMCollection.Add(new SK_WM_GoodsVM(bo));
                }
                ViewBag.Keyword = null;
            }
            //��������(���������������ͼȡǰ10��)
            ViewBag.HotSearch = _GoodsRepository.GetAll().OrderByDescending(x => x.SalesVolume);
            //��ȡ���з���
            ViewBag.GoodsCategory = await _ScRepository.GetAllAsyn();
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/BusinessBG/Search", boVMCollection);
        }

        /// <summary>
        /// ��վ����
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<IActionResult> CateSearch(string keyword)
        {
            var boVMCollection = new List<SK_WM_GoodsVM>();

            if (!String.IsNullOrEmpty(keyword))
            {      
                var boCollection = await _GoodsRepository.GetAllIncludingAsyn(x => x.GoodsIMG, x => x.Shop, x => x.ShopForUser, x => x.SK_WM_GoodsCategory);

                foreach (var bo in boCollection.Where(x=>x.SK_WM_GoodsCategory.Name==keyword))
                {
                    var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);

                    foreach (var image in images)
                    {
                        if (image.IsForTitle == true)
                        {
                            bo.GoodsIMG = image;
                        }
                    }
                    boVMCollection.Add(new SK_WM_GoodsVM(bo));
                }
                ViewBag.Keyword = keyword;
            }
            else
            {
                var boCollection = await _GoodsRepository.GetAllIncludingAsyn(x => x.GoodsIMG, x => x.Shop, x => x.ShopForUser, x => x.SK_WM_GoodsCategory);
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

                    boVMCollection.Add(new SK_WM_GoodsVM(bo));
                }
                ViewBag.Keyword = null;
            }
            //��������(���������������ͼȡǰ10��)
            ViewBag.HotSearch = _GoodsRepository.GetAll().OrderByDescending(x => x.SalesVolume);
            //��ȡ���з���
            ViewBag.GoodsCategory = await _ScRepository.GetAllAsyn();
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/BusinessBG/Search", boVMCollection);
        }

        /// <summary>
        /// ��վ����
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<IActionResult> ShopSearch(string keyword)
        {
            var boVMCollection = new List<SK_WM_GoodsVM>();
            if (!String.IsNullOrEmpty(keyword))
            {              
                var boCollection = await _GoodsRepository.GetAllIncludingAsyn(x => x.GoodsIMG, x => x.Shop, x => x.ShopForUser, x => x.SK_WM_GoodsCategory);
                foreach (var bo in boCollection.Where(x=>x.Shop.Name.Contains(keyword)))
                {
                    var images = _ImageRepository.GetAll().OrderByDescending(x => x.SortCode).Where(y => y.RelevanceObjectID == bo.ID);

                    foreach (var image in images)
                    {
                        if (image.IsForTitle == true)
                        {
                            bo.GoodsIMG = image;
                        }
                    }
                    boVMCollection.Add(new SK_WM_GoodsVM(bo));
                }
                ViewBag.Keyword = keyword;
            }
            else
            {
                var boCollection = await _GoodsRepository.GetAllIncludingAsyn(x => x.GoodsIMG, x => x.Shop, x => x.ShopForUser, x => x.SK_WM_GoodsCategory);
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
                    boVMCollection.Add(new SK_WM_GoodsVM(bo));
                }
                ViewBag.Keyword = null;
            }
            //��������(���������������ͼȡǰ10��)
            ViewBag.HotSearch = _GoodsRepository.GetAll().OrderByDescending(x => x.SalesVolume);
            //��ȡ���з���
            ViewBag.GoodsCategory = await _ScRepository.GetAllAsyn();
            ViewBag.UserLogonInformation = GetUserName();
            return View("../../Views/BusinessOrganization/BusinessBG/Search", boVMCollection);
        }
        ///// <summary>
        ///// ���ݹ�������
        ///// </summary>
        ///// <returns></returns>
        //public async Task<IActionResult> Index(Guid id)
        //{
        //    Guid id2 = new Guid();
        //    var spCollection = await _GoodsRepository.GetAllAsyn();

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
        //    return View("../../Views/BusinessShopAndSpecialty/SpecialtyManager/Index", spVMCollection);
        //}


        ///// <summary>
        ///// ���ݹؼ��ʼ�����Ʒ���ݼ��ϣ����ظ�ǰ��ҳ��
        ///// </summary>
        ///// <param name="keyword">�ؼ���</param>
        ///// <returns></returns>
        //public async Task<IActionResult> List(Guid id)
        //{
        //    var spVMCollection = new List<SK_WM_GoodsVM>();

        //    var spCollection = await _GoodsRepository.GetAllAsyn();
        //    foreach (var sp in spCollection)
        //    {
        //        spVMCollection.Add(new SK_WM_GoodsVM(sp));
        //    }
        //    //}
        //    return PartialView("../../Views/BusinessShopAndSpecialty/SpecialtyManager/_List", spVMCollection);

        //}

        ///// <summary>
        ///// �������߱༭��Ʒ���ݵĴ���
        ///// </summary>
        ///// <param name="id">��Ʒ�����ID����ֵ��������ֵ��ϵͳ���Ҳ�������Ķ����򿴳����½�����</param>
        ///// <returns></returns>
        //[HttpGet]
        //[HttpPost]
        //public async Task<IActionResult> CreateOrEdit(Guid id)
        //{
        //    #region ���
        //    var scCollection = _ScRepository.GetAll();
        //    var scACollection = new List<string>();
        //    //var scICollection = new List<string>();
        //    var scVMCollection = new List<SK_WM_GoodsCategoryVM>();
        //    foreach (var sc in scCollection)
        //    {
        //        var scVM = new SK_WM_GoodsCategoryVM(sc);
        //        scACollection.Add(scVM.Name);
        //    }
        //    ViewBag.scACollection = scACollection;
        //    #endregion

        //    #region �����û��������û��������
        //    //#region 1.��ѡ����û�������
        //    //var role = _RoleManager.Roles;
        //    //var roleItems = new List<PlainFacadeItem>();
        //    //foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
        //    //{
        //    //    var rItem = new PlainFacadeItem() { ID = item.Id, Name = item.Name, DisplayName = item.DisplayName, SortCode = item.SortCode };
        //    //    roleItems.Add(rItem);
        //    //}
        //    //boVM.RoleItemColection = roleItems;
        //    //#endregion
        //    //#region 2.�Ѿ��������û��鲿��
        //    //boVM.RoleItemIDCollection = new List<string>();
        //    //foreach (var item in _RoleManager.Roles.OrderBy(x => x.SortCode))
        //    //{
        //    //    var h = await _UserManager.IsInRoleAsync(user, item.Name);
        //    //    if (h)
        //    //    {
        //    //        boVM.RoleItemIDCollection.Add(item.Id);
        //    //    }
        //    //}
        //    //#endregion
        //    #endregion


        //    var isNew = false;
        //    var sp = await _GoodsRepository.GetSingleAsyn(id);
        //    if (sp == null)
        //    {
        //        sp = new SK_WM_Goods();
        //        sp.Name = "";
        //        sp.Unit = "";       
        //        sp.Description = "";
        //        sp.SortCode = "";
        //        sp.Stock = "";
        //        isNew = true;
        //    }

        //    var spVM = new SK_WM_GoodsVM(sp);
        //    //var scVM = new SK_WM_GoodsCategoryVM(sc);
        //    spVM.IsNew = isNew;
        //    if (spVM.GoodsCategory != null)
        //    {
        //        ViewBag.SpecialtyClass = spVM.GoodsCategory.Name;
        //    }
        //    else
        //    {
        //        ViewBag.SpecialtyClass = scACollection[0];
        //    }
        //    return View("../../Views/Shop/SpecialtyManager/CreateOrEdit", spVM);
        //}

        ///// <summary>
        ///// �Ծֲ�ҳ�ķ�ʽ�ķ�ʽ��������ϸ���ݵĴ���
        ///// </summary>
        ///// <param name="id">��Ʒ�����ID����ֵ</param>
        ///// <returns></returns>
        //public async Task<IActionResult> Detail(Guid id)
        //{
        //    var sp = await _GoodsRepository.GetSingleAsyn(id);
        //    var spVM = new SK_WM_GoodsVM(sp);
        //    return PartialView("../../Views/BusinessShopAndSpecialty/SpecialtyManager/_Detail", spVM);
        //}

        ///// <summary>
        ///// ������Ʒ����
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> Save([Bind("ID,IsNew,Name,SpecialtyNum,DistributionScope,path,FareIncrease,Express,SourcePlace,Description,SortCode,SpecialtyClass")]SK_WM_GoodsVM spVM)
        //{
        //    var hasDuplicateNmaeSclass = await _ScRepository.FindByAsyn(x => x.SpecialtyClass == spVM.SpecialtyClass);
        //    //PhoneNewUpload(spVM.files);
        //    var hasDuplicateNmaeShop = await _GoodsRepository.HasInstanceAsyn(x => x.Name == spVM.Name);
        //    spVM.SClass = hasDuplicateNmaeSclass.FirstOrDefault();
        //    if (hasDuplicateNmaeShop && spVM.IsNew)
        //    {
        //        ModelState.AddModelError("", "���и���Ʒ���޷���ӡ�");
        //        return View("../../Views/BusinessShopAndSpecialty/SpecialtyManager/CreateOrEdit", spVM);
        //    }

        //    var sp = new Specialty();
        //    if (!spVM.IsNew)
        //    {
        //        sp = await _GoodsRepository.GetSingleAsyn(spVM.ID);
        //    }

        //    spVM.MapToSp(sp);
        //    var saveStatus = await _GoodsRepository.AddOrEditAndSaveAsyn(sp);
        //    if (saveStatus)
        //        return RedirectToAction("Index");
        //    else
        //    {
        //        ModelState.AddModelError("", "���ݱ�������쳣���޷���������ϵ������Ա��");
        //        return View("../../Views/BusinessShopAndSpecialty/SpecialtyManager/CreateOrEdit", spVM);
        //    }
        //}

        ///// <summary>
        ///// ɾ�����ݣ���ɾ�������Ľ�����ص� DeleteStatus ����Ȼ��ת�� json ���ݷ��ظ�ǰ�ˡ�
        ///// </summary>
        ///// <param name="id">��ɾ���Ķ��� ID ����ֵ</param>
        ///// <returns>��ɾ����������Ľ��תΪ json ���ݷ��ص�ǰ�ˣ���ǰ�˸����������</returns>
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var status = await _GoodsRepository.DeleteAndSaveAsyn(id);
        //    return Json(status);
        //}



        //public async Task<IActionResult> PhoneNewUpload(Guid id)
        //{
        //    var sp = await _GoodsRepository.GetSingleAsyn(id);
        //    var spVM = new SK_WM_GoodsVM(sp);
        //    return View("../../Views/BusinessShopAndSpecialty/SpecialtyManager/PhoneNewUpload", spVM);
        //}

        //public async Task<IActionResult> PhoneManager(List<IFormFile> files, Guid id)
        //{
        //    var sp = new Specialty();
        //    sp = await _GoodsRepository.GetSingleAsyn(id);

        //    long size = 0;
        //    foreach (var file in files)
        //    {
        //        //var fileName = file.FileName;
        //        var fileName = ContentDispositionHeaderValue
        //                        .Parse(file.ContentDisposition)
        //                        .FileName
        //                        .Trim('"')
        //                        .Substring(file.FileName.LastIndexOf("\\") + 1);
        //        sp.SpecialtyPicture = $@"\uploadFiles\{fileName}";
        //        fileName = _HostingEnv.WebRootPath + $@"\uploadFiles\{fileName}";

        //        size += file.Length;
        //        using (FileStream fs = System.IO.File.Create(fileName))
        //        {
        //            file.CopyTo(fs);
        //            fs.Flush();
        //        }
        //    }
        //    _GoodsRepository.EditAndSave(sp);
        //    return RedirectToAction("index");
        //}
    }
}