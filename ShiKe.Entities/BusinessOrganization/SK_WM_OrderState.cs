using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    ///  订单状态枚举类 （已创建,已完成,未完成,待发货,退款中,已撤销）
    /// </summary>
    public class SK_WM_OrderState
    {
        public enum Orderstate {
            待付款,           
            配送中,
            待发货,
            待收货,
            商家已下架,
            已完成,
            未完成,
            退款中,
            已撤销
        }
    }
}
