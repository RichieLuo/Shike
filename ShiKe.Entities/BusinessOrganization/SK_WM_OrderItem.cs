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
    /// 订单项
    /// </summary>
    public class SK_WM_OrderItem:IEntity
    {
        [Key]
        public Guid ID { get; set; }                  //订单项ID
        public string Name { get; set; }
        public string Description { get; set; }
        public string SortCode { get; set; }          //订单编码
        public string OrderNumber { get; set; }      
        public string GoodsName { get; set; }         //商品名称
        public string GoodsID { get; set; }           //商品ID
        public string ShopName { get; set; }          //店铺名称
        public int Count { get; set; }                  //商品购买数量
        public string Price { get; set; }               //商品单价    
        public string TotalPrice { get; set; }         //总价
        public string ImgPath { get; set; }           //商品图片
        public string DeliveryAdderss { get; set; }   //配送地址
        public decimal DeliveryFee { get; set; }       //配送费
        public string State { get; set; }                //订单状态
        public DateTime CreateOrderTime { get; set; } //订单创建时间
        public SK_WM_Order ItemForOrder { get; set; } //订单列表项所属的订单
   

    public SK_WM_OrderItem()
        {          
            this.SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<SK_WM_OrderItem>();
            this.ID = Guid.NewGuid();
            
        }
        
    }
}
