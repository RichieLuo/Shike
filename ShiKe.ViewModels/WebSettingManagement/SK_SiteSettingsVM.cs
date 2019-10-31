using ShiKe.Common.JsonModels;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.WebSettingManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.WebSettingManagement
{
    public class SK_SiteSettingsVM : IEntityVM
    {
        [Key]
        public Guid ID { get; set; }
        public string OrderNumber { get; set; }
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }
        [Display(Name = "网站名称")]
        public string Name { get; set; }                     //网站名称
        [Display(Name = "网站后缀")]
        public string Suffix { get; set; }                   //网站后缀
        [Display(Name = "网站域名")]
        public string DomainName { get; set; }               //网站域名
        [Display(Name = "网站关键字")]
        public string KeyWords { get; set; }                 //网站关键字
        [Display(Name = "网站描述")]
        public string Description { get; set; }              //网站描述
        [Display(Name = "网站统计代码")]
        public string Statistics { get; set; }               //网站统计代码
        public BusinessImage Logo { get; set; }              //网站LOGO
        public string Copyright { get; set; }                // 网站版权
        public string ICP { get; set; }                      // 备案号

        public string SortCode { get; set; }

        public SK_SiteSettingsVM() { }
        public SK_SiteSettingsVM(SK_SiteSettings bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            Suffix = bo.Suffix;
            DomainName = bo.DomainName;
            KeyWords = bo.KeyWords;
            Description = bo.Description;
            Statistics = bo.Statistics;
            if (bo.Logo!=null)
            {
                Logo = bo.Logo;
            }          
            Copyright = bo.Copyright;
            ICP = bo.ICP;
        }
        public void MapToSiteSettings(SK_SiteSettings bo)
        {
            bo.ID = ID;
            bo.Name = Name;
            bo.Suffix = Suffix;
            bo.DomainName = DomainName;
            bo.KeyWords = KeyWords;
            bo.Description = Description;
            bo.Statistics = Statistics;
            bo.Logo = Logo;
            bo.Copyright = Copyright;
            bo.ICP = ICP;
        }
    }
}
