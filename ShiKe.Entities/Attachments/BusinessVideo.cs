using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiKe.Entities.Attachments
{
    public class BusinessVideo:IEntity
    {
        [Key]
        public Guid ID { get; set; }
        [StringLength(150)]
        public string Name { get; set; }                              
        [StringLength(1000)]
        public string Description { get; set; }                       
        [StringLength(250)]
        public string SortCode { get; set; }
        public DateTime AttachmentTimeUploaded { get; set; }          
        [StringLength(500)]
        public string OriginalFileName { get; set; }                  
        [StringLength(500)]
        public string UploadPath { get; set; }                        
        public bool IsInDB { get; set; }                              
        [StringLength(10)]
        public string UploadFileSuffix { get; set; }                  
        public byte[] BinaryContent { get; set; }                     
        public long FileSize { get; set; }                            
        [StringLength(120)]
        public string IconString { get; set; }                        

        public Guid RelevanceObjectID { get; set; }
        public Guid UploaderID { get; set; }                           // 关联上传人ID

        public BusinessVideo() 
        {
            this.ID = Guid.NewGuid();
            this.SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<BusinessVideo>();
            this.AttachmentTimeUploaded = DateTime.Now;
        }
    }
}
