using ShiKe.Common.JsonModels;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.BusinessOrganization
{
    public class SK_WM_ShopSttledVM : IEntityVM
    {
        public Guid ID { get; set; }
        public string OrderNumber { get; set; }
        public ListPageParameter ListPageParameter { get; set; }
        public bool IsNew { get; set; }
        [Required(ErrorMessage = "请输入店铺名")]
        public string Name { get; set; }                                      //店铺名称
        [Required(ErrorMessage = "请输入店铺地址")]
        public string Address { get; set; }                                   //店铺地址
        [Required(ErrorMessage = "请输入店主名字")]
        public string UserName { get; set; }                                  //认证人名字
        [Required(ErrorMessage = "请输入身份证号码")]
        [RegularExpression(@"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$", ErrorMessage = "请输入正确的身份证号码")]
        public string IDCar { get; set; }                                     //身份证
        [Required(ErrorMessage = "请输入手机号")]
        [RegularExpression(@"^1[3|4|5|7|8][0-9]\d{4,8}$", ErrorMessage = "请输入正确的手机号码。")]
        public string MobilePhone { get; set; }                               //手机号
        [Required(ErrorMessage = "请输入固定电话")]
        public string Telephone { get; set; }
        [Required(ErrorMessage = "请输入营业执照号")]
        public string LicenceID { get; set; }


        public string Description { get; set; }                               //店铺描述
        public string SortCode { get; set; }                                  // 系统内部编码
        public DateTime ShelvesTime { get; set; }                             //店铺认证时间

        public virtual string LicencePath { get; set; }                    //营业执照图片
        public virtual string FrontIDCarPath { get; set; }                 //身份证正面
        public virtual string BackIDCarPath { get; set; }                  //身份证背面
        public virtual string EnvironmentPath { get; set; }                //环境图
        public int State { get; set; }                                     //认证状态
        public int Step { get; set; }        
        public virtual BusinessImage Licence { get; set; }                    //营业执照图片
        public virtual BusinessImage FrontIDCar { get; set; }                 //身份证正面
        public virtual BusinessImage BackIDCar { get; set; }                  //身份证背面
        public virtual BusinessImage Environment { get; set; }                //环境图
        public string BelongToUserID { get; set; }                           //认证信息绑定的UserID
        public virtual ApplicationUser ShopForUser { get; set; }
        public string BelongToExamineID { get; set; }                      //系统审核员ID
        public virtual ApplicationUser ShopForExamine { get; set; }             //系统审核员ID
        public SK_WM_ShopSttledVM()
        { }

        public SK_WM_ShopSttledVM(SK_WM_ShopSttled bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            Description = bo.Description;
            SortCode = bo.SortCode;
            ShelvesTime = bo.ShelvesTime;
            MobilePhone = bo.MobilePhone;
            Address = bo.Address;
            IDCar = bo.IDCar;
            LicenceID = bo.LicenceID;
            UserName = bo.UserName;
            State = bo.State;
            Step = bo.Step;
            ShopForUser = bo.ShopForUser;
            BelongToUserID = bo.BelongToUserID;            
            ShopForExamine = bo.ShopForExamine;
            BelongToExamineID = bo.BelongToExamineID;
            IsNew = bo.IsNew;
            if (bo.Licence != null)
            {
                LicencePath = bo.Licence.UploadPath;
            }
            if (bo.FrontIDCar != null)
            {
                FrontIDCarPath = bo.FrontIDCar.UploadPath;
            }
            if (bo.BackIDCar != null)
            {
                BackIDCarPath = bo.BackIDCar.UploadPath;
            }
            if (bo.Environment != null)
            {
                EnvironmentPath = bo.Environment.UploadPath;
            }
        }

        public void MapToShop(SK_WM_ShopSttled bo)
        {
            bo.Name = Name;
            bo.Description = Description;
            bo.SortCode = SortCode;
            bo.ShelvesTime = DateTime.Now;
            bo.MobilePhone = MobilePhone;
            bo.Address = Address;
            bo.IDCar = IDCar;
            bo.LicenceID = LicenceID;
            bo.UserName = UserName;
            bo.State = State;
            bo.Step = Step;
        }

    }
}
