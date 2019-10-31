using ShiKe.Common.JsonModels;
using ShiKe.Common.ViewModelComponents;
using ShiKe.DataAccess.Utilities;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.BusinessOrganization
{
    public class SK_WM_GoodsCategoryVM : IEntityVM
    {
        public Guid ID { get; set; }
        public string OrderNumber { get; set; }
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }

        [Required(ErrorMessage = "名称不能为空值。")]
        [Display(Name = "类别名称")]
        [StringLength(10, ErrorMessage = "你输入的数据超出限制10个字符的长度。")]
        public string Name { get; set; }

        [Display(Name = "简要说明")]
        [StringLength(100, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        public string Description { get; set; }

        [Display(Name = "业务编码")]
        [Required(ErrorMessage = "类型业务编码不能为空值。")]
        [StringLength(150, ErrorMessage = "你输入的数据超出限制150个字符的长度。")]
        public string SortCode { get; set; }

        [Display(Name = "类别下的商品")]
        public string GoodsIDs { get; set; }

        [Display(Name = "类别下的商品")]
        public SK_WM_Goods Goods { get; set; }

        [Display(Name = "类别下的商品")]
        public ICollection<SK_WM_Goods> GoodsItems { get; set; }

        public SK_WM_GoodsCategoryVM()
        {
        
        }

        public SK_WM_GoodsCategoryVM(SK_WM_GoodsCategory bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            Description = bo.Description;
            SortCode = bo.SortCode;
            
        }

        public void MapToBo(SK_WM_GoodsCategory bo)
        {
            bo.Name = Name;
            bo.Description = Description;
            bo.SortCode = SortCode;
        }
    }
}