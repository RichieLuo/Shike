using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.BusinessManagement.Audit;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.Entities.WebSettingManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShiKe.DataAccess.SqlServer
{
    public class EntityDbContext : IdentityDbContext<ApplicationUser>
    {
        public EntityDbContext(DbContextOptions<EntityDbContext> options) : base(options)
        {
        }

        #region 用户与角色相关
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        #endregion

        #region 用户工作区与菜单相关
        public DbSet<SystemWorkPlace> SystemWorkPlaces { get; set; }
        public DbSet<SystemWorkSection> SystemWorkSections { get; set; }
        public DbSet<SystemWorkTask> SystemWorkTasks { get; set; }
        #endregion

        #region 业务组织相关
        public DbSet<Person> Persons { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SK_WM_Goods> SK_WM_Goods { get; set; }
        public DbSet<SK_WM_GoodsCategory> SK_WM_GoodsCategory { get; set; }
        public DbSet<SK_WM_GoodsCollection> SK_WM_GoodsCollection { get; set; }
        public DbSet<SK_WM_Shop> SK_WM_Shops { get; set; }
        public DbSet<SK_WM_ShopSttled> SK_WM_ShopSttled { get; set; }
        public DbSet<SK_WM_ShopCar> SK_WM_ShopCar { get; set; }
        public DbSet<SK_WM_ShopCarGoodsItem> SK_WM_ShopCarGoodsItem { get; set; }
        public DbSet<SK_WM_Order> SK_WM_Order { get; set; }
        public DbSet<SK_WM_OrderItem> SK_WM_OrderItem { get; set; }
        public DbSet<SK_WM_ShopExecuteIllegal> SK_WM_ShopExecuteIllegal { get; set; }
        public DbSet<SK_SiteSettings> SK_SiteSettings { get; set; }
        #endregion

        #region 一些基础业务对象相关
        public DbSet<AuditRecord> AuditRecords { get; set; }
        public DbSet<BusinessFile> BusinessFiles { get; set; }
        public DbSet<BusinessImage> BusinessImages { get; set; }
        public DbSet<BusinessVideo> BusinessVideos { get; set; }
        #endregion


        /// <summary>
        /// 如果不需要 DbSet<T> 所定义的属性名称作为数据库表的名称，可以在下面的位置自己重新定义
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Person>().ToTable("Person");
            base.OnModelCreating(modelBuilder);

        }
    }
}
