using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShiKe.DataAccess.SqlServer;
using ShiKe.DataAccess;
using ShiKe.DataAccess.SqlServerr;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.DataAccess.Seeds;
using Microsoft.EntityFrameworkCore;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Web.Utilities;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.WebSettingManagement;

namespace ShiKe.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 添加 EF Core 框架
            services.AddDbContext<EntityDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("ShiKe.Common")));
            //services.AddDbContext<EntityDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // 添加微软自己的用户登录令牌资料
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<EntityDbContext>()
                .AddDefaultTokenProviders();

            // Add framework services.
            services.AddMvc();

            // 配置 Identity
            services.Configure<IdentityOptions>(options =>
            {
                // 密码策略的常规设置
                options.Password.RequireDigit = true;            // 是否需要数字字符
                options.Password.RequiredLength = 6;             // 必须的长度
                options.Password.RequireNonAlphanumeric = true;  // 是否需要非拉丁字符，如%，@ 等
                options.Password.RequireUppercase = false;        // 是否需要大写字符
                options.Password.RequireLowercase = false;        // 是否需要小写字符

                // 登录尝试锁定策略
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // Cookie 设置
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(150);
                options.Cookies.ApplicationCookie.LoginPath = "/Home/Logon";   // 缺省的登录路径
                options.Cookies.ApplicationCookie.LogoutPath = "/Home/LogOut"; // 注销以后的路径

                // 其它的一些设置
                options.User.RequireUniqueEmail = true;
            });

            #region 域控制器相关的依赖注入服务清单
            services.AddTransient<IEntityRepository<SystemWorkPlace>, EntityRepository<SystemWorkPlace>>();
            services.AddTransient<IEntityRepository<SystemWorkSection>, EntityRepository<SystemWorkSection>>();
            services.AddTransient<IEntityRepository<SystemWorkTask>, EntityRepository<SystemWorkTask>>();
            services.AddTransient<IEntityRepository<BusinessImage>, EntityRepository<BusinessImage>>();
            services.AddTransient<IEntityRepository<Person>, EntityRepository<Person>>();
            services.AddTransient<IEntityRepository<Department>, EntityRepository<Department>>();
            services.AddTransient<IEntityRepository<SK_WM_Goods>, EntityRepository<SK_WM_Goods>>();
            services.AddTransient<IEntityRepository<SK_WM_GoodsCategory>, EntityRepository<SK_WM_GoodsCategory>>();
            services.AddTransient<IEntityRepository<SK_WM_Shop>, EntityRepository<SK_WM_Shop>>();
            services.AddTransient<IEntityRepository<SK_WM_ShopSttled>, EntityRepository<SK_WM_ShopSttled>>();
            services.AddTransient<IEntityRepository<SK_WM_ShopCar>, EntityRepository<SK_WM_ShopCar>>();
            services.AddTransient<IEntityRepository<SK_WM_ShopCarGoodsItem>, EntityRepository<SK_WM_ShopCarGoodsItem>>();
            services.AddTransient<IEntityRepository<SK_WM_Order>, EntityRepository<SK_WM_Order>>();
            services.AddTransient<IEntityRepository<SK_WM_OrderItem>, EntityRepository<SK_WM_OrderItem>>();
            services.AddTransient<IEntityRepository<SK_WM_ShopExecuteIllegal>, EntityRepository<SK_WM_ShopExecuteIllegal>>();
            services.AddTransient<IEntityRepository<SK_SiteSettings>, EntityRepository<SK_SiteSettings>>(); 
                services.AddTransient<IEntityRepository<SK_WM_GoodsCollection>, EntityRepository<SK_WM_GoodsCollection>>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, EntityDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            DbInitializer.Initialize(context);
            MenuItemCollection.Initializer(context);
        }
    }
}
