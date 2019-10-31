using ShiKe.Common.JsonModels;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.BusinessOrganization
{
    public class SK_WM_ShopVM : IEntityVM
    {
        [Key]
        public Guid ID { get; set; }
        public string OrderNumber { get; set; }
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }
        [Display(Name = "店铺名称")]
        public string Name { get; set; }                     //店铺名称
        [Display(Name = "店铺描述")]
        public string Description { get; set; }              //店铺描述
        public string SortCode { get; set; }
        [Display(Name = "店铺评分")]
        public decimal Grade { get; set; }                   //店铺评分
        [Display(Name = "店铺被收藏的统计")]
        public int Collection { get; set; }                  //店铺被收藏的统计       
        [Display(Name = "店铺电话")]
        [StringLength(11)]
        public string Telephone { get; set; }                //店铺电话
        [Display(Name = "店铺地址")]
        public string Adress { get; set; }                   //店铺地址
        [Display(Name = "入驻的时间")]
        public DateTime SettledDateTime { get; set; }        //入驻的时间
        [Display(Name = "状态")]
        public string State { get; set; }                      //店铺状态
        public string ShopAvatarPath { get; set; }           //店铺头像
        public string ShopBannerPath { get; set; }          //店铺横幅
        public string BelongToUserID { get; set; }          //所属的用户
        public virtual ApplicationUser ShopForUser { get; set; }    //所属的用户
        public virtual SK_WM_ShopExecuteIllegal ShoForExecuteIllegal { get; set; }


        public string waitPay { get; set; }
        public string waitSend { get; set; }
        public string waitReceipt { get; set; }
        //[Display(Name = "店铺下的商品")]
        //public string GoodsIDs { get; set; }

        //[Display(Name = "店铺下的商品")]
        //public SK_WM_Goods Goods { get; set; }

        //[Display(Name = "店铺下的商品")]
        //public ICollection<SK_WM_Goods> GoodsItems { get; set; }


        public SK_WM_ShopVM()
        { }

        public SK_WM_ShopVM(SK_WM_Shop bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            Description = bo.Description;
            SortCode = bo.SortCode;
            Grade = bo.Grade;
            Collection = bo.Collection;
            Telephone = bo.Telephone;
            Adress = bo.Adress;
            SettledDateTime = bo.SettledDateTime;
            State = bo.State;
            ShopForUser = bo.ShopForUser;
            ShoForExecuteIllegal = bo.ShopForExecuteIllegal;
            BelongToUserID = bo.BelongToUserID;
            if (bo.ShopAvatar != null)
            {
                ShopAvatarPath = bo.ShopAvatar.UploadPath;
            }
            if (bo.ShopBanner != null)
            {
                ShopBannerPath = bo.ShopBanner.UploadPath;
            }
        }
        public void MapToSh(SK_WM_Shop bo)
        {
            bo.ID = ID;
            bo.Name = Name;
            bo.Description = Description;
            bo.SortCode = SortCode;
            bo.Grade = Grade;
            bo.Collection = Collection;
            bo.Telephone = Telephone;
            bo.Adress = Adress;
            bo.SettledDateTime = SettledDateTime;
            bo.State = State;
            bo.ShopForExecuteIllegal = ShoForExecuteIllegal;
            bo.BelongToUserID = BelongToUserID;
        }
    }
}
