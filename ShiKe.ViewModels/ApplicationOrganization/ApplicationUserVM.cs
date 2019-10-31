using ShiKe.Common.JsonModels;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Entities.ApplicationOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.ApplicationOrganization
{
    public class ApplicationUserVM : IEntityVM
    {
        public Guid ID { get; set; }
        public string OrderNumber { get; set; } // 列表时候需要的序号
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }




        [Required(ErrorMessage = "显示名不能为空。")]
        [Display(Name = "名称")]
        [StringLength(10, ErrorMessage = "显示名不能超过10个字符")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入用户名。")]
        [Display(Name = "用户名")]
        [StringLength(28, ErrorMessage = "用户名不能超过28个字符")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "用户名只能输入英文")]
        public string UserName { get; set; }

        public string ChineseFullName { get; set; }


        [Required(ErrorMessage = "请填写您的手机号码。")]
        [Display(Name = "手机")]
        [RegularExpression(@"^1[3|4|5|7|8][0-9]\d{4,8}$", ErrorMessage = "请输入正确的手机号码。")]
        public string MobileNumber { get; set; }


        [Required(ErrorMessage = "请填写您的电子邮件。")]
        [Display(Name = "电子邮件")]
        [StringLength(100, ErrorMessage = "你输入的数据超出限制100个字符的长度。")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "非法的电子邮件格式。")]
        public string EMail { get; set; }
        
        [Required(ErrorMessage = "必须给出密码。")]
        [Display(Name = "密码")]
        [StringLength(20, ErrorMessage = "密码应在6-20个字符之间。",MinimumLength = 6)]
        [RegularExpression(@"^(?![a-zA-z]+$)(?!\d+$)(?![!@#$%^&*]+$)(?![a-zA-z\d]+$)(?![a-zA-z!@#$%^&*]+$)(?![\d!@#$%^&*]+$)[a-zA-Z\d!@#$%^&*]+$", ErrorMessage = "密码组合至少包含数字、特殊符号(如'@')、大小写字母，并且不少于6位字符")]
        public string Password { get; set; }

        [Required(ErrorMessage = "必须给出重复密码。")]
        [Display(Name = "密码")]
        [Compare("Password", ErrorMessage = "密码和重复密码不匹配。")]
        [StringLength(20, ErrorMessage = "密码应在6-20个字符之间。。", MinimumLength = 6)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "归属用户组")]
        public List<string> RoleItemIDCollection { get; set; }
        [Display(Name = "归属用户组")]
        public string RoleItemNameString { get; set; }
        [PlainFacadeItemSpecification("RoleItemIDCollection")]
        public List<PlainFacadeItem> RoleItemColection { get; set; }

        [Display(Name = "简要说明")]
        [StringLength(1000, ErrorMessage = "你输入的数据超出限制1000个字符的长度。")]
        public string Description { get; set; }

        [Display(Name = "用户内部编码")]
        [StringLength(150, ErrorMessage = "你输入的数据超出限制150个字符的长度。")]
        public string SortCode { get; set; }

        [Display(Name = "归属部门")]
        public string DepartmentName { get; set; }
        
        [Display(Name = "关联人员")]
        public string PersonID { get; set; }
        [Display(Name = "关联人员")]
        public string PersonName { get; set; }
        [PlainFacadeItemSpecification("PersonID")]
        public List<PlainFacadeItem> PersonItemCollection { get; set; }

        public bool LockoutEnabled { get; set; }//用户被禁用状态
        public Guid RoleID { get; set; }         // 角色ID

        public string AvatarPath { get; set; }
        public string UserAddress { get; set; }               // 联系地址
        public string DistributionAddress { get; set; }       // 配送地址

        public ApplicationUserVM() { }

        public ApplicationUserVM(ApplicationUser bo)
        {
            this.ID = Guid.Parse(bo.Id);
            this.UserName = bo.UserName;
            this.MobileNumber = bo.MobileNumber;
            this.EMail = bo.Email;
            this.Name = bo.FirstName+bo.LastName;
            this.LockoutEnabled = bo.LockoutEnabled;
            this.ChineseFullName = bo.ChineseFullName;
            this.DistributionAddress = bo.DistributionAddress;
            this.UserAddress = bo.UserAddress;
            if (bo.Person != null)
            {
                this.PersonID = bo.Person.ID.ToString();
                this.PersonName = bo.Person.Name;

                if (bo.Person.Department != null)
                {
                    this.DepartmentName = bo.Person.Department.Name;
                }
            }

            //if(bo.)

        }

    }
}
