using ShiKe.Common.JsonModels;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.BusinessOrganization
{
    public class SK_WM_OrderItemVM : IEntityVM
    {
        [Key]
        public Guid ID { get; set; }
        public string OrderNumber { get; set; }
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SortCode { get; set; }          //订单编号
        public string GoodsName { get; set; }         //商品名称
        public string GoodsID { get; set; }            //商品ID
        public string ShopName { get; set; }          //店铺名称
        public int Count { get; set; }                  //商品购买数量
        public string Price { get; set; }        //商品单价
        public string TotalPrice { get; set; }          //总价
        public string DeliveryAdderss { get; set; }   //配送地址
        public decimal DeliveryFee { get; set; }       //配送费
        public string State { get; set; }                //订单状态
        public string ImgPath { get; set; }           //商品图片路径
        public DateTime CreateOrderTime { get; set; } //订单创建时间
        public SK_WM_Order ItemForOrder { get; set; } //订单列表项所属的订单

        public bool HasGoods { get; set; }
        public SK_WM_OrderItemVM(SK_WM_OrderItem bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            SortCode = bo.SortCode.Substring(16);
            GoodsName = bo.GoodsName;
            Description = bo.Description;
            GoodsID = bo.GoodsID;
            ShopName = bo.ShopName;
            Count = bo.Count;
            Price = bo.Price;
            TotalPrice = bo.TotalPrice;
            DeliveryAdderss = bo.DeliveryAdderss;
            DeliveryFee = bo.DeliveryFee;
            State = bo.State;
            CreateOrderTime = bo.CreateOrderTime;
            ItemForOrder = bo.ItemForOrder;
            ImgPath = bo.ImgPath;
            HasGoods = true;
        }
        public void MapToOrderItem(SK_WM_OrderItem bo)
        {
            bo.SortCode = SortCode;
            bo.GoodsName = GoodsName;
            bo.Description = Description;
            bo.GoodsID = GoodsID;
            bo.ShopName = ShopName;
            bo.Count = Count;
            bo.Price = Price;
            bo.TotalPrice = TotalPrice;
            bo.DeliveryAdderss = DeliveryAdderss;
            bo.DeliveryFee = DeliveryFee;
            bo.State = State;
            bo.CreateOrderTime = CreateOrderTime;
            bo.ItemForOrder = ItemForOrder;
            bo.ImgPath = ImgPath;
        }
    }
}
