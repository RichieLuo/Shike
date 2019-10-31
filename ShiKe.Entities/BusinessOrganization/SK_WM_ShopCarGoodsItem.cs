using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.Entities.BusinessOrganization
{
    public class SK_WM_ShopCarGoodsItem: IEntity
    {
        [Key]
        public Guid ID { get; set; }
        public string ShopName { get; set; }//店家名
        public string GoodsName { get; set; }//商品名
        public int Count { get; set; }//数量
        public string Price { get; set; }//单价
        public string TotalPrice { get; set; }//总价
        public string BelongToShopCarID { get; set; }
        public string SortCode { get; set; }
        public virtual SK_WM_ShopCar shopCar { get; set; } //归属购物车
        public virtual ApplicationUser ShopCarForUser { get; set; }//归属用户
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgPath { get; set; }
        public string GoodsID { get; set; }            //商品ID
        public DateTime CreateOrderTime { get; set; } //订单创建时间
        public SK_WM_ShopCarGoodsItem()
        {
            this.SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<SK_WM_ShopCarGoodsItem>();
            this.ID = Guid.NewGuid();
        }
    }
}
