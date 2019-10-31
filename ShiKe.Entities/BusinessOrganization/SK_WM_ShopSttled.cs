using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShiKe.Entities.BusinessOrganization
{
   public class SK_WM_ShopSttled : IEntity
    {
        [Key]
        public Guid ID { get; set; }                                          //认证店铺ID
        public string Name { get; set; }                                      //店铺名称
         public bool IsNew { get; set; }
        public DateTime ShelvesTime { get; set; }                             //店铺认证时间
        public string MobilePhone { get; set; }                               //手机号
        public string Telephone { get; set; }
        public string Description { get; set; }                               //店铺描述
        public string SortCode { get; set; }                                  // 系统内部编码
        public string Address { get; set; }                                   //商品单位
        public string IDCar { get; set; }                                     //身份证
        public string LicenceID { get; set; }                                 //营业执照ID
        public string UserName { get; set; }                                  //认证人名字
        public int State { get; set; }                                        //认证状态
        public int Step { get; set; }                                         //所处步骤 用于提交时判断 例如0是处于基本信息填写 1是处于身份证正反面认证 2是处于店铺环境认证
        public virtual BusinessImage Licence { get; set; }                    //营业执照图片
        public virtual BusinessImage FrontIDCar { get; set; }                 //身份证正面
        public virtual BusinessImage BackIDCar { get; set; }                  //身份证背面
        public virtual BusinessImage Environment { get; set; }                //环境图
        public virtual SK_WM_Shop Shop { get; set; }                          //所属的店铺
        public string BelongToUserID { get; set; }                           //认证信息绑定的UserID
        public virtual ApplicationUser ShopForUser { get; set; }              //认证信息绑定的User
        public string BelongToExamineID { get; set; }                      //系统审核员ID
        public virtual ApplicationUser ShopForExamine { get; set; }             //系统审核员ID
        public SK_WM_ShopSttled()
        {
            this.SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<SK_WM_Goods>();
            this.ID = Guid.NewGuid();
            this.State = 0;
        }
    }
}
