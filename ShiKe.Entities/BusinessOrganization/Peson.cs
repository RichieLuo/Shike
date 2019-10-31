using ShiKe.Entities.Attachments;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    /// 系统业务人员规约信息
    /// </summary>
    public class Person:IEntity
    {
        [Key]
        public Guid ID { get; set; }
        [StringLength(50)]
        public string Name { get; set; }                                    // 全名
        [StringLength(1000)]
        public string Description { get; set; }                             // 人员简要说明
        [StringLength(150)]
        public string SortCode { get; set; }                                // 系统内部编码
        public DateTime CreateDateTime { get; set; }                        // 创建日期
        public DateTime ExpiredDateTime { get; set; }
        public int Duration { get; set; }
        [StringLength(50)]
        public string EmployeeCode { get; set; }                            // 业务工号
        [StringLength(50)]
        public string FirstName { get; set; }                               // 姓氏
        [StringLength(50)]
        public string LastName { get; set; }                                // 名字
        public bool Sex { get; set; }                                       // 性别
        [StringLength(20)]
        public string TelephoneNumber { get; set; }                         // 电话号码
        [StringLength(20)]
        public string Mobile { get; set; }                            // 手机号码
        [StringLength(100)]
        public string Email { get; set; }                                   // 电子邮箱
        public DateTime Birthday { get; set; }                              // 出生日期
        [StringLength(26)]
        public string CredentialsCode { get; set; }                         // 身份证编号
        public DateTime UpdateTime { get; set; }                            // 信息更新时间
        [StringLength(50)]
        public string InquiryPassword { get; set; }                         // 查询密码，仅仅用于查询是否已经已经建立数据

        public virtual Department Department { get; set; }                  // 所属部门
        public virtual BusinessImage Avatar { get; set; }

        public Person()
        {
            ID = Guid.NewGuid();
            UpdateTime = CreateDateTime = Birthday = ExpiredDateTime = DateTime.Now;
            SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<Person>();
        }

    }
}
