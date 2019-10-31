using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.Text;
using ShiKe.Common.JsonModels;

namespace ShiKe.ViewModels.BusinessOrganization
{
   public class SK_WM_ShopCarVM: IEntityVM
    {
        public Guid ID { get; set; }
        public string BelongToUserID { get; set; }
        public string SortCode { get; set; }
        public virtual ApplicationUser ShopCarForUser { get; set; }
        public string Name { get ; set; }
        public string Description { get ; set ; }
        public string OrderNumber { get ; set; }
        public bool IsNew { get ; set ; }
        public ListPageParameter ListPageParameter { get ; set; }

        public SK_WM_ShopCarVM()
        { }

        public SK_WM_ShopCarVM(SK_WM_ShopCar bo)
        {
            ID = bo.ID;
            ShopCarForUser = bo.ShopCarForUser;
            BelongToUserID = bo.BelongToUserID;
            
        }
        public void MapToSh(SK_WM_ShopCar bo)
        {
            bo.ID = ID;
            bo.SortCode = SortCode;
            bo.BelongToUserID = BelongToUserID;
        }
    }
}
