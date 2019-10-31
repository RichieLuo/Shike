using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.ApplicationOrganization
{
    public class ApplicationUserResetPasswordVM
    {
        [Required]
        public string ID { get; set; }   // 用户ID

        [Required(ErrorMessage = "必须给出密码。")]
        [Display(Name = "密码")]
        [StringLength(150, ErrorMessage = "你输入的数据超出限制150个字符的长度。")]
        public string Password { get; set; }

        [Required(ErrorMessage = "必须给出重复密码。")]
        [Display(Name = "密码")]
        [Compare("Password", ErrorMessage = "密码和重复密码不匹配。")]
        [StringLength(150, ErrorMessage = "你输入的数据超出限制150个字符的长度。")]
        public string ConfirmPassword { get; set; }

    }
}
