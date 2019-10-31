using ShiKe.Common.JsonModels;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Entities.ApplicationOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShiKe.ViewModels.ApplicationManagement
{
    public class SystemWorkPlaceVM : IEntityVM
    {
        public Guid ID { get; set; }
        public string OrderNumber { get; set; } // 列表时候需要的序号
        public bool IsNew { get; set; }
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

        [Display(Name = "菜单图标")]
        [StringLength(100, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        public string IconString { get; set; }

        [Display(Name = "链接URL")]
        [StringLength(250, ErrorMessage = "你输入的数据超出限制250个字符的长度。")]
        public string URL { get; set; } 

        public PlainFacadeItem ApplicationInfo { get; set; }
        public List<PlainFacadeItem> SystemWorkSetionItems { get; set; }

        public List<SystemWorkSectionVM> SystemWorkSectionVMCollection { get; set; }

        public SystemWorkPlaceVM()
        {
            IsNew = true;
        }

        public SystemWorkPlaceVM(SystemWorkPlace bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            Description = bo.Description;
            SortCode = bo.SortCode;
            IconString = bo.IconString;
            URL = bo.URL;
            if (bo.SystemWorkSections != null)
            {
                SystemWorkSetionItems = new List<PlainFacadeItem>();
                foreach (var item in bo.SystemWorkSections)
                {
                    var pItem = new PlainFacadeItem
                        {
                            ID = item.ID.ToString(),
                            Name = item.Name,
                            Description = item.Description,
                            SortCode = item.SortCode
                        };
                    SystemWorkSetionItems.Add(pItem);
                }
            }
        }

        public void MapToBo(SystemWorkPlace bo)
        {
            bo.Name = Name;
            bo.Description = Description;
            bo.SortCode = SortCode;
            bo.IconString = IconString;
            bo.URL = URL;
        }
    }
}