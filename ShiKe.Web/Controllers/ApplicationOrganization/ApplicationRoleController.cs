using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.ViewModels.ApplicationOrganization;
using ShiKe.DataAccess.SqlServer.Utilities;

namespace ShiKe.Web.Controllers.ApplicationOrganization
{
    /// <summary>
    /// 系统角色组管理
    /// </summary>
    public class ApplicationRoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _RoleManager;
        private readonly UserManager<ApplicationUser> _UserManager;

        public ApplicationRoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _RoleManager = roleManager;
            _UserManager = userManager;
        }


        #region 角色管理部分
        /// <summary>
        /// 用户组管理入口
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var boCollection = _RoleManager.Roles;
            var boVMCollection = new List<ApplicationRoleVM>();
            var counter = 0;
            foreach (var item in boCollection.OrderBy(x => x.SortCode))
            {
                var boVM = new ApplicationRoleVM(item)
                {
                    OrderNumber = (++counter).ToString(),
                };
                var userInRole = from u in _UserManager.Users
                                 where u.Roles.Select(x => x.RoleId).Contains(item.Id)
                                 select u;
                boVM.UserAmount = userInRole.Count();
                boVMCollection.Add(boVM);
            }

            ViewBag.WhatItIs = "Roles";     // 用于为 _RoleAndUserLayout 判断调用那个 PartialView 使用的
            ViewBag.RolesItems = boVMCollection;

            return View("../../Views/ApplicationOrganization/ApplicationRole/Index", boVMCollection);
        }

        /// <summary>
        /// 角色组数据列表，用于处理更新过角色组刷新列表
        /// </summary>
        /// <returns></returns>
        public IActionResult List()
        {
            var boCollection = _RoleManager.Roles;
            var boVMCollection = new List<ApplicationRoleVM>();
            var counter = 0;
            foreach (var item in boCollection.OrderBy(x => x.SortCode))
            {
                var boVM = new ApplicationRoleVM(item)
                {
                    OrderNumber = (++counter).ToString(),
                };
                var userInRole = from u in _UserManager.Users
                                 where u.Roles.Select(x => x.RoleId).Contains(item.Id)
                                 select u;
                boVM.UserAmount = userInRole.Count();
                boVMCollection.Add(boVM);
            }

            return PartialView("../../Views/ApplicationOrganization/ApplicationRole/_NavigatorForRoles", boVMCollection);
        }

        /// <summary>
        /// 创建待编辑的角色组数据，返回到前端页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateOrEdit(Guid id)
        {
            var bo = await _RoleManager.FindByIdAsync(id.ToString());
            if (bo == null)
            {
                bo = new ApplicationRole();
            }
            var boVM = new ApplicationRoleVM(bo);
            return PartialView("../../Views/ApplicationOrganization/ApplicationRole/_CreateOrEdit", boVM);
        }

        /// <summary>
        /// 保存用户组数据
        /// </summary>
        /// <param name="boVM"></param>
        /// <returns></returns>
        public async Task<IActionResult> Save([Bind("ID,Name,DisplayName,Description,SortCode")]ApplicationRoleVM boVM)
        {
            if (ModelState.IsValid)
            {
                var bo = await _RoleManager.FindByIdAsync(boVM.ID.ToString());
                if (bo == null)
                {
                    bo = new ApplicationRole();
                    boVM.MapToBo(bo);
                    await _RoleManager.CreateAsync(bo);
                }
                else
                {
                    boVM.MapToBo(bo);
                    await _RoleManager.UpdateAsync(bo);
                }
                var saveStatus = new EditAndSaveStatus() { SaveOk = true, StatusMessage = "../../ApplicationRole/Index" };
                return Json(saveStatus);

            }
            else
                return PartialView("../../Views/ApplicationOrganization/ApplicationRole/_CreateOrEdit", boVM);

        }
        #endregion
    }
}
