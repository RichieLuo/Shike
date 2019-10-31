using ShiKe.DataAccess.SqlServer;
using ShiKe.Entities.ApplicationOrganization;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.BusinessOrganization;
using ShiKe.Entities.WebSettingManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiKe.DataAccess.Seeds
{
    /// <summary>
    /// 构建一个初始化原始数据的组件，用于程序启动的时候执行一些数据初始化的操作
    /// </summary>
    public static class DbInitializer
    {
        static EntityDbContext _Context;

        public static void Initialize(EntityDbContext context)
        {
            _Context = context;
            context.Database.EnsureCreated(); //如果创建了，则不会重新创建

            _AddApplicationRole();
            _SetWorkPlace();
            _PersonAndDepartment();
            _AddPerson();
            _AddShopGoodsAndGoodsCategory();
        }
        private static void _AddApplicationRole()
        {
            if (_Context.ApplicationRoles.Any())
                return;
            var roles = new List<ApplicationRole>()
            {
               //new ApplicationRole(){Name="Admin",DisplayName="系统管理人员", Description="适用于系统管理人员",ApplicationRoleType=ApplicationRoleTypeEnum.适用于系统管理人员,SortCode="69a5f56g" },
               new ApplicationRole(){Name="Maintain",DisplayName="业务数据维护人员", Description="适用于业务数据维护人员",ApplicationRoleType=ApplicationRoleTypeEnum.适用于业务数据维护人员,SortCode="49aaf56g" },
               new ApplicationRole(){Name="AverageUser",DisplayName="普通注册用户", Description="适用于普通注册用户",ApplicationRoleType=ApplicationRoleTypeEnum.适用于普通注册用户 ,SortCode="99avf56g"}
            };

            foreach (var role in roles)
            {
                _Context.ApplicationRoles.Add(role);
            }
            _Context.SaveChanges();
        }

        private static void _SetWorkPlace()
        {
            var wp01 = new SystemWorkPlace() { Name = "系统管理", Description = "", SortCode = "wp01", IconString = "mif-cog" };

            var ws01 = new SystemWorkSection() { Name = "角色用户", Description = "", SortCode = "wp01ws01" };
            var ws02 = new SystemWorkSection() { Name = "导航菜单", Description = "", SortCode = "wp01ws02" };

            var wt0101 = new SystemWorkTask() { Name = "系统角色管理", Description = "", SortCode = "wp01ws01wt001", IconName = "mif-tools", BusinessEntityName = "ApplicationRole", ControllerName = "ApplicationRole", ControllerMethod = "", ControllerMethodParameter = "" };
            var wt0102 = new SystemWorkTask() { Name = "系统用户管理", Description = "", SortCode = "wp01ws01wt002", IconName = "mif-user-3", BusinessEntityName = "ApplicationUser", ControllerName = "ApplicationUser", ControllerMethod = "", ControllerMethodParameter = "" };

            ws01.SystemWorkTasks = new List<SystemWorkTask>();
            ws01.SystemWorkTasks.Add(wt0101);
            ws01.SystemWorkTasks.Add(wt0102);

            var wt0201 = new SystemWorkTask() { Name = "通用菜单配置管理", Description = "", SortCode = "wp01ws01wt001", IconName = "mif-tools", BusinessEntityName = "SystemConfig", ControllerName = "SystemConfig", ControllerMethod = "", ControllerMethodParameter = "" };
            ws02.SystemWorkTasks = new List<SystemWorkTask>();
            ws02.SystemWorkTasks.Add(wt0201);

            wp01.SystemWorkSections = new List<SystemWorkSection>();
            wp01.SystemWorkSections.Add(ws01);
            wp01.SystemWorkSections.Add(ws02);
            _Context.SystemWorkPlaces.Add(wp01);

            _Context.SaveChanges();

        }
        private static void _PersonAndDepartment()
        {
            if (_Context.Departments.Any())
                return;
            var dept01 = new Department() { Name = "总经办", Description = "", SortCode = "01" };
            var dept02 = new Department() { Name = "综合管理办公室", Description = "", SortCode = "02" };
            var dept03 = new Department() { Name = "开发部", Description = "", SortCode = "03" };
            var dept04 = new Department() { Name = "营运部", Description = "", SortCode = "04" };
            var dept0401 = new Department() { Name = "客户响应服务组", Description = "", SortCode = "0401" };
            var dept0402 = new Department() { Name = "客户需求分析组", Description = "", SortCode = "0402" };
            var dept0403 = new Department() { Name = "应用设计开发组", Description = "", SortCode = "0403" };
            var dept05 = new Department() { Name = "市场部", Description = "", SortCode = "05" };
            var dept06 = new Department() { Name = "品管部", Description = "", SortCode = "06" };
            var dept0601 = new Department() { Name = "营运部驻场服务组", Description = "", SortCode = "0601" };
            var dept0602 = new Department() { Name = "开发部驻场服务组", Description = "", SortCode = "0602" };

            dept01.ParentDepartment = dept01;
            dept02.ParentDepartment = dept02;
            dept03.ParentDepartment = dept03;
            dept04.ParentDepartment = dept04;
            dept0401.ParentDepartment = dept04;
            dept0402.ParentDepartment = dept04;
            dept0403.ParentDepartment = dept04;
            dept05.ParentDepartment = dept05;
            dept06.ParentDepartment = dept06;
            dept0601.ParentDepartment = dept06;
            dept0602.ParentDepartment = dept06;

            var depts = new List<Department>() { dept01, dept02, dept03, dept04, dept0401, dept0402, dept0403, dept05, dept06, dept0601, dept0602 };
            foreach (var item in depts)
                _Context.Departments.Add(item);
            _Context.SaveChanges();

            if (_Context.Persons.Any())
            {
                return;
            }

            var persons = new List<Person>()
            {
                new Person() { Name="刘虎军", Email="Liuhj@qq.com", Mobile="15107728899", SortCode="01001", Description="请补充个人简介", Department=dept01 },
                new Person() { Name="魏小花", Email="weixh@163.com", Mobile="13678622345", SortCode="01002", Description="请补充个人简介",Department=dept02 },
                new Person() { Name="李文慧", Email="liwenhui@tom.com", Mobile="13690251923", SortCode="01003", Description="请补充个人简介",Department=dept02 },
                new Person() { Name="张江的", Email="zhangjd@msn.com", Mobile="13362819012", SortCode="01004", Description="请补充个人简介",Department=dept03 },
                new Person() { Name="萧可君", Email="xiaokj@qq.com", Mobile="13688981234", SortCode="01005", Description="请补充个人简介",Department=dept03 },
                new Person() { Name="魏铜生", Email="weitsh@qq.com", Mobile="18398086323", SortCode="01006", Description="请补充个人简介",Department=dept03 },
                new Person() { Name="刘德华", Email="liudh@icloud.com", Mobile="13866225636", SortCode="01007", Description="请补充个人简介",Department=dept03 },
                new Person() { Name="魏星亮", Email="weixl@liuzhou.com", Mobile="13872236091", SortCode="01008", Description="请补充个人简介",Department=dept04 },
                new Person() { Name="潘家富", Email="panjf@guangxi.com", Mobile="13052366213", SortCode="01009", Description="请补充个人简介",Department=dept0401 },
                new Person() { Name="黎温德", Email="liwende@qq.com", Mobile="13576345509", SortCode="01010", Description="请补充个人简介",Department=dept0401 },
                new Person() { Name="邓淇升", Email="dengqsh@qq.com", Mobile="13709823456", SortCode="01011", Description="请补充个人简介" ,Department=dept0402},
                new Person() { Name="谭冠希", Email="tangx@live.com", Mobile="18809888754", SortCode="01012", Description="请补充个人简介" ,Department=dept0403},
                new Person() { Name="陈慧琳", Email="chenhl@live.com", Mobile="13172038023", SortCode="01013", Description="请补充个人简介" ,Department=dept05},
                new Person() { Name="祁华钰", Email="qihy@qq.com", Mobile="15107726987", SortCode="01014", Description="请补充个人简介" ,Department=dept06},
                new Person() { Name="胡德财", Email="hudc@qq.com", Mobile="13900110988", SortCode="01015", Description="请补充个人简介" ,Department=dept0601},
                new Person() { Name="吴富贵", Email="wufugui@hotmail.com", Mobile="15087109921", SortCode="01016", Description="请补充个人简介" ,Department=dept0602}
            };

            foreach (var person in persons)
            {
                _Context.Persons.Add(person);
            }
            _Context.SaveChanges();
        }

        private static void _AddPerson()
        {
            var dept = _Context.Departments.FirstOrDefault(x => x.Name == "开发部");
            var persons = new List<Person>()
            {
                new Person() { Name="黄虎军", Email="Liuhj@qq.com", Mobile="15107728899", SortCode="01001", Description="请补充个人简介", Department=dept },
                new Person() { Name="河小花", Email="weixh@163.com", Mobile="13678622345", SortCode="01002", Description="请补充个人简介",Department=dept },
                new Person() { Name="陆文慧", Email="liwenhui@tom.com", Mobile="13690251923", SortCode="01003", Description="请补充个人简介",Department=dept },
                new Person() { Name="刘江的", Email="zhangjd@msn.com", Mobile="13362819012", SortCode="01004", Description="请补充个人简介",Department=dept },
                new Person() { Name="韦可君", Email="xiaokj@qq.com", Mobile="13688981234", SortCode="01005", Description="请补充个人简介",Department=dept },
                new Person() { Name="韦铜生", Email="weitsh@qq.com", Mobile="18398086323", SortCode="01006", Description="请补充个人简介",Department=dept },
                new Person() { Name="韦德华", Email="liudh@icloud.com", Mobile="13866225636", SortCode="01007", Description="请补充个人简介",Department=dept },
                new Person() { Name="蒋星亮", Email="weixl@liuzhou.com", Mobile="13872236091", SortCode="01008", Description="请补充个人简介",Department=dept },
                new Person() { Name="蒋家富", Email="panjf@guangxi.com", Mobile="13052366213", SortCode="01009", Description="请补充个人简介",Department=dept },
                new Person() { Name="张温德", Email="liwende@qq.com", Mobile="13576345509", SortCode="01010", Description="请补充个人简介",Department=dept },
                new Person() { Name="张淇升", Email="dengqsh@qq.com", Mobile="13709823456", SortCode="01011", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="秦冠希", Email="tangx@live.com", Mobile="18809888754", SortCode="01012", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="刘慧琳", Email="chenhl@live.com", Mobile="13172038023", SortCode="01013", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="周华钰", Email="qihy@qq.com", Mobile="15107726987", SortCode="01014", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="钱德财", Email="hudc@qq.com", Mobile="13900110988", SortCode="01015", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="孙富贵", Email="wufugui@hotmail.com", Mobile="15087109921", SortCode="01016", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="韦虎军", Email="Liuhj@qq.com", Mobile="15107728899", SortCode="01001", Description="请补充个人简介", Department=dept },
                new Person() { Name="韦小花", Email="weixh@163.com", Mobile="13678622345", SortCode="01002", Description="请补充个人简介",Department=dept },
                new Person() { Name="韦文慧", Email="liwenhui@tom.com", Mobile="13690251923", SortCode="01003", Description="请补充个人简介",Department=dept },
                new Person() { Name="韦江的", Email="zhangjd@msn.com", Mobile="13362819012", SortCode="01004", Description="请补充个人简介",Department=dept },
                new Person() { Name="温可君", Email="xiaokj@qq.com", Mobile="13688981234", SortCode="01005", Description="请补充个人简介",Department=dept },
                new Person() { Name="温铜生", Email="weitsh@qq.com", Mobile="18398086323", SortCode="01006", Description="请补充个人简介",Department=dept },
                new Person() { Name="温德华", Email="liudh@icloud.com", Mobile="13866225636", SortCode="01007", Description="请补充个人简介",Department=dept },
                new Person() { Name="温星亮", Email="weixl@liuzhou.com", Mobile="13872236091", SortCode="01008", Description="请补充个人简介",Department=dept },
                new Person() { Name="温家富", Email="panjf@guangxi.com", Mobile="13052366213", SortCode="01009", Description="请补充个人简介",Department=dept },
                new Person() { Name="覃温德", Email="liwende@qq.com", Mobile="13576345509", SortCode="01010", Description="请补充个人简介",Department=dept },
                new Person() { Name="覃淇升", Email="dengqsh@qq.com", Mobile="13709823456", SortCode="01011", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="覃冠希", Email="tangx@live.com", Mobile="18809888754", SortCode="01012", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="覃慧琳", Email="chenhl@live.com", Mobile="13172038023", SortCode="01013", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="覃华钰", Email="qihy@qq.com", Mobile="15107726987", SortCode="01014", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="覃德财", Email="hudc@qq.com", Mobile="13900110988", SortCode="01015", Description="请补充个人简介" ,Department=dept},
                new Person() { Name="覃富贵", Email="wufugui@hotmail.com", Mobile="15087109921", SortCode="01016", Description="请补充个人简介" ,Department=dept}
            };

            foreach (var person in persons)
            {
                _Context.Persons.Add(person);
            }
            _Context.SaveChanges();
        }

        private static void _AddShopGoodsAndGoodsCategory()
        {
            #region 网站的基本信息
            if (_Context.SK_SiteSettings.Any())
                return;
            var siteSettings = new SK_SiteSettings() { Name = "食刻", Suffix = "美食每刻", DomainName = "www.shike.com", KeyWords = "美食,外卖,点餐", Logo = null, Description = "美食与每刻，原来饭还可以这么吃", Copyright = "版权归美食每刻所有", ICP = "这里填写ICP网站备案号", Statistics = "这里填写网站统计代码" };
            _Context.SK_SiteSettings.Add(siteSettings);
            _Context.SaveChanges();
            #endregion

            #region 用于处理店铺违规或正常使用
            if (_Context.SK_WM_ShopExecuteIllegal.Any())
                return;
            var shEI1 = new SK_WM_ShopExecuteIllegal() { ModifyTime = DateTime.Now, ShopState = (SK_WM_ShopState.ShopState.已开启).ToString(), IllegalCategory = "", Description = "正常使用" };
            var shEI2 = new SK_WM_ShopExecuteIllegal() { ModifyTime = DateTime.Now, ShopState = (SK_WM_ShopState.ShopState.已开启).ToString(), IllegalCategory = "", Description = "正常使用" };
            var shEI3 = new SK_WM_ShopExecuteIllegal() { ModifyTime = DateTime.Now, ShopState = (SK_WM_ShopState.ShopState.已开启).ToString(), IllegalCategory = "", Description = "正常使用" };
            var shEI4 = new SK_WM_ShopExecuteIllegal() { ModifyTime = DateTime.Now, ShopState = (SK_WM_ShopState.ShopState.已开启).ToString(), IllegalCategory = "", Description = "正常使用" };
            var shEI5 = new SK_WM_ShopExecuteIllegal() { ModifyTime = DateTime.Now, ShopState = (SK_WM_ShopState.ShopState.已开启).ToString(), IllegalCategory = "", Description = "正常使用" };
            var shEIs = new List<SK_WM_ShopExecuteIllegal>() { shEI1, shEI2, shEI3, shEI4, shEI5 };
            foreach (var shei in shEIs)
            {
                _Context.SK_WM_ShopExecuteIllegal.Add(shei);
            }
            _Context.SaveChanges();
            #endregion

            #region 店铺
            if (_Context.SK_WM_Shops.Any())
                return;
            var sh1 = new SK_WM_Shop() { Name = "食刻1号店", Telephone = "15578806785", Adress = "社湾路柳州职业技术学院1号店", Description = "店铺描述暂无", State = (SK_WM_ShopState.ShopState.已开启).ToString(), ShopForExecuteIllegal = shEI1 };
            var sh2 = new SK_WM_Shop() { Name = "食刻2号店", Telephone = "15578806785", Adress = "社湾路柳州职业技术学院2号店", Description = "店铺描述暂无", State = (SK_WM_ShopState.ShopState.已开启).ToString(), ShopForExecuteIllegal = shEI2 };
            var sh3 = new SK_WM_Shop() { Name = "食刻3号店", Telephone = "15578806785", Adress = "社湾路柳州职业技术学院3号店", Description = "店铺描述暂无", State = (SK_WM_ShopState.ShopState.已开启).ToString(), ShopForExecuteIllegal = shEI3 };
            var sh4 = new SK_WM_Shop() { Name = "食刻4号店", Telephone = "15578806785", Adress = "社湾路柳州职业技术学院4号店", Description = "店铺描述暂无", State = (SK_WM_ShopState.ShopState.已开启).ToString(), ShopForExecuteIllegal = shEI4 };
            var sh5 = new SK_WM_Shop() { Name = "食刻5号店", Telephone = "15578806785", Adress = "社湾路柳州职业技术学院5号店", Description = "店铺描述暂无", State = (SK_WM_ShopState.ShopState.已开启).ToString(), ShopForExecuteIllegal = shEI5 };
            var shops = new List<SK_WM_Shop>() { sh1, sh2, sh3, sh4, sh5 };
            foreach (var shop in shops)
            {
                _Context.SK_WM_Shops.Add(shop);
            }
            _Context.SaveChanges();
            #endregion

            #region 商品类别
            if (_Context.SK_WM_GoodsCategory.Any())
                return;
            var c1 = new SK_WM_GoodsCategory() { Name = "小吃快餐", Description = "这就是小吃快餐", SortCode = "" };
            var c2 = new SK_WM_GoodsCategory() { Name = "烧烤烤肉", Description = "这就是烧烤烤肉", SortCode = "" };
            var c3 = new SK_WM_GoodsCategory() { Name = "甜点饮品", Description = "这就是甜点饮品", SortCode = "" };
            var c4 = new SK_WM_GoodsCategory() { Name = "其他美食", Description = "这就是其他美食", SortCode = "" };           
           
        
            var cls = new List<SK_WM_GoodsCategory>() { c1, c2, c3, c4};
            foreach (var cl in cls)
            {
                _Context.SK_WM_GoodsCategory.Add(cl);
            }
            _Context.SaveChanges();
            #endregion

            #region 商品
            if (_Context.SK_WM_Goods.Any())
                return;
            var good1 = new SK_WM_Goods() { Name = "螺肉饭", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "香甜美味的螺肉，配上香喷喷的大米饭", FacadePrice = "15.00", Price = "12.00", Unit = "份", SalesVolume = "16", Stock = "999", State = "已上架", BelongToShopID = sh1.ID.ToString(), Shop = sh1, SK_WM_GoodsCategory = c1, };
            var good2 = new SK_WM_Goods() { Name = "正宗螺蛳粉", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "柳州正宗螺蛳粉，螺肉超多超实惠", FacadePrice = "8.00", Price = "6.50", Unit = "份", SalesVolume = "27", Stock = "999", State = "已上架", BelongToShopID = sh1.ID.ToString(), Shop = sh1, SK_WM_GoodsCategory = c1 };
            var good3 = new SK_WM_Goods() { Name = "小龙虾", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "清蒸水煮冰镇小龙虾，鲜美无比", FacadePrice = "30.00", Price = "28.80", Unit = "份", SalesVolume = "32", Stock = "999", State = "已上架", BelongToShopID = sh1.ID.ToString(), Shop = sh1, SK_WM_GoodsCategory = c1 };
            var good4 = new SK_WM_Goods() { Name = "老友粉", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "和你的老友一起來碗老友粉叙叙旧吧", FacadePrice = "8.00", Price = "7.00", Unit = "份", SalesVolume = "10", Stock = "999", State = "已上架", BelongToShopID = sh1.ID.ToString(), Shop = sh1, SK_WM_GoodsCategory = c1 };
            var good5 = new SK_WM_Goods() { Name = "红烧牛肉面", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "牛肉玉面的结合，吃过的人都很怀念", FacadePrice = "10.00", Price = "8.80", Unit = "份", SalesVolume = "5", Stock = "999", State = "已上架", BelongToShopID = sh1.ID.ToString(), Shop = sh1, SK_WM_GoodsCategory = c1 };
            var good6 = new SK_WM_Goods() { Name = "牛肉丸子", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "一口香滑，牛肉汤汁，满口回味", FacadePrice = "60.00", Price = "49.90", Unit = "份", SalesVolume = "35", Stock = "999", State = "已上架", BelongToShopID = sh1.ID.ToString(), Shop = sh1, SK_WM_GoodsCategory = c1 };

            var good7 = new SK_WM_Goods() { Name = "蒜蓉烤茄子", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "蒜末香浓，其味无穷，年轻人都爱吃", FacadePrice = "8.00", Price = "8.00", Unit = "份", SalesVolume = "14", Stock = "999", State = "已上架", BelongToShopID = sh2.ID.ToString(), Shop = sh2, SK_WM_GoodsCategory = c2 };
            var good8 = new SK_WM_Goods() { Name = "烤鱼", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "配菜任意加，特色烤鱼，店铺招牌菜", FacadePrice = "40.00", Price = "38.00", Unit = "份", SalesVolume = "13", Stock = "999", State = "已上架", BelongToShopID = sh2.ID.ToString(), Shop = sh2, SK_WM_GoodsCategory = c2 };
            var good9 = new SK_WM_Goods() { Name = "烤串", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "各类烤串满足你的味蕾，价格实惠", FacadePrice = "3.00", Price = "3.00", Unit = "份", SalesVolume = "16", Stock = "999", State = "已上架", BelongToShopID = sh2.ID.ToString(), Shop = sh2, SK_WM_GoodsCategory = c2 };
            var good10 = new SK_WM_Goods() { Name = "烤韭菜", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "热卖的烧烤类之一，爱吃烧烤的人都爱吃的烤韭菜", FacadePrice = "2.00", Price = "2.00", Unit = "份", SalesVolume = "29", Stock = "999", State = "已上架", BelongToShopID = sh2.ID.ToString(), Shop = sh2, SK_WM_GoodsCategory = c2 };
            var good11 = new SK_WM_Goods() { Name = "考生蚝", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "身体虽小，但全身都是精华，美味无比，一份2个", FacadePrice = "5.00", Price = "5.00", Unit = "份", SalesVolume = "18", Stock = "999", State = "已上架", BelongToShopID = sh2.ID.ToString(), Shop = sh2, SK_WM_GoodsCategory = c2 };
            var good12 = new SK_WM_Goods() { Name = "孜然烤羊排", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "特卖羊排，实惠，好吃到爆，快来试一试吧", FacadePrice = "88.80", Price = "68.00", Unit = "份", SalesVolume = "3", Stock = "999", State = "已上架", BelongToShopID = sh2.ID.ToString(), Shop = sh2, SK_WM_GoodsCategory = c2 };

            var good13 = new SK_WM_Goods() { Name = "情侣冰激凌", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "情侣套餐，一份两个，到店提供免费WiFi", FacadePrice = "99.99", Price = "99.99", Unit = "份", SalesVolume = "20", Stock = "999", State = "已上架", BelongToShopID = sh3.ID.ToString(), Shop = sh3, SK_WM_GoodsCategory = c3 };
            var good14 = new SK_WM_Goods() { Name = "草莓巧克力", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "18.88", Price = "18.88", Unit = "份", SalesVolume = "7", Stock = "999", State = "已上架", BelongToShopID = sh3.ID.ToString(), Shop = sh3, SK_WM_GoodsCategory = c3 };
            var good15 = new SK_WM_Goods() { Name = "巧克力蛋糕", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "26.66", Price = "26.66", Unit = "份", SalesVolume = "56", Stock = "999", State = "已上架", BelongToShopID = sh3.ID.ToString(), Shop = sh3, SK_WM_GoodsCategory = c3 };
            var good16 = new SK_WM_Goods() { Name = "水果沙拉", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "20.00", Price = "20.00", Unit = "份", SalesVolume = "23", Stock = "999", State = "已上架", BelongToShopID = sh3.ID.ToString(), Shop = sh3, SK_WM_GoodsCategory = c3 };
            var good17 = new SK_WM_Goods() { Name = "特色铜锣烧", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "10.00", Price = "10.00", Unit = "份", SalesVolume = "14", Stock = "999", State = "已上架", BelongToShopID = sh3.ID.ToString(), Shop = sh3, SK_WM_GoodsCategory = c3 };
            var good18 = new SK_WM_Goods() { Name = "西点蛋糕", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "8.88", Price = "8.00", Unit = "份", SalesVolume = "19", Stock = "999", State = "已上架", BelongToShopID = sh3.ID.ToString(), Shop = sh3, SK_WM_GoodsCategory = c3 };

            var good19 = new SK_WM_Goods() { Name = "美味蛋炒饭", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "10.00", Price = "10.00", Unit = "份", SalesVolume = "75", Stock = "999", State = "已上架", BelongToShopID = sh4.ID.ToString(), Shop = sh4, SK_WM_GoodsCategory = c4 };
            var good20 = new SK_WM_Goods() { Name = "至尊芝士鸡排双人套餐", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "38.00", Price = "38.00", Unit = "份", SalesVolume = "47", Stock = "999", State = "已上架", BelongToShopID = sh4.ID.ToString(), Shop = sh4, SK_WM_GoodsCategory = c4 };
            var good21 = new SK_WM_Goods() { Name = "新奥尔良风情比萨", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "66.00", Price = "66.00", Unit = "份", SalesVolume = "6", Stock = "999", State = "已上架", BelongToShopID = sh4.ID.ToString(), Shop = sh4, SK_WM_GoodsCategory = c4 };
            var good22 = new SK_WM_Goods() { Name = "宴席套餐", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "宴席套餐，建议10人使用", FacadePrice = "788.00", Price = "788.00", Unit = "份", SalesVolume = "9", Stock = "999", State = "已上架", BelongToShopID = sh4.ID.ToString(), Shop = sh4, SK_WM_GoodsCategory = c4 };
            var good23 = new SK_WM_Goods() { Name = "意大利面", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "16.00", Price = "16.00", Unit = "份", SalesVolume = "54", Stock = "999", State = "已上架", BelongToShopID = sh4.ID.ToString(), Shop = sh4, SK_WM_GoodsCategory = c4 };
            var good24 = new SK_WM_Goods() { Name = "鱼蛙火锅", ShelvesTime = DateTime.Now, ModifyTime = DateTime.Now, Description = "此处为商品描述，该商品暂无描述！", FacadePrice = "66.66", Price = "66.66", Unit = "份", SalesVolume = "6", Stock = "999", State = "已上架", BelongToShopID = sh5.ID.ToString(), Shop = sh5, SK_WM_GoodsCategory = c4 };


            var sk_wm_Goods = new List<SK_WM_Goods>() { good1, good2, good3, good4, good5, good6, good7, good8, good9, good10, good11, good12, good13, good14, good15, good16, good17, good18, good19, good20, good21, good22, good23, good24 };

            foreach (var sk_wm_Good in sk_wm_Goods)
            {
                _Context.SK_WM_Goods.Add(sk_wm_Good);
            }
            _Context.SaveChanges();
            #endregion

            #region 图片添加
            if (_Context.BusinessImages.Any())
                return;
            //商品图片
            var shIMG1 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good1.ID, UploadPath = "/images/DemoGoodsIMG/xiaochi/xiaoc_lrf.jpg" };
            var shIMG2 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good2.ID, UploadPath = "/images/DemoGoodsIMG/xiaochi/xiaoc_lsf.jpg" };
            var shIMG3 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good3.ID, UploadPath = "/images/DemoGoodsIMG/xiaochi/xiaoc_lx.jpg" };
            var shIMG4 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good4.ID, UploadPath = "/images/DemoGoodsIMG/xiaochi/xiaoc_lyf.jpg" };
            var shIMG5 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good5.ID, UploadPath = "/images/DemoGoodsIMG/xiaochi/xiaoc_nrm.jpg" };
            var shIMG6 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good6.ID, UploadPath = "/images/DemoGoodsIMG/xiaochi/xiaoc_nrw.jpg" };

            var shIMG7 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good7.ID, UploadPath = "/images/DemoGoodsIMG/shaokao/shaok_kaoqiezi.jpg" };
            var shIMG8 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good8.ID, UploadPath = "/images/DemoGoodsIMG/shaokao/shaok_kaoyu.jpg" };
            var shIMG9 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good9.ID, UploadPath = "/images/DemoGoodsIMG/shaokao/shaok_kc.jpg" };
            var shIMG10 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good10.ID, UploadPath = "/images/DemoGoodsIMG/shaokao/shaok_kjc.jpg" };
            var shIMG11 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good11.ID, UploadPath = "/images/DemoGoodsIMG/shaokao/shaok_ksh.jpg" };
            var shIMG12 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good12.ID, UploadPath = "/images/DemoGoodsIMG/shaokao/shaok_yp.jpg" };

            var shIMG13 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good13.ID, UploadPath = "/images/DemoGoodsIMG/tiandian/tiand_bingqilin.jpg" };
            var shIMG14 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good14.ID, UploadPath = "/images/DemoGoodsIMG/tiandian/tiand_caomeiqiaokeli.jpg" };
            var shIMG15 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good15.ID, UploadPath = "/images/DemoGoodsIMG/tiandian/tiand_qiaokelidangao.jpg" };
            var shIMG16 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good16.ID, UploadPath = "/images/DemoGoodsIMG/tiandian/tiand_shuiguoshala.jpg" };
            var shIMG17 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good17.ID, UploadPath = "/images/DemoGoodsIMG/tiandian/tiand_tongluoshao.jpg" };
            var shIMG18 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good18.ID, UploadPath = "/images/DemoGoodsIMG/tiandian/tiand_xidiandangao.jpg" };

            var shIMG19 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good19.ID, UploadPath = "/images/DemoGoodsIMG/qitameishi/qita_dcf.png" };
            var shIMG20 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good20.ID, UploadPath = "/images/DemoGoodsIMG/qitameishi/qita_jp.jpg" };
            var shIMG21 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good21.ID, UploadPath = "/images/DemoGoodsIMG/qitameishi/qita_pisa.jpg" };
            var shIMG22 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good22.ID, UploadPath = "/images/DemoGoodsIMG/qitameishi/qita_yanxitaocan.jpg" };
            var shIMG23 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good23.ID, UploadPath = "/images/DemoGoodsIMG/qitameishi/qita_ydlm.jpg" };
            var shIMG24 = new BusinessImage() { IsForTitle = true, RelevanceObjectID = good24.ID, UploadPath = "/images/DemoGoodsIMG/qitameishi/qita_hgyt.jpg" };

            //店铺LOGO图片和横幅图片
            var shopAvatar1 = new BusinessImage() { RelevanceObjectID = sh1.ID, UploadPath = "/images/dpDefault.jpg", Description = "shopAvatar" };
            var shopBanner1 = new BusinessImage() { RelevanceObjectID = sh1.ID, UploadPath = "/images/hf.jpg", Description = "shopBanner" };

            var shopAvatar2 = new BusinessImage() { RelevanceObjectID = sh2.ID, UploadPath = "/images/dpDefault.jpg", Description = "shopAvatar" };
            var shopBanner2 = new BusinessImage() { RelevanceObjectID = sh2.ID, UploadPath = "/images/hf.jpg", Description = "shopBanner" };

            var shopAvatar3 = new BusinessImage() { RelevanceObjectID = sh3.ID, UploadPath = "/images/dpDefault.jpg", Description = "shopAvatar" };
            var shopBanner3 = new BusinessImage() { RelevanceObjectID = sh3.ID, UploadPath = "/images/hf.jpg", Description = "shopBanner" };

            var shopAvatar4 = new BusinessImage() { RelevanceObjectID = sh4.ID, UploadPath = "/images/dpDefault.jpg", Description = "shopAvatar" };
            var shopBanner4 = new BusinessImage() { RelevanceObjectID = sh4.ID, UploadPath = "/images/hf.jpg", Description = "shopBanner" };

            var shopAvatar5 = new BusinessImage() { RelevanceObjectID = sh5.ID, UploadPath = "/images/dpDefault.jpg", Description = "shopAvatar" };
            var shopBanner5 = new BusinessImage() { RelevanceObjectID = sh5.ID, UploadPath = "/images/hf.jpg", Description = "shopBanner" };

            var shIMGs = new List<BusinessImage>() {
                shIMG1, shIMG2, shIMG3, shIMG4, shIMG5, shIMG6, shIMG7, shIMG8, shIMG9, shIMG10,
                shIMG11, shIMG12,shIMG13, shIMG14, shIMG15, shIMG16, shIMG17, shIMG18, shIMG19,
                shIMG20,shIMG21,shIMG22,shIMG23,shIMG24,
                shopAvatar1,shopBanner1,
                shopAvatar2,shopBanner2,
                shopAvatar3,shopBanner3,
                shopAvatar4,shopBanner4,
                shopAvatar5,shopBanner5
            };

            foreach (var shIMG in shIMGs)
            {
                _Context.BusinessImages.Add(shIMG);
            }
            _Context.SaveChanges();
            #endregion


        }


    }
}
