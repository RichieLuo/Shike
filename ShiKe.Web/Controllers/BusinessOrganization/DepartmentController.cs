using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.DataAccess;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.ViewModels.BusinessOrganization;
using ShiKe.DataAccess.Utilities;
using ShiKe.DataAccess.SqlServer.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace ShiKe.Web.Controllers.BusineesOrganization
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IEntityRepository<Department> _BoRepository;

        public DepartmentController(IEntityRepository<Department> repository)
        {
            _BoRepository = repository;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var boCollection = await _BoRepository.GetAllAsyn();
            var boVMCollection = new List<DepartmentVM>();

            var couter = 0;
            foreach (var item in boCollection.OrderBy(x => x.SortCode))
            {
                var boVM = new DepartmentVM(item);
                boVM.OrderNumber = (++couter).ToString();
                boVMCollection.Add(boVM);
            }

            #region 为部门数据呈现处理名称缩进
            var deptItems = SelfReferentialItemFactory<Department>.GetCollection(boCollection.ToList(), true);
            foreach (var item in deptItems)
            {
                var dID = Guid.Parse(item.ID);
                var dept = boVMCollection.FirstOrDefault(x => x.ID == dID);
                dept.Name = item.DisplayName;
            }
            #endregion

            return View("../../Views/BusinessOrganization/Department/Index", boVMCollection);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List(string keyword)
        {
            var boCollection = await _BoRepository.GetAllAsyn();
            var boVMCollection = new List<DepartmentVM>();

            var couter = 0;
            foreach (var item in boCollection.OrderBy(x => x.SortCode))
            {
                var boVM = new DepartmentVM(item);
                boVM.OrderNumber = (++couter).ToString();
                boVMCollection.Add(boVM);
            }

            #region 为部门数据呈现处理名称缩进
            var deptItems = SelfReferentialItemFactory<Department>.GetCollection(boCollection.ToList(), true);
            foreach (var item in deptItems)
            {
                var dID = Guid.Parse(item.ID);
                var dept = boVMCollection.FirstOrDefault(x => x.ID == dID);
                dept.Name = item.DisplayName;
            }
            #endregion

            return PartialView("../../Views/BusinessOrganization/Department/_List", boVMCollection);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var bo = await _BoRepository.GetSingleAsyn(id);
            var boVM = new DepartmentVM(bo);
            return PartialView("../../Views/BusinessOrganization/Department/_Detail", boVM);
        }

        [HttpGet]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrEdit(Guid id)
        {
            var isNew = false;
            var bo = await _BoRepository.GetSingleAsyn(id);

            if (bo != null && bo.ParentDepartment == null)
            {
                var AllAsyn = _BoRepository.GetAllAsyn();
                foreach (var item in AllAsyn.Result)
                {
                    if (item.ID == id)
                    {
                        bo.ParentDepartment = item.ParentDepartment;
                    }
                }
            }
            if (bo == null)
            {
                bo = new Department();
                bo.Name = "";
                bo.Description = "";
                bo.SortCode = "";
                isNew = true;
            }

            var boVM = new DepartmentVM(bo);
            boVM.IsNew = isNew;

            #region 创建供归属部门选择器使用的元素集合
            var depts = _BoRepository.EntitiesContext.Departments.ToList();
            var selectItems = SelfReferentialItemFactory<Department>.GetCollection(depts, true);
            boVM.ParentItemCollection = selectItems;
            #endregion

            return PartialView("../../Views/BusinessOrganization/Department/_CreateOrEdit", boVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Save(string jsonBoVM)
        {
            var saveStatus = new EditAndSaveStatus() { SaveOk = true, StatusMessage = "../Department/List" };
            var boVM = Newtonsoft.Json.JsonConvert.DeserializeObject<DepartmentVM>(jsonBoVM);

            if (ModelState.IsValid)
            {
                var bo = await _BoRepository.GetSingleAsyn(boVM.ID);
                if (bo == null)
                    bo = new Department();
                var parentID = Guid.Parse(boVM.ParentItemID);
                var parentItem = await _BoRepository.GetSingleAsyn(parentID);
                boVM.MapToBo(bo, parentItem);
                await _BoRepository.AddOrEditAndSaveAsyn(bo);
            }
            else
            {
            }
            return Json(saveStatus);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var status = await _BoRepository.DeleteAndSaveAsyn(id);
            return Json(status);
        }

    }
}
