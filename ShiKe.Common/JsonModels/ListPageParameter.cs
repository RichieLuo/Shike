using System.Collections.Generic;

namespace ShiKe.Common.JsonModels
{
    /// <summary>
    /// 用于处理在分页页面时，向相应的 action，通常是List提交ajax格式的数据时的规约
    /// </summary>
    public class ListPageParameter
    {
        public string ObjectTypeID { get; set; }           // 对应的归属类型ID
        public int PageIndex { get; set; }                 // 当前页码
        public int PageSize { get; set; }                  // 每页数据条数 为"0"时显示所有
        public int PageAmount { get; set; }                // 相关对象列表分页处理分页数量
        public int ObjectAmount { get; set; }              // 相关的对象的总数
        public string Keyword { get; set; }                // 当前的关键词
        public string SortProperty { get; set; }           // 排序属性
        public string SortDesc { get; set; }               // 排序方向，缺省值正向 Default，前端用开关方式转为逆向：Descend
        public string SelectedObjectID { get; set; }       // 当前页面处理中处理的焦点对象 ID
        public bool IsSearch { get; set; }                 // 当前是否为检索

        public List<PagenateStringListItem> PagenateStringList { get; set; }  // 根据分页的条件值，提取用于页脚的分页控件的显示和链接参数
        private int PagenateStringListAmount = 8;                             // 在分页器中显示直接导航页的数量        

        public bool HasPreviousPage // 是否有前一页
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage     // 是否有后一页
        {
            get
            {
                return (PageIndex < PageAmount);
            }
        }

        public ListPageParameter()
        {
            ObjectTypeID = "";
            PageIndex = 1;
            PageSize = 18;
            Keyword = "";
            SortProperty = "SortCode";
            SortDesc = "default";
            SelectedObjectID = "";
            IsSearch = false;
            PageAmount = 1;
            ObjectAmount = 0;
        }

        public ListPageParameter(int pageIndex, int pageSize, int pageAmount, int objectAmount)
        {
            ObjectTypeID = "";
            PageIndex = pageIndex;
            PageSize = pageSize;
            Keyword = "";
            SortProperty = "SortCode";
            SortDesc = "default";
            SelectedObjectID = "";
            IsSearch = false;
            PageAmount = pageAmount;
            ObjectAmount = objectAmount;

        }


        /// <summary>
        /// 截取在分页器中显示直接导航页的元素
        /// 1.如果 （当前页+PagenateStringListAmount）< PageAmount 则 PagenateStringList截取的最后一个是 PageAmount
        /// </summary>
        private void SetPagenateStringList(int pageIndex)
        {

            #region 1.如果 （当前页+PagenateStringListAmount）< PageAmount 则 PagenateStringList截取的最后一个是 PageAmount

            #endregion

            if (!HasPreviousPage && !HasNextPage)
            {
                PagenateStringList = new List<PagenateStringListItem>();
                var firstItem = new PagenateStringListItem() { ItemType = PagenateStringListItemType.首页, InUse = false, Value = "1" };

                var pageIndexItems = new List<PagenateStringListItem>();
                for (int i = 1; i < PageAmount + 1; i++)
                {
                    var pageIndexItem = new PagenateStringListItem() { ItemType = PagenateStringListItemType.页码, InUse = true, Value = i.ToString() };
                    pageIndexItems.Add(pageIndexItem);
                }
            }

        }
    }

    public class PagenateStringListItem
    {
        public PagenateStringListItemType ItemType { get; set; }   // 分页元素类型
        public bool InUse { get; set; }
        public string Value { get; set; }                          // 值
    }

    public enum PagenateStringListItemType { 首页, 前一批, 页码, 后一批, 末页, 分隔符 }
}