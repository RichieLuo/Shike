using ShiKe.Common.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiKe.ViewModels
{
    /// <summary>
    /// 业务实体视图模型统一的接口规范，用于规约所有的视图模型的基础属性
    /// </summary>
    public interface IEntityVM
    {
        Guid ID { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string SortCode { get; set; }
        string OrderNumber { get; set; } // 列表时候需要的序号
        bool IsNew { get; set; }         // 是否是新创建的对象，要特别注意在实际使用时候的赋值

        ListPageParameter ListPageParameter { get; set; } // 页面参数规格
    }
}
