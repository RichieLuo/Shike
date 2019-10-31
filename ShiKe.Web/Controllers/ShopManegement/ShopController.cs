using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.ViewModels.ApplicationOrganization;
using Microsoft.AspNetCore.Identity;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.DataAccess;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.ViewModels.BusinessOrganization;
using ShiKe.Web.Controllers.ApplicationOrganization;

namespace ShiKe.Web.Controllers.ShopManegement
{
    public class ShopController : Controller
    {
        private readonly IEntityRepository<SK_WM_Goods> _BoRepository;

        public ShopController(IEntityRepository<SK_WM_Goods> repository)
        {
            _BoRepository = repository;
        }
        public async Task<IActionResult> Index()
        {
            var pageIndex = 1;
            var pageSize = 12;

            var boCollection = await _BoRepository.PaginateAsyn(pageIndex, pageSize, x => x.SortCode, null, x => x.SK_WM_GoodsCategory);
            var boVMCollection = new List<SK_WM_GoodsVM>();

            var orderNumber = 0;
            foreach (var bo in boCollection)
            {
                var boVM = new SK_WM_GoodsVM(bo);
                boVM.OrderNumber = (++orderNumber).ToString();
                boVMCollection.Add(boVM);
            }
            var user = User.Identity;
            ViewBag.UserLogonInformation = "";
            if (user != null)
            {
                ViewBag.UserLogonInformation = user.Name;
            }

            return View("Index", boVMCollection);
            
        }
    }
}