using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShiKe.Common.JsonModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShiKe.CustomMvcCoreExtension
{
    public static class PageCommonItemHelper
    {
        public static HtmlString ShiKeSetListPageParameter(this IHtmlHelper helper, ListPageParameter pageParameter)
        {
            var htmlContent = new StringBuilder();

            htmlContent.Append(
                "<input type='hidden' name='对应的类型' id='shikeTypeID' value='" + pageParameter.ObjectTypeID + "' />");
            htmlContent.Append(
                "<input type='hidden' name='当前页码' id='shikePageIndex' value='" + pageParameter.PageIndex + "' />");
            htmlContent.Append(
                "<input type='hidden' name='每页数据条数' id='shikePageSize' value='" + pageParameter.PageSize +
                    "' /> ");
            htmlContent.Append(
                "<input type='hidden' name='分页数量' id='shikePageAmount' value='" + pageParameter.PageAmount +
                    "' />");
            htmlContent.Append(
                "<input type='hidden' name='相关的对象的总数' id='shikeObjectAmount' value='" + pageParameter.ObjectAmount +
                    "' />");
            htmlContent.Append(
                "<input type='hidden' name='当前的检索关键词' id='shikeKeyword' value='" + pageParameter.Keyword + "' />");
            htmlContent.Append(
                "<input type='hidden' name='排序属性' id='shikeSortProperty' value='" + pageParameter.SortProperty +
                    "' /> ");
            htmlContent.Append(
                "<input type='hidden' name='排序方向' id='shikeSortDesc' value='" + pageParameter.SortDesc + "' />");
            htmlContent.Append(
                "<input type='hidden' name='当前焦点对象 ID' id='shikeSelectedObjectID' value='" +
                    pageParameter.SelectedObjectID + "' />");
            htmlContent.Append(
                "<input type='hidden' name='当前是否为检索' id='shikeIsSearch' value='" +
                    pageParameter.IsSearch + "' />");
            return new HtmlString(htmlContent.ToString());

        }
    }
}
