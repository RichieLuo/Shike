using Microsoft.AspNetCore.Mvc;
using ShiKe.DataAccess;
using ShiKe.DataAccess.Utilities;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.ViewModels.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShiKe.Web.Controllers.BusineesOrganization
{
    public class PersonController : Controller
    {
        private readonly IEntityRepository<Person> _BoRepository;

        public PersonController(IEntityRepository<Person> repository)
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
            var pageSize = 10;

            var boCollection = await _BoRepository.PaginateAsyn(pageIndex,pageSize,x=>x.SortCode,null,x=>x.Department);
            var boVMCollection = new List<PersonVM>();

            var orderNumber = 0;
            foreach(var bo in boCollection)
            {
                var boVM = new PersonVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                boVMCollection.Add(boVM);
            }
            return View("../../Views/BusinessOrganization/Person/Index", boVMCollection);
        }

        /// <summary>
        /// 根据关键词检索人员数据集合，返回给前端页面
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public async Task<IActionResult> List(string keyword)
        {
            var boVMCollection = new List<PersonVM>();
            if (!String.IsNullOrEmpty(keyword))
            {
                Expression<Func<Person, bool>> condition = x =>
                x.Name.Contains(keyword) ||
                x.Email.Contains(keyword) ||
                x.Mobile.Contains(keyword) ||
                x.Description.Contains(keyword) ||
                x.SortCode.Contains(keyword);

                var boCollection = await _BoRepository.FindByAsyn(condition);
                foreach (var bo in boCollection)
                {
                    boVMCollection.Add(new PersonVM(bo));
                }
            }
            else
            {
                var boCollection = await _BoRepository.GetAllIncludingAsyn(x => x.Department);
                foreach (var bo in boCollection)
                {
                    boVMCollection.Add(new PersonVM(bo));
                }
            }
            return PartialView("../../Views/BusinessOrganization/Person/_List", boVMCollection);
        }

        /// <summary>
        /// 新增或者编辑人员数据的处理
        /// </summary>
        /// <param name="id">人员对象的ID属性值，如果这个值在系统中找不到具体的对象，则看成是新建对象。</param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(Guid id)
        {
            var isNew = false;
            var bo = await _BoRepository.GetSingleAsyn(id, x => x.Department);
            if (bo == null)
            {
                bo = new Person();
                bo.Name = "";
                bo.Email = "";
                bo.Mobile = "";
                bo.Description = "";
                bo.SortCode = "";
                isNew = true;
            }

            var boVM = new PersonVM(bo);
            boVM.IsNew = isNew;

            #region 创建供归属部门选择器使用的元素集合
            var depts = _BoRepository.EntitiesContext.Departments.ToList();
            var selectItems = SelfReferentialItemFactory<Department>.GetCollection(depts, true);
            boVM.ParentItemCollection = selectItems;
            #endregion

            return View("../../Views/BusinessOrganization/Person/CreateOrEdit", boVM);
        }

        /// <summary>
        /// 以局部页的方式的方式，构建明细数据的处理
        /// </summary>
        /// <param name="id">人员对象的ID属性值</param>
        /// <returns></returns>
        public async Task<IActionResult> Detail(Guid id)
        {
            var bo = await _BoRepository.GetSingleAsyn(id);
            var boVM = new PersonVM(bo);
            return PartialView("../../Views/BusinessOrganization/Person/_Detail", boVM);
        }

        /// <summary>
        /// 保存人员数据
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Save([Bind("ID,IsNew,ParentItemID,Name,Email,MobileNumber,Description,SortCode")]PersonVM boVM)
        {
            if (ModelState.IsValid)
            {
                var hasDuplicateNmaePerson = await _BoRepository.HasInstanceAsyn(x => x.Name == boVM.Name);
                if (hasDuplicateNmaePerson && boVM.IsNew)
                {
                    ModelState.AddModelError("", "人员姓名重复，无法添加。");
                    return View("../../Views/BusinessOrganization/Person/CreateOrEdit", boVM);
                }

                var bo = new Person();
                if (!boVM.IsNew)
                {
                    bo = await _BoRepository.GetSingleAsyn(boVM.ID);
                }

                // 处理一般的属性数据
                boVM.MapToBo(bo);

                // 处理关联数据
                if (!String.IsNullOrEmpty(boVM.ParentItemID))
                {
                    var dID = Guid.Parse(boVM.ParentItemID);
                    var dept = _BoRepository.EntitiesContext.Departments.FirstOrDefault(x => x.ID == dID);
                    bo.Department = dept;
                }
                var saveStatus = await _BoRepository.AddOrEditAndSaveAsyn(bo);
                if (saveStatus)
                    return RedirectToAction("Index");
                else
                {
                    ModelState.AddModelError("", "数据保存出现异常，无法处理，请联系开发人员。");
                    return View("../../Views/BusinessOrganization/Person/CreateOrEdit", boVM);
                }
            }
            else
            {
                return View("../../Views/BusinessOrganization/Person/CreateOrEdit", boVM);
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
