using ShiKe.Entities.ApplicationOrganization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShiKe.Entities.BusinessOrganization
{
    public class SK_WM_GoodsCollection:IEntityBase
    {
        public Guid ID { get; set; }
        public DateTime JionDateTime { get; set; }
        public virtual SK_WM_Goods Goods { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }    //关联的人员
        public SK_WM_GoodsCollection()
        {
            this.ID = Guid.NewGuid();
            JionDateTime = DateTime.Now;
        }
    }
}
