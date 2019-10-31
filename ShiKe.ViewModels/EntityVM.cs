using ShiKe.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels
{
    /// <summary>
    /// 完全继承自 IEntity 的实体类共用的视图模型
    /// </summary>
    public class EntityVM
    {
        public Guid ID { get; set; }
        public string OrderNumber { get; set; } // 列表时候需要的序号
        public bool IsNew { get; set; }

        [Display(Name = "名称")]
        [StringLength(100, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        public string Name { get; set; }

        [Display(Name = "简要说明")]
        [StringLength(1000, ErrorMessage = "你输入的数据超出限制1000个字符的长度。")]
        public string Description { get; set; }

        [Display(Name = "角色编码")]
        [StringLength(150, ErrorMessage = "你输入的数据超出限制150个字符的长度。")]
        public string SortCode { get; set; }

        public EntityVM() { }

        public void SetVM<T>(T bo) where T : class, IEntity, new()
        {
            ID = bo.ID;
            Name = bo.Name;
            Description = bo.Description;
            SortCode = bo.SortCode;
        }

        public void MapToBo<T>(T bo) where T : class, IEntity, new()
        {
            bo.Name = this.Name;
            bo.Description = this.Description;
            bo.SortCode = this.SortCode;
        }
    }
}
