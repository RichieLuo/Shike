using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    /// 用户订单实体
    /// </summary>
    public class SK_WM_Order :IEntity
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SortCode { get; set; }
        public int Count { get; set; }                                        //商品总数量
        public decimal TotalPrice { get; set; }                               //商品总价格 
        public string State { get; set; }                       //订单状态
        public DateTime CreateOrderTime { get; set; }                        //订单创建时间
        public SK_WM_Goods Goods { get; set; }                               //对应的商品
        public virtual ApplicationUser OrderForUser { get; set; }            //绑定的User

        //public virtual IQueryable<SK_WM_OrderItem> SK_WM_OrderItem { get; set; }
        public SK_WM_Order() {
            this.SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<SK_WM_Order>();
            this.ID = Guid.NewGuid();
        }

    }
}
