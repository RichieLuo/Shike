using System;
using System.Collections.Generic;
using System.Text;

namespace ShiKe.Common.ViewModelComponents
{
    /// <summary>
    /// 用于处理在分页页面时，向相应的 action，通常是List提交ajax格式的数据时的规约
    /// </summary>
    public class PaginationSpecification
    {
        public string TypeID { get; set; }           // 对应的类型ID
        public string PageIndex { get; set; }        // 当前页码
        public string PageSize { get; set; }         // 每页数据条数
        public string Keyword { get; set; }          // 当前的关键词
        public string SortProperty { get; set; }     // 排序属性
        public string SortDesc { get; set; }         // 排序方向，缺省值正向 Default，前端用开关方式转为逆向：Descend
        public string SelectedObjectID { get; set; } // 当前页面处理中处理的焦点对象 ID

        public PaginationSpecification()
        {
            TypeID = "";
            PageIndex = "1";
            PageSize = "18";
            Keyword = "";
            SortProperty = "SortCode";
            SortDesc = "Default";
            SelectedObjectID = "";
        }
    }
}
