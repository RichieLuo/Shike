using ShiKe.Common.JsonModels;
using ShiKe.Common.ViewModelComponents;
using ShiKe.DataAccess.Utilities;
using ShiKe.Entities.ApplicationOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShiKe.ViewModels.ApplicationManagement
{
    public class SystemWorkTaskVM : IEntityVM
    {
        public Guid ID { get; set; }
        public string OrderNumber { get; set; } // 列表时候需要的序号
        public bool IsNew { get; set; }         // 是否是新创建的对象，要特别注意在实际使用时候的赋值
        public ListPageParameter ListPageParameter { get; set; }

        [Required(ErrorMessage = "名称不能为空值。")]
        [Display(Name = "名称")]
        [StringLength(100, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        public string Name { get; set; }

        [Display(Name = "简要说明")]
        [StringLength(1000, ErrorMessage = "你输入的数据超出限制1000个字符的长度。")]
        public string Description { get; set; }

        [Display(Name = "业务编码")]
        [Required(ErrorMessage = "类型业务编码不能为空值。")]
        [StringLength(150, ErrorMessage = "你输入的数据超出限制150个字符的长度。")]
        public string SortCode { get; set; }

        [Display(Name = "菜单分区")]
        public string ParentItemID { get; set; }                            // 这个用于编辑、新建时候选择项的绑定属性
        public List<PlainFacadeItem> ParentItemCollection { get; set; }     // 这个用于提供给前端实现时所需要的下拉或者列表选择的时候的元素

        [Display(Name = "菜单分区")]
        public PlainFacadeItem ParentItem { get; set; }                     // 这个用于在明细或者列表时候呈现关联信息

        [Display(Name = "控制器名称")]
        [Required(ErrorMessage = "控制器名称不能为空值。")]
        [StringLength(100, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        public string ControllerName { get; set; }

        [Display(Name = "控制器方法")]
        [Required(ErrorMessage = "控制器方法不能为空值。")]
        [StringLength(100, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        public string ControllerMethod { get; set; }

        [Display(Name = "方法参数")]
        [StringLength(500, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        public string ControllerMethodParameter { get; set; }

        [Display(Name = "菜单图标")]
        [StringLength(100, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        public string IconName { get; set; }

        [Display(Name = "主业务实体")]
        [StringLength(100, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        public string BusinessEntityName { get; set; }

        [Display(Name = "受限于个人")]
        public bool IsForMeOnly { get; set; }

        public string IsForMeOnlyString { get; set; }

        [PlainFacadeItemSpecification("IsForMeOnly")]
        public List<PlainFacadeItem> IsForMeOnlySelector { get; set; }

        [Display(Name = "受限于部门")]
        public bool IsForMyDepartmentOnly { get; set; }

        public string IsForMyDepartmentOnlyString { get; set; }

        [PlainFacadeItemSpecification("IsForMyDepartmentOnly")]
        public List<PlainFacadeItem> IsForMyDepartmentOnlySelector { get; set; }

        [Display(Name = "受限于角色")]
        public bool IsForDefaultSystemRoleGroup { get; set; }

        public string IsForDefaultSystemRoleGroupString { get; set; }

        [PlainFacadeItemSpecification("IsForDefaultSystemRoleGroup")]
        public List<PlainFacadeItem> IsForDefaultSystemRoleGroupSelector { get; set; }

        [Display(Name = "显示在导航菜单")]
        public bool IsUsedInMenu { get; set; }

        public string IsUsedInMenuString { get; set; }

        [PlainFacadeItemSpecification("IsUsedInMenu")]
        public List<PlainFacadeItem> IsUsedInMenuSelector { get; set; }

        [Display(Name = "使用快捷链接")]
        public bool HasShortCutLinkItem { get; set; }

        public string HasShortCutLinkItemString { get; set; }

        [PlainFacadeItemSpecification("HasShortCutLinkItem")]
        public List<PlainFacadeItem> HasShortCutLinkItemSelector { get; set; }

        [Display(Name = "使用桌面瓷砖")]
        public bool HasTileLinkItem { get; set; }

        public string HasTileLinkItemString { get; set; }

        [PlainFacadeItemSpecification("HasTileLinkItem")]
        public List<PlainFacadeItem> HasTileLinkItemSelector { get; set; }

        public SystemWorkTaskVM()
        {
            IsForMeOnlySelector = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(false);
            IsForMyDepartmentOnlySelector = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(false);
            IsForDefaultSystemRoleGroupSelector = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(false);
            IsUsedInMenuSelector = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(true);
            HasShortCutLinkItemSelector = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(false);
            HasTileLinkItemSelector = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(false);
        }

        public SystemWorkTaskVM(SystemWorkTask bo)
        {
            ID                          = bo.ID;
            Name                        = bo.Name;
            Description                 = bo.Description;
            SortCode                    = bo.SortCode;
            ControllerName              = bo.ControllerName;
            ControllerMethod            = bo.ControllerMethod;
            ControllerMethodParameter   = bo.ControllerMethodParameter;
            BusinessEntityName          = bo.BusinessEntityName;
            IconName                    = bo.IconName;
            IsForMeOnly                 = bo.IsForMeOnly;
            IsForMyDepartmentOnly       = bo.IsForMyDepartmentOnly;
            IsForDefaultSystemRoleGroup = bo.IsForDefaultSystemRoleGroup;
            IsUsedInMenu                = bo.IsUsedInMenu;
            HasShortCutLinkItem         = bo.HasShortCutLinkItem;
            HasTileLinkItem             = bo.HasTileLinkItem;

            IsForMeOnlyString                 = bo.IsForMeOnly ? "是" : "否";
            IsForMyDepartmentOnlyString       = bo.IsForMyDepartmentOnly ? "是" : "否";
            IsForDefaultSystemRoleGroupString = bo.IsForDefaultSystemRoleGroup ? "是" : "否";
            IsUsedInMenuString                = bo.IsUsedInMenu ? "是" : "否";
            HasShortCutLinkItemString         = bo.HasShortCutLinkItem ? "是" : "否";
            HasTileLinkItemString             = bo.HasTileLinkItem ? "是" : "否";

            IsForMeOnlySelector                 = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(bo.IsForMeOnly);
            IsForMyDepartmentOnlySelector       = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(bo.IsForMyDepartmentOnly);
            IsForDefaultSystemRoleGroupSelector = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(bo.IsForDefaultSystemRoleGroup);
            IsUsedInMenuSelector                = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(bo.IsUsedInMenu);
            HasShortCutLinkItemSelector         = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(bo.HasShortCutLinkItem);
            HasTileLinkItemSelector             = PlainFacadeItemFactory<SystemWorkTask>.GetByBool(bo.HasTileLinkItem);
        }

        public void MapToBo(SystemWorkTask bo)
        {
            bo.Name                        = Name;
            bo.Description                 = Description;
            bo.SortCode                    = SortCode;
            bo.ControllerName              = ControllerName;
            bo.ControllerMethod            = ControllerMethod;
            bo.ControllerMethodParameter   = ControllerMethodParameter;
            bo.BusinessEntityName          = BusinessEntityName;
            bo.IsForMeOnly                 = IsForMeOnly;
            bo.IsForMyDepartmentOnly       = IsForMyDepartmentOnly;
            bo.IsForDefaultSystemRoleGroup = IsForDefaultSystemRoleGroup;
            bo.IsUsedInMenu                = IsUsedInMenu;
            bo.HasShortCutLinkItem         = HasShortCutLinkItem;
            bo.HasTileLinkItem             = HasTileLinkItem;
            bo.IconName                    = IconName;
        }
    }
}