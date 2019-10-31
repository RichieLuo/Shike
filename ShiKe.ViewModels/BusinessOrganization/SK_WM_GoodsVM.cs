using ShiKe.Common.JsonModels;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.BusinessOrganization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.BusinessOrganization
{
    public class SK_WM_GoodsVM : IEntityVM
    {
        [Key]
        public Guid ID { get; set; }
        public string OrderNumber { get; set; }
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }
        [Required(ErrorMessage = "请输入商品名")]
        public string Name { get; set; }
        [Required(ErrorMessage = "请输入商品描述")]
        public string Description { get; set; }
        public string SortCode { get; set; }

        [Display(Name = "上架日期")]
        [ListItemSpecification("上架日期", "08", 100, false)]
        public DateTime ShelvesTime { get; set; }   //商品上架时间
        [Display(Name = "修改日期")]
        [ListItemSpecification("修改日期", "08", 100, false)]
        public DateTime ModifyTime { get; set; }    //商品修改时间
        [Display(Name = "售价")]
        [Required(ErrorMessage = "请输入售价")]
        [RegularExpression(@"^[-+]?\d+(\.\d+)?$", ErrorMessage = "请输入正确的售价")]
        public string Price { get; set; }          //商品售价 
        [Required(ErrorMessage = "请输入售价")]
        [Display(Name = "实体店售价")]
        [RegularExpression(@"^[-+]?\d+(\.\d+)?$", ErrorMessage = "请输入正确的售价")]
        public string FacadePrice { get; set; }    //实体店售价
        [Required(ErrorMessage = "请输入商品单位")]
        [Display(Name = "单位")]
        public string Unit { get; set; }            //商品单位
        [Display(Name = "销量")]
        public string SalesVolume { get; set; }        //商品销售量
        [Required(ErrorMessage = "请输入商品库存")]
        [Display(Name = "库存")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "请输入正确的库存")]
        public string Stock { get; set; }              //商品库存
        [Display(Name = "状态")]
        public string State { get; set; }           //商品状态
                                                    //public SK_WM_GoodsCategory SK_WM_GoodsCategory { get; set; }  //所属分类


        [Display(Name = "所属分类")]
        public string GoodsCategoryID { get; set; }

        [Display(Name = "所属分类")]
        public PlainFacadeItem GoodsCategory { get; set; }

        //[SelfReferentialItemSpecification("GoodsCategoryID")]
        public List<PlainFacadeItem> GoodsCategoryCollection { get; set; }

        [Display(Name = "所属店铺")]
        public string ShopID { get; set; }
        [Display(Name = "所属店铺")]
        public SK_WM_Shop Shop { get; set; }
        public virtual ApplicationUser ShopForUser { get; set; }
        public string BelongToUserID { get; set; }

        ////[SelfReferentialItemSpecification("ShopID")]
        //public List<PlainFacadeItem> ShopCollection { get; set; }
        public string BelongToShopID { get; set; }
        public string AvatarPath { get; set; }

        public SK_WM_GoodsVM()
        {

        }
        public SK_WM_GoodsVM(SK_WM_Goods bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            Description = bo.Description;
            SortCode = bo.SortCode;
            ShelvesTime = bo.ShelvesTime;
            ModifyTime = bo.ModifyTime;
            // IMG = bo.IMG;
            SalesVolume = "0";
              Price = bo.Price;
            FacadePrice = bo.FacadePrice;
            Unit = bo.Unit;
            SalesVolume = bo.SalesVolume;
            State = bo.State;
            Stock = bo.Stock;
            ShopForUser = bo.ShopForUser;
            BelongToUserID = bo.BelongToUserID;
            BelongToShopID = bo.BelongToShopID;
            
            if (bo.GoodsIMG != null)
            {
                AvatarPath = bo.GoodsIMG.UploadPath;
            }
            if (bo.SK_WM_GoodsCategory != null)
            {
                GoodsCategoryID = bo.SK_WM_GoodsCategory.ID.ToString();
                GoodsCategory = new PlainFacadeItem
                {
                    ID = bo.SK_WM_GoodsCategory.ID.ToString(),
                    //ParentID = bo.Department.ParentDepartment.ID.ToString(),
                    DisplayName = bo.SK_WM_GoodsCategory.Name,
                    SortCode = bo.SK_WM_GoodsCategory.SortCode,
                    //OperateFunction = "",
                    TargetType = "",
                    //TipsString = ""
                };
            }
            if (bo.Shop != null)
            {
                Shop = bo.Shop;
                //ShopID = bo.Shop.ID.ToString();
                //Shop = new SK_WM_Shop
                //{
                //    ID = bo.ID,
                //    Name = bo,
                //    Description = bo.Description,
                //    SortCode = bo.SortCode,                   
                //    Telephone = "",
                //    Adress = ""
                //};
            }
        }
        public void MapToBo(SK_WM_Goods bo)
        {
            bo.Name = Name;
            bo.Description = Description;
            bo.SortCode = SortCode;
            bo.ShelvesTime = DateTime.Now;
            bo.ModifyTime = DateTime.Now;
           // bo.IMG = IMG;
            bo.Price = Price;
            bo.FacadePrice = FacadePrice;
            bo.Unit = Unit;
            bo.SalesVolume = SalesVolume;
            bo.State = State;
            bo.Stock = Stock;
            bo.BelongToShopID = BelongToShopID;
            bo.BelongToUserID = BelongToUserID;
            bo.Shop = Shop;
        }
    }
}
