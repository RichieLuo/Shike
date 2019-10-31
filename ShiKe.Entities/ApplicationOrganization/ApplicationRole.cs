using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiKe.Entities.ApplicationOrganization
{
    public class ApplicationRole : IdentityRole
    {
        [StringLength(250)]
        public string DisplayName { get; set; }
        [StringLength(550)]
        public string Description { get; set; }
        [StringLength(50)]
        public string SortCode { get; set; }
        public ApplicationRoleTypeEnum ApplicationRoleType { get; set; }

        public virtual Department Department { get; set; }

        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
