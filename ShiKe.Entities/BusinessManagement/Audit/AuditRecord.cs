using ShiKe.Entities.BusinessOrganization;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.Entities.BusinessManagement.Audit
{
    /// <summary>
    /// 审核操作的记录
    /// </summary>
    public class AuditRecord:IEntity
    {
        [Key]
        public Guid ID { get; set; }
        [StringLength(50)]
        public string Name { get; set; }                 // 审核名称，由调用这个审核的实体类命名
        [StringLength(1000)]
        public string Description { get; set; }          // 审核意见
        [StringLength(150)]
        public string SortCode { get; set; }

        public Guid ObjectID { get; set; }               // 关联的业务对象的ID
        public AuditResultEnum AuditResult { get; set; } // 审核结果
        public DateTime AuditDateTime { get; set; }      // 审核时间
        public virtual Person Auditor { get; set; }      // 审核人

        public AuditRecord()
        {
            ID = Guid.NewGuid();
            SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<AuditRecord>();
            AuditDateTime = DateTime.Now;
        }
    }
}
