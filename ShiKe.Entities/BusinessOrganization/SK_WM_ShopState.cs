using System;
using System.Collections.Generic;
using System.Text;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    /// 店铺状态枚举类 （已开启，已关闭，已禁用）
    /// </summary>
    public class SK_WM_ShopState
    {
        public enum ShopState
        {
            已开启,
            已关闭,
            已停封,
            待处理
        }
    }
}
