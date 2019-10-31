using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShiKe.Entities.ApplicationOrganization
{
    /// <summary>
    /// 系统用户定义，这是直接继承 IdentityUser 实现的
    /// </summary>
    public class ApplicationUser : IdentityUser 
    {
        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(100)]
        public string ChineseFullName { get; set; }
        [StringLength(50)]
        public string MobileNumber { get; set; }           // 移动电话，父类中的 PhoneNumber 用于固定电话     
        
        public virtual Person Person { get; set; }         // 关联的实际个人
        public virtual BusinessImage Avatar { get; set; }  // 用户头像
        public string UserAddress { get; set; }               // 联系地址
        public string DistributionAddress { get; set; }       // 配送地址

        public ApplicationUser():base()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public ApplicationUser(string userName) : base(userName)
        {
            this.Id = Guid.NewGuid().ToString();
            this.UserName = userName;
        }
    }
}
