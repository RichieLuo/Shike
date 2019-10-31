using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShiKe.DataAccess.Utilities;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Common.JsonModels;
using ShiKe.Entities.BusinessOrganization;

namespace ShiKe.ViewModels.BusinessOrganization
{
    public class PersonVM : IEntityVM
    {
        [Key]
        public Guid ID { get; set; }
        public string OrderNumber { get; set; }
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "简要说明")]
        [StringLength(1000, ErrorMessage = "你输入的数据超出限制1000个字符的长度。")]
        public string Description { get; set; }

        public string SortCode { get; set; }

        //[Required(ErrorMessage = "必须选择人员归属部门。")]
        [Display(Name = "归属部门")]
        public string ParentItemID { get; set; }

        [Display(Name = "归属部门")]
        public SelfReferentialItem ParentItem { get; set; }

        [SelfReferentialItemSpecification("ParentItemID")]
        public List<SelfReferentialItem> ParentItemCollection { get; set; }

        [ListItemSpecification("<i class='icon-pictures'></i>", "01", 40, false)]
        [StringLength(50)]
        public string PersonPhotoPath { get; set; }

        //[Required(ErrorMessage = "工号不能为空值。")]
        [Display(Name = "工号")]
        [StringLength(20, ErrorMessage = "你输入的数据超出限制20个字符的长度。")]
        public string EmployeeCode { get; set; }

        //[Required(ErrorMessage = "姓氏不能为空值。")]
        [Display(Name = "姓氏")]
        [StringLength(6, ErrorMessage = "你输入的数据超出限制6个字符的长度。")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "名字不能为空值。")]
        [Display(Name = "名字")]
        [StringLength(6, ErrorMessage = "你输入的数据超出限制6个字符的长度。")]
        public string LastName { get; set; }

        [Display(Name = "性别")]
        public bool Sex { get; set; }

        [Display(Name = "性别")]
        [ListItemSpecification("性别", "04", 50, false)]
        public string SexString { get; set; }

        [PlainFacadeItemSpecification("Sex")]
        public List<PlainFacadeItem> SexSelector { get; set; }

        [Display(Name = "出生日期")]
        public DateTime Birthday { get; set; }

        [ListItemSpecification("出生日期", "08", 100, false)]
        [Display(Name = "出生日期")]
        public string BirthdayString { get; set; }

        [Display(Name = "固定电话")]
        [StringLength(20, ErrorMessage = "你输入的数据超出限制20个字符的长度。")]
        public string TelephoneNumber { get; set; }

        [Required(ErrorMessage = "移动电话不能为空值。")]
        [Display(Name = "移动电话")]
        [StringLength(20, ErrorMessage = "你输入的数据超出限制6个字符的长度。")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "电子邮件不能为空值。")]
        [Display(Name = "电子邮件")]
        [StringLength(150, ErrorMessage = "你输入的数据超出限制6个字符的长度。")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "非法的电子邮件格式。")]
        public string Email { get; set; }

        [Display(Name = "身份证件编号")]
        [StringLength(50, ErrorMessage = "你输入的数据超出限制50个字符的长度。")]
        public string CredentialsCode { get; set; }

        public PersonVM()
        {
            SexSelector = PlainFacadeItemFactory<Person>.GetBySex();
        }

        public PersonVM(Person bo)
        {
            ID              = bo.ID;
            Name            = bo.Name;
            Description     = bo.Description;
            SortCode        = bo.SortCode;
            EmployeeCode    = bo.EmployeeCode;
            FirstName       = bo.FirstName;
            LastName        = bo.LastName;
            Sex             = bo.Sex;
            TelephoneNumber = bo.TelephoneNumber;
            MobileNumber    = bo.Mobile;
            Email           = bo.Email;
            CredentialsCode = bo.CredentialsCode;
            Birthday        = bo.Birthday;
            BirthdayString  = bo.Birthday.ToString("yyyy-MM-dd");

            if (Birthday.Year == 1)
            {
                Birthday = DateTime.Now;
                BirthdayString = Birthday.ToString("yyyy-MM-dd");
            }

            SexString = bo.Sex ? "男" : "女";

            SexSelector = PlainFacadeItemFactory<Person>.GetBySex(bo.Sex);

            if (bo.Department != null)
            {
                ParentItemID = bo.Department.ID.ToString();
                ParentItem = new SelfReferentialItem
                    {
                        ID = bo.Department.ID.ToString(),
                        //ParentID = bo.Department.ParentDepartment.ID.ToString(),
                        DisplayName = bo.Department.Name,
                        SortCode = bo.Department.SortCode,
                        OperateFunction = "",
                        TargetType = "",
                        TipsString = ""
                    };
            }
        }

        public void MapToBo(Person bo)
        {
            bo.Name            = Name;
            bo.Description     = Description;
            bo.SortCode        = SortCode;
            bo.EmployeeCode    = EmployeeCode;
            bo.FirstName       = FirstName;
            bo.LastName        = LastName;
            bo.Sex             = Sex;
            bo.TelephoneNumber = TelephoneNumber;
            bo.Mobile          = MobileNumber;
            bo.Email           = Email;
            bo.CredentialsCode = CredentialsCode;
            bo.Birthday        = DateTime.Now;                //DateTime.Parse(BirthdayString);
        }
    }
}