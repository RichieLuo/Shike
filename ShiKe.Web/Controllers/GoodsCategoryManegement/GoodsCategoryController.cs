using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.DataAccess;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.ViewModels.BusinessOrganization;
using System.Linq.Expressions;

namespace ShiKe.Web.Controllers.GoodsCategoryManegement
{
    public class GoodsCategoryController : Controller
    {
        private readonly IEntityRepository<SK_WM_GoodsCategory> _BoRepository;

        public GoodsCategoryController(IEntityRepository<SK_WM_GoodsCategory> repository)
        {
            _BoRepository = repository;
        }

        /// <summary>
        /// ���ݹ�������
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var pageIndex = 1;
            var pageSize = 50;

            var boCollection = await _BoRepository.PaginateAsyn(pageIndex, pageSize, x => x.SortCode, null, null);
            var boVMCollection = new List<SK_WM_GoodsCategoryVM>();

            var orderNumber = 0;
            foreach (var bo in boCollection)
            {
                var boVM = new SK_WM_GoodsCategoryVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                boVMCollection.Add(boVM);
            }
            return View("../../Views/GoodsCategoryManegement/GoodsCategory/Index", boVMCollection);
        }

        public async Task<IActionResult> List(string keyword)
        {
            var boVMCollection = new List<SK_WM_GoodsCategoryVM>();
            if (!String.IsNullOrEmpty(keyword))
            {
                Expression<Func<SK_WM_GoodsCategory, bool>> condition = x =>
                x.Name.Contains(keyword) ||
                x.Description.Contains(keyword) ||
                x.SortCode.Contains(keyword);

                var boCollection = await _BoRepository.FindByAsyn(condition);
                foreach (var bo in boCollection)
                {
                    boVMCollection.Add(new SK_WM_GoodsCategoryVM(bo));
                }
            }
            else
            {
                var pageIndex = 1;
                var pageSize = 10;

                var boCollection = await _BoRepository.PaginateAsyn(pageIndex, pageSize, x => x.SortCode, null, x => x.Name);

                var orderNumber = 0;
                foreach (var bo in boCollection)
                {
                    var boVM = new SK_WM_GoodsCategoryVM(bo);
                    boVM.OrderNumber = (++orderNumber).ToString();
                    boVMCollection.Add(boVM);
                }
            }
            return PartialView("../../Views/GoodsCategoryManegement/GoodsCategory/_List", boVMCollection);
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Create(Guid id)
        {
            var isNew = false;
            var bo = await _BoRepository.GetSingleAsyn(id);
            if (bo == null)
            {
                bo = new SK_WM_GoodsCategory();
                bo.Name = "";             
                bo.Description = "";
                isNew = true;
            }          
            var boVM = new SK_WM_GoodsCategoryVM(bo);          
            boVM.IsNew = isNew;

            return View("../../Views/GoodsCategoryManegement/GoodsCategory/Create", boVM);
        }


        [HttpPost]
        public async Task<IActionResult> Save([Bind("ID,IsNew,Name,Description,SortCode")]SK_WM_GoodsCategoryVM boVM)
        {
          
                var hasDuplicateNmaeGoods = await _BoRepository.HasInstanceAsyn(x => x.Name == boVM.Name);
                if (hasDuplicateNmaeGoods && boVM.IsNew)
                {
                    ModelState.AddModelError("", "�����ظ����޷���ӡ�");
                    return View("../../Views/GoodsCategoryManegement/GoodsCategory/Create", boVM);
                }

                var bo = new SK_WM_GoodsCategory();           

                // ����һ�����������
                boVM.MapToBo(bo);
              
                var saveStatus = await _BoRepository.AddOrEditAndSaveAsyn(bo);
                if (saveStatus)
                    return RedirectToAction("Index");
                else
                {
                    ModelState.AddModelError("", "���ݱ�������쳣���޷���������ϵ������Ա��");
                    return View("../../Views/GoodsCategoryManegement/GoodsCategory/Create", boVM);
                }
         
        }      

        /// <summary>
        /// ɾ�����ݣ���ɾ�������Ľ�����ص� DeleteStatus ����Ȼ��ת�� json ���ݷ��ظ�ǰ�ˡ�
        /// </summary>
        /// <param name="id">��ɾ���Ķ��� ID ����ֵ</param>
        /// <returns>��ɾ����������Ľ��תΪ json ���ݷ��ص�ǰ�ˣ���ǰ�˸����������</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            var status = await _BoRepository.DeleteAndSaveAsyn(id);
            return Json(status);
        }
    }
}