using System;
using System.Collections.Generic;
using System.Text;
using ShiKe.Common.JsonModels;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.Entities.ApplicationOrganization;

namespace ShiKe.ViewModels.BusinessOrganization
{
    public class SK_WM_ShopCarGoodsItemVM : IEntityVM
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SortCode { get; set; }
        public string OrderNumber { get; set; }
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }
        public string GoodsID { get; set; }            //商品ID
        public DateTime CreateOrderTime { get; set; } //订单创建时间
        public string ShopName { get; set; }//店家名
        public string GoodsName { get; set; }//商品名
        public int Count { get; set; }//数量
        public string Price { get; set; }//单价
        public string TotalPrice { get; set; }//总价
        public string BelongToShopCarID { get; set; }

        public virtual SK_WM_ShopCar shopCar { get; set; } //归属购物车
        public virtual ApplicationUser ShopCarForUser { get; set; }//归属用户
        public string ImgPath { get; set; }
        public SK_WM_ShopCarGoodsItemVM(SK_WM_ShopCarGoodsItem bo)
        {
            ID = bo.ID;
            GoodsID = bo.GoodsID;
            SortCode = bo.SortCode;
            ShopName = bo.ShopName;
            GoodsName = bo.GoodsName;
            Count = bo.Count;
            Price = bo.Price;
            TotalPrice = bo.TotalPrice;
            BelongToShopCarID = bo.BelongToShopCarID;
            ShopCarForUser = bo.ShopCarForUser;
            ImgPath = bo.ImgPath;
             CreateOrderTime=bo.CreateOrderTime ;
        }
        public void MapToBo(SK_WM_ShopCarGoodsItem bo)
        {

            bo.ID = ID;
            bo.GoodsID = GoodsID;
            bo.SortCode = SortCode;
            bo.ShopName = ShopName;
            bo.GoodsName = GoodsName;
            bo.Count = Count;
            bo.Price = Price;
            bo.TotalPrice = TotalPrice;
            bo.BelongToShopCarID = BelongToShopCarID;
            bo.ShopCarForUser = ShopCarForUser;
            bo.ImgPath = ImgPath;
            bo.CreateOrderTime = CreateOrderTime;
        }
    }
}
