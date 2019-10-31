using ShiKe.Common.JsonModels;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.BusinessOrganization
{
    public class SK_WM_OrderVM : IEntityVM
    {
        [Key]
        public Guid ID { get; set; }
        public string OrderNumber { get; set; }
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SortCode { get; set; }
        public int Count { get; set; }                                        //商品总数量
        public decimal CountPrice { get; set; }                               //商品总价格  
        public string State { get; set; }                        //订单状态
        public DateTime CreateOrderTime { get; set; }                        //订单创建时间
        public SK_WM_Goods Goods { get; set; }                               //对应的商品
        public virtual ApplicationUser OrderItemForUser { get; set; }         //绑定的User

        public SK_WM_OrderVM() { }
        public SK_WM_OrderVM(SK_WM_Order bo)
        {
            ID = bo.ID;
            Count = bo.Count;
            CountPrice = bo.TotalPrice;
            State = bo.State;
            CreateOrderTime = bo.CreateOrderTime;
            OrderItemForUser = bo.OrderForUser;
            Goods = bo.Goods;
            Name = bo.Name;
            SortCode = bo.SortCode;            

        }
        public void MapToOrder(SK_WM_Order bo)
        {
            bo.SortCode = SortCode;
            bo.Count = Count;
            bo.TotalPrice = CountPrice;
            bo.State = State;
            bo.CreateOrderTime = CreateOrderTime;
            bo.OrderForUser = OrderItemForUser;
            bo.Goods = Goods;
            Name = bo.Name;
            SortCode = bo.SortCode;

        }
    }
}
