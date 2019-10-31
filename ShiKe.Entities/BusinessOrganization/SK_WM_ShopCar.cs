using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShiKe.Entities.BusinessOrganization
{
    /// <summary>
    /// 购物车实体
    /// </summary>
    public class SK_WM_ShopCar : IEntity
    {
        [Key]
        public Guid ID { get; set; }
        public string BelongToUserID { get; set; }
        public string SortCode { get; set; }
        public virtual ApplicationUser ShopCarForUser { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public SK_WM_ShopCar()
        {
            this.SortCode = BusinessEntityComponentsFactory.SortCodeByDefaultDateTime<SK_WM_Goods>();
            this.ID = Guid.NewGuid();
        }
    }
}
