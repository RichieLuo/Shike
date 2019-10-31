using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    /// 商品标签
    /// </summary>
    public class SK_WM_GoodsTag
    {
        [Key]
        public Guid ID { get; set; }//商品标签ID
        public string Name { get; set; }//商品标签名
        //public virtual ICollection<SK_WM_Goods> SK_WM_Goods { get; set; }
    }
}
