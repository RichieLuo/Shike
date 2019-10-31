using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.DataAccess;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.ViewModels.ApplicationManagement;
using ShiKe.DataAccess.SqlServer.Utilities;
using ShiKe.Web.Utilities;

namespace ShiKe.Web.Controllers.ApplicationManagement
{
    /// <summary>
    /// 系统菜单执行环境配置管理：直接管理 SystemWorkPlace、SystemWorkSection、SystemWorkTask 三个业务实体
    /// 这是一个比较大型的控制器应用范例，一般不提倡这样构建过于庞大的控制器。
    /// </summary>
    public class SystemConfigController : Controller
    {
        private readonly IEntityRepository<SystemWorkPlace> _BoRepository;
        private readonly IEntityRepository<SystemWorkSection> _WorkSectionRepository;

        public SystemConfigController(IEntityRepository<SystemWorkPlace> repository, IEntityRepository<SystemWorkSection> workSectionRepository)
        {
            _BoRepository = repository;
            _WorkSectionRepository = workSectionRepository;
        }

        public IActionResult Index()
        {
            var boCollection = _BoRepository.GetAllIncluding(x=>x.SystemWorkSections);
            var boVMCollection = new List<SystemWorkPlaceVM>();
            foreach(var item in boCollection.OrderBy(x=>x.SortCode))
            {
                var boVM = new SystemWorkPlaceVM(item);
                boVM.SystemWorkSectionVMCollection = new List<SystemWorkSectionVM>();
                foreach(var sItem in item.SystemWorkSections.OrderBy(y => y.SortCode))
                {
                    var workSectionVM = new SystemWorkSectionVM(sItem);
                    workSectionVM.SystemWorkTaskVMCollection = new List<SystemWorkTaskVM>();
                    var workSectionBo = _WorkSectionRepository.GetSingle(sItem.ID, x => x.SystemWorkTasks);
                    var wtCounter = 0;
                    foreach(var tItem in workSectionBo.SystemWorkTasks)
                    {
                        var worktaskVM = new SystemWorkTaskVM(tItem);
                        worktaskVM.OrderNumber = (++wtCounter).ToString();
                        workSectionVM.SystemWorkTaskVMCollection.Add(worktaskVM);
                    }
                    boVM.SystemWorkSectionVMCollection.Add(workSectionVM);
                }
                boVMCollection.Add(boVM);
            }
            return View("../../Views/ApplicationManagement/SystemConfig/Index",boVMCollection);
        }

        public IActionResult CreateOrEditForSystemWorkPlace(Guid id)
        {
            var isNew = false;
            var bo = _BoRepository.GetSingle(id);
            if (bo == null)
            {
                bo = new SystemWorkPlace();
                bo.Name = "";
                bo.Description = "";
                bo.SortCode = "";
                bo.URL = "";
                isNew = true;
            }

            var boVM = new SystemWorkPlaceVM(bo);
            boVM.IsNew = isNew;
            return PartialView("../../Views/ApplicationManagement/SystemConfig/_CreateOrEditForSystemWorkPlace", boVM);
        }

        public IActionResult CreateOrEditForSystemWorkSection(Guid id,Guid systemWorkPlaceID)
        {
            var isNew = false;
            var bo = _BoRepository.GetSingle(systemWorkPlaceID,x=>x.SystemWorkSections).SystemWorkSections.Where(x=>x.ID==id).FirstOrDefault();
            if (bo == null)
            {
                bo = new SystemWorkSection();
                bo.Name = "";
                bo.Description = "";
                bo.SortCode = "";
                isNew = true;
            }
            var workPlace = _BoRepository.GetSingle(systemWorkPlaceID);
            var boVM = new SystemWorkSectionVM(bo);
            boVM.ParentItemID = workPlace.ID.ToString();
            boVM.ParentItem = new Common.ViewModelComponents.PlainFacadeItem() { DisplayName = workPlace.Name, Name = workPlace.Name, ID = workPlace.ID.ToString() };

            boVM.IsNew = isNew;
            return PartialView("../../Views/ApplicationManagement/SystemConfig/_CreateOrEditForSystemWorkSection", boVM);
        }

        public IActionResult CreateOrEditForSystemWorkTask(Guid id, Guid systemWorkSectionID)
        {
            var isNew = false;
            var bo = _WorkSectionRepository.GetSingle(systemWorkSectionID, x => x.SystemWorkTasks).SystemWorkTasks.Where(x => x.ID == id).FirstOrDefault();
            if (bo == null)
            {
                bo = new SystemWorkTask();
                bo.Name = "";
                bo.Description = "";
                bo.SortCode = "";
                isNew = true;
            }
            var workSection = _WorkSectionRepository.GetSingle(systemWorkSectionID);
            var boVM = new SystemWorkTaskVM(bo);
            boVM.ParentItemID = workSection.ID.ToString();
            boVM.ParentItem = new Common.ViewModelComponents.PlainFacadeItem() { DisplayName = workSection.Name, Name = workSection.Name, ID = workSection.ID.ToString() };
            boVM.IsNew = isNew;
            return PartialView("../../Views/ApplicationManagement/SystemConfig/_CreateOrEditSystemWorkTask", boVM);
        }

        public IActionResult SaveSystemWorkPlace([Bind("ID,Name,URL,Description,SortCode")]SystemWorkPlaceVM boVM)
        {
            if (ModelState.IsValid)
            {
                var bo = _BoRepository.GetSingle(boVM.ID);
                if (bo == null)
                    bo = new SystemWorkPlace();
                boVM.MapToBo(bo);
                _BoRepository.AddOrEditAndSave(bo);

                MenuItemCollection.UpdateMainTopMenuItem(bo);

                var saveStatus = new EditAndSaveStatus() { SaveOk = true, StatusMessage = "../../SystemConfig/Index" };
                return Json(saveStatus);
            }
            else
            {
                return PartialView("../../Views/ApplicationManagement/SystemConfig/_CreateOrEditForSystemWorkPlace", boVM);
            }
        }

        public IActionResult SaveSystemWorkSection([Bind("ID,IsNew,ParentItemID,Name,Description,SortCode")]SystemWorkSectionVM boVM)
        {
            if (ModelState.IsValid)
            {
                var systemWorkPlaceID = Guid.Parse(boVM.ParentItemID);
                var workPlace = _BoRepository.GetSingle(systemWorkPlaceID);

                var bo = _BoRepository.GetSingle(systemWorkPlaceID, x => x.SystemWorkSections).SystemWorkSections.Where(x => x.ID == boVM.ID).FirstOrDefault();
                if (bo == null)
                {
                    bo = new SystemWorkSection();
                    boVM.MapToBo(bo);
                    workPlace.SystemWorkSections.Add(bo);
                    _BoRepository.EditAndSave(workPlace);
                }
                else
                {
                    boVM.MapToBo(bo);
                    _BoRepository.EntitiesContext.SystemWorkSections.Update(bo);
                    _BoRepository.EntitiesContext.SaveChanges();
                }
                var saveStatus = new EditAndSaveStatus() { SaveOk = true, StatusMessage = "../../SystemConfig/Index" };
                return Json(saveStatus);
            }
            else
            {
                return PartialView("../../Views/ApplicationManagement/SystemConfig/_CreateOrEditForSystemWorkSection", boVM);
            }
        }

        public IActionResult SaveSystemWorkTask([Bind("ID,IsNew,ParentItemID,Name,Description,SortCode,ControllerName,ControllerMethod,ControllerMethodParameter,BusinessEntityName,IsUsedInMenuString")]SystemWorkTaskVM boVM)
        {
            if (ModelState.IsValid)
            {
                var systemWorkSectionID = Guid.Parse(boVM.ParentItemID);
                var systemSection = _WorkSectionRepository.GetSingle(systemWorkSectionID);
                var bo = _WorkSectionRepository.GetSingle(systemWorkSectionID, x => x.SystemWorkTasks).SystemWorkTasks.Where(x => x.ID == boVM.ID).FirstOrDefault();
                if (bo == null)
                {
                    bo = new SystemWorkTask();
                    boVM.MapToBo(bo);
                    systemSection.SystemWorkTasks.Add(bo);
                    _WorkSectionRepository.EditAndSave(systemSection);
                }
                else
                {
                    boVM.MapToBo(bo);
                    _WorkSectionRepository.EntitiesContext.SystemWorkTasks.Update(bo);
                    _WorkSectionRepository.EntitiesContext.SaveChanges();
                }
                var saveStatus = new EditAndSaveStatus() { SaveOk = true, StatusMessage = "../../SystemConfig/Index" };
                return Json(saveStatus);
            }
            else
            {
                return PartialView("../../Views/ApplicationManagement/SystemConfig/_CreateOrEditForSystemWorkTask", boVM);
            }
        }

        public IActionResult DeleteSystemWorkPlace(Guid id)
        {
            var status = _BoRepository.DeleteAndSave(id);
            return Json(status);
        }

        public IActionResult DeleteSystemWorkSection(Guid id)
        {
            var status = _WorkSectionRepository.DeleteAndSave(id);
            return Json(status);
        }

        public IActionResult DeleteSystemWorkTask(Guid id)
        {
            var bo = _WorkSectionRepository.EntitiesContext.SystemWorkTasks.Find(id);
            var status = new DataAccess.SqlServer.Ultilities.DeleteStatusModel() { DeleteSatus = true, Message = "数据删除成功" };
            try
            {
                _WorkSectionRepository.EntitiesContext.SystemWorkTasks.Remove(bo);
                _WorkSectionRepository.EntitiesContext.SaveChanges();
            }
            catch
            {
                status.DeleteSatus = false;
                status.Message = "删除操作出现意外，主要原因是关联数据没有处理干净活者是其他原因。";
            }
            return Json(status);
        }
    }
}
