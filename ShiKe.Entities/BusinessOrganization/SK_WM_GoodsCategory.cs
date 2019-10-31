using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    /// 商品类别
    /// </summary>
    public class SK_WM_GoodsCategory:IEntity
    {
        [Key]
        public Guid ID { get; set; }//商品类别ID
        public string Name { get; set; }//商品类别名      
        public string Description { get; set; }
        public string SortCode { get; set; }
        
    }
}
