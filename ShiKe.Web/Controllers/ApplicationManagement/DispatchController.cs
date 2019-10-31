using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiKe.Web.Utilities;

namespace ShiKe.Web.Controllers.ApplicationManagement
{
    public class DispatchController : Controller
    {
        public IActionResult Index(Guid id,string targetUrl)
        {
            MenuItemCollection.CurrentMainTopMenuItemID = id;
            if (String.IsNullOrEmpty(targetUrl))
                targetUrl = "../Home";
            return Redirect(targetUrl);
        }
    }
}
