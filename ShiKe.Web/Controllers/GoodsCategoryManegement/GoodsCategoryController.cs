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
        /// 数据管理的入口
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
                    ModelState.AddModelError("", "名称重复，无法添加。");
                    return View("../../Views/GoodsCategoryManegement/GoodsCategory/Create", boVM);
                }

                var bo = new SK_WM_GoodsCategory();           

                // 处理一般的属性数据
                boVM.MapToBo(bo);
              
                var saveStatus = await _BoRepository.AddOrEditAndSaveAsyn(bo);
                if (saveStatus)
                    return RedirectToAction("Index");
                else
                {
                    ModelState.AddModelError("", "数据保存出现异常，无法处理，请联系开发人员。");
                    return View("../../Views/GoodsCategoryManegement/GoodsCategory/Create", boVM);
                }
         
        }      

        /// <summary>
        /// 删除数据，将删除操作的结果加载到 DeleteStatus 对象，然后转成 json 数据返回给前端。
        /// </summary>
        /// <param name="id">待删除的对象 ID 属性值</param>
        /// <returns>将删除操作处理的结果转为 json 数据返回到前端，供前端根据情况处理</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            var status = await _BoRepository.DeleteAndSaveAsyn(id);
            return Json(status);
        }
    }
}