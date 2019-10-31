using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.Entities;
using ShiKe.DataAccess;
using ShiKe.ViewModels;
using ShiKe.DataAccess.SqlServer.Utilities;


namespace ShiKe.Web.Controllers
{
    public class CommonEntityController<T> : Controller where T : class, IEntity, new()
    {
        private readonly IEntityRepository<T> _BoRepository;
        public string EntityTitle;
        public string ControllerInstanName;

        public CommonEntityController(IEntityRepository<T> repository)
        {
            _BoRepository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var boCollection = await _BoRepository.GetAllAsyn();
            var boVMCollection = new List<EntityVM>();
            var counter = 0;
            foreach(var item in boCollection.OrderBy(x=>x.SortCode))
            {
                var boVM = new EntityVM();
                boVM.SetVM(item);
                boVM.OrderNumber = (++counter).ToString();
                boVMCollection.Add(boVM);
            }

            ViewBag.EntityTitle = EntityTitle;
            ViewBag.ControllerInstanName = ControllerInstanName;

            return View("../../Views/CommonEntity/Index", boVMCollection);
        }

        public async Task<IActionResult> List()
        {
            var boCollection = await _BoRepository.GetAllAsyn();
            var boVMCollection = new List<EntityVM>();
            var counter = 0;
            foreach (var item in boCollection.OrderBy(x => x.SortCode))
            {
                var boVM = new EntityVM();
                boVM.SetVM(item);
                boVM.OrderNumber = (++counter).ToString();
                boVMCollection.Add(boVM);
            }

            ViewBag.EntityTitle = EntityTitle;
            ViewBag.ControllerInstanName = ControllerInstanName;

            return PartialView("../../Views/CommonEntity/_List", boVMCollection);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(Guid id)
        {
            var isNew = false;
            var bo = await _BoRepository.GetSingleAsyn(id);
            if (bo == null)
            {
                bo = new T();
                bo.Name = "";
                bo.Description = "";
                bo.SortCode = "";
                isNew = true;
            }

            var boVM = new EntityVM();
            boVM.SetVM(bo);
            boVM.IsNew = isNew;
            return PartialView("../../Views/CommonEntity/_CreateOrEdit", boVM);
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var bo = await _BoRepository.GetSingleAsyn(id);
            var boVM = new EntityVM();
            boVM.SetVM(bo);
            return PartialView("../../Views/CommonEntity/_Detail", boVM);
        }

        public async Task<IActionResult> Save(string jsonBoVM)
        {
            var saveStatus = new EditAndSaveStatus() { SaveOk = true, StatusMessage = "../"+ControllerInstanName+"/List" };
            // 将前端传入的 Json 数据转为 DepartmentVM 
            var boVM = Newtonsoft.Json.JsonConvert.DeserializeObject<EntityVM>(jsonBoVM);
            if (ModelState.IsValid)
            {
                var bo = await _BoRepository.GetSingleAsyn(boVM.ID);
                if (bo == null)
                    bo = new T();
                boVM.MapToBo(bo);
                await _BoRepository.AddOrEditAndSaveAsyn(bo);
                return Json(saveStatus);
            }
            else
            {
                return Json(saveStatus);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var status = await _BoRepository.DeleteAndSaveAsyn(id);
            return Json(status);
        }
    }
}
