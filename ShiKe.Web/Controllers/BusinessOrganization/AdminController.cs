using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ShiKe.Common.JsonModels;
using Microsoft.AspNetCore.Identity;
using ShiKe.Entities.ApplicationOrganization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShiKe.Web.Controllers.BusinessOrganization
{
    public class AdminController : Controller
    {

        private readonly RoleManager<ApplicationRole> _RoleManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;

        public AdminController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _RoleManager = roleManager;
            _UserManager = userManager;
            _SignInManager = signInManager;

        }
        /// <summary>
        /// 系统管理员登录界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Logon()
        {
            return View("../../Views/BusinessOrganization/AdminBG/Logon");
        }

        /// <summary>
        /// 系统管理员登录界面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logon(string jsonLogonInformation)
        {
            var logonVM = Newtonsoft.Json.JsonConvert.DeserializeObject<LogonInformation>(jsonLogonInformation);
            var user = await _UserManager.FindByNameAsync(logonVM.UserName);
            if (user != null)
            {
                var result = await _SignInManager.PasswordSignInAsync(logonVM.UserName, logonVM.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    //判断当前登录用户是否为管理员
                    if (!await _UserManager.IsInRoleAsync(user, "Admin"))
                    {                        
                        return Json(new { result = true, isAdminRole = false, message = "对不起，您不是系统管理员，请勿进行登录操作！<a href='../../Account/Personal'>点此进入我的个人中心</a>" });
                    }
                    else
                    {
                        // 下面的登录成功后的导航应该具体依赖用户所在的角色组来进行处理的。
                        var returnUrl = Url.Action("Index", "AdminBG");
                        return Json(new { result = true, isAdminRole = true, message = returnUrl });
                    }
                }
                else
                {
                    return Json(new { result = false, message = "用户名或密码错误，请检查后重新处理。" });
                }
            }
            else
                return Json(new { result = false, message = "无法执行登录操作，请仔细检查后重新处理。" });

        }
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _SignInManager.SignOutAsync();
            return RedirectToAction("Logon", "Admin");
        }


    
    }
}
