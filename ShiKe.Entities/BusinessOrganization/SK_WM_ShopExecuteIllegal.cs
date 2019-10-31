using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    /// 违规的店铺
    /// </summary>
    public class SK_WM_ShopExecuteIllegal : IEntity
    {
        [Key]
        public Guid ID { get; set; }                                          //ID
        public string Name { get; set; }                                      //名称
        public DateTime IllegalTime { get; set; }                             //创建时间
        public DateTime ModifyTime { get; set; }                              //修改时间
        public string Description { get; set; }                               //违规描述
        public string ShopState { get; set; }                                 //违规后的店铺状态
        public string IllegalCategory { get; set; }                           //违规类型
        public string SortCode { get; set; }                                  // 系统内部编码
        public string ShopID { get; set; }
        public SK_WM_ShopExecuteIllegal()
        {
            this.SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<SK_WM_Goods>();
            this.ID = Guid.NewGuid();
        }
    }

}
