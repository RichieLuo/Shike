using ShiKe.Common.JsonModels;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Entities.ApplicationOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShiKe.ViewModels.ApplicationManagement
{
    public class SystemWorkSectionVM : IEntityVM
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

        [Display(Name = "归属主菜单")]
        public string ParentItemID { get; set; }                        // 这个用于编辑、新建时候选择项的绑定属性
        public List<PlainFacadeItem> ParentItemCollection { get; set; } // 这个用于提供给前端实现时所需要的下拉或者列表选择的时候的元素

        [Display(Name = "归属主菜单")]
        public PlainFacadeItem ParentItem { get; set; }                 // 这个用于在明细或者列表时候呈现关联信息

        public List<SystemWorkTaskVM> SystemWorkTaskVMCollection { get; set; }
        public SystemWorkSectionVM()
        {
            IsNew = true;
        }

        public SystemWorkSectionVM(SystemWorkSection bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            Description = bo.Description;
            SortCode = bo.SortCode;
        }

        public void MapToBo(SystemWorkSection bo)
        {
            bo.Name = Name;
            bo.Description = Description;
            bo.SortCode = SortCode;
        }
    }
}