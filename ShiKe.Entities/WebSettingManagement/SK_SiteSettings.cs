using ShiKe.Entities.Attachments;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.Entities.WebSettingManagement
{
    /// <summary>
    /// 网站信息实体
    /// </summary>
    public class SK_SiteSettings : IEntity
    {

        [Key]
        public Guid ID { get; set; }
        [StringLength(20)]
        public string Name { get; set; }//网站标题
        [StringLength(20)]
        public string Suffix { get; set; }//网站后缀
        [StringLength(20)]
        public string DomainName { get; set; }//网站域名
        public string KeyWords { get; set; }      //网站关键字
        public string Description { get; set; }//网站描述
        public string Statistics { get; set; }//网站统计代码
        public string SortCode { get; set; }
        public BusinessImage Logo { get; set; } //网站LOGO
        [StringLength(30)]
        public string Copyright { get; set; }// 网站版权
        [StringLength(30)]
        public string ICP { get; set; }// 备案号
        public SK_SiteSettings ()
        {
            this.SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<SK_SiteSettings>();
            this.ID= Guid.NewGuid();
        }
    }
}
