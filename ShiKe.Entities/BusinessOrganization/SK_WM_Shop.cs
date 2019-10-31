using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    /// 店铺实体
    /// </summary>
    public class SK_WM_Shop : IEntity
    {
        [Key]
        public Guid ID { get; set; }
        [StringLength(10)]
        public string Name { get; set; }                            //店铺名称
        [StringLength(200)]
        public string Description { get; set; }                     //店铺描述
        public string SortCode { get; set; }
        public decimal Grade { get; set; }                          //店铺评分
        public int Collection { get; set; }                         //店铺被收藏的统计       
        [StringLength(11)]
        public string Telephone { get; set; }                       //店铺电话
        public string Adress { get; set; }                          //店铺地址
        public DateTime SettledDateTime { get; set; }               //入驻的时间
        public string State { get; set; }                              //店铺状态
        public virtual BusinessImage ShopAvatar { get; set; }       //店铺头像

        public virtual BusinessImage ShopBanner { get; set; }       //店铺横幅

        public virtual ApplicationUser ShopForUser { get; set; }    //所属的用户
        public virtual SK_WM_ShopExecuteIllegal ShopForExecuteIllegal { get; set; }
        public string BelongToUserID { get; set; }
        
        
        public SK_WM_Shop()
        {
            this.SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<SK_WM_Shop>();
            this.ID = Guid.NewGuid();
            SettledDateTime = DateTime.Now;
        }

    }
}
