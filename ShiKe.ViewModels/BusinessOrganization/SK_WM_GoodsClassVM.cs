using ShiKe.Common.JsonModels;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.BusinessOrganization
{
    public class SK_WM_GoodsClassVM
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }//店铺名称
        public string Description { get; set; }//店铺描述
        public string SortCode { get; set; }//店铺评分
        

        public SK_WM_GoodsClassVM()
        { }
        public SK_WM_GoodsClassVM(SK_WM_GoodsCalss bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            Description = bo.Description;
            SortCode = bo.SortCode;
        }
        public void Goods(SK_WM_Goods bo)
        {
            bo.ID = ID;
            bo.Name = Name;
            bo.Description = Description;
            bo.SortCode = SortCode;
        }
    }
}
