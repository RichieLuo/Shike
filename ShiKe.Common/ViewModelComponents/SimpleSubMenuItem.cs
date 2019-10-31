using System;
using System.Collections.Generic;
using System.Text;

namespace ShiKe.Common.ViewModelComponents
{
    /// <summary>
    /// 用于简单约束管理桌面子侧菜单的的过渡性质的类
    /// </summary>
    public class SimpleSubMenuItem
    {
        public Guid ID { get; set; }
        public string Name { get; set; }     // 显示的名称
        public string URL { get; set; }      // 操作导航的
        public string SortCode { get; set; } // 编码
        public Guid ParentID { get; set; }   // 上层约束元素的ID
    }
}
