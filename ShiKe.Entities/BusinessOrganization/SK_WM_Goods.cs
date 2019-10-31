using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    /// 商品实体
    /// </summary>
    public class SK_WM_Goods:IEntity
    {
        [Key]
        public Guid ID { get; set; }                                          //商品ID
        public string Name { get; set; }                                      //商品名称
        public DateTime ShelvesTime { get; set; }                             //商品上架时间
        public DateTime ModifyTime { get; set; }                              //商品修改时间
        public string Description { get; set; }                               //商品描述
        public string SortCode { get; set; }                                  // 系统内部编码
        public string Price { get; set; }                                     //商品售价       
        public string FacadePrice { get; set; }                               //实体店售价
        public string Unit { get; set; }                                      //商品单位
        public string SalesVolume { get; set; }                               //商品上架时间
        public string Stock { get; set; }                                     //商品库存
        public string State { get; set; }                                     //商品状态
        public virtual BusinessImage GoodsIMG { get; set; }                     //商品图片
        public virtual SK_WM_GoodsCategory SK_WM_GoodsCategory { get; set; }  //所属分类
        public virtual SK_WM_Shop Shop { get; set; }                          //所属的店铺
        public virtual ApplicationUser ShopForUser { get; set; }
        public string BelongToUserID { get; set; }
        public string BelongToShopID { get; set; }
        public SK_WM_Goods() {
            this.SortCode= BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<SK_WM_Goods>();
            this.ID = Guid.NewGuid();
        }


    }
}
