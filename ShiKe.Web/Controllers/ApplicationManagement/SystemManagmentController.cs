using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShiKe.Web.Controllers.ApplicationManagement
{
    public class SystemManagmentController : Controller
    {
        public IActionResult Index()
        {
            return View("../../Views/ApplicationManagement/SystemManagment/Index");
        }
    }
}
