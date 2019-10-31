using System;
using System.Collections.Generic;
using System.Text;

namespace ShiKe.Common.ViewModelComponents.ForMvcHelper
{
    /// <summary>
    /// 用于定义 Input 标签规格数据
    /// </summary>
    public class HtmlInputItemSpecification
    {
        public string ItemDisplayName { get; set; }   // 显示的名称
        public string ItemID { get; set; }            // 用于定义与 input 相关的 id、name 等具体的名称
        public string ItemValue { get; set; }         // 属性的值
        public string PlaceholderString { get; set; } // 用于在 intput 内部使用的提示内容
        public string OnfocusFuntion { get; set; }    // 用于在 input 获取焦点事件 onfocus 需要执行的函数，
                                                      // 例如：clearErrStyle(this.id)，clearErrStyle(\"abcd\")
        public string OnBlurFunction { get; set; }    // 用于在 input 失去焦点事件 onBlur 需要执行的函数
        public string AutofocusName { get; set; }     // 将当前的 input 设为焦点，它的值必须是：autofocus 才能生效

        public HtmlInputItemSpecification()
        {
            this.ItemDisplayName = "";
            this.ItemID = "";
            this.ItemValue = "";
            this.PlaceholderString = "";
            this.OnfocusFuntion = "";
            this.OnBlurFunction = "";
            this.AutofocusName = "";

        }
    }
}
