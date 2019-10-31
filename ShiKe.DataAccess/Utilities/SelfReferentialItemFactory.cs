using ShiKe.Common.ViewModelComponents;
using ShiKe.DataAccess.SqlServerr;
using ShiKe.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShiKe.DataAccess.Utilities
{
    /// <summary>
    /// 用于将一些需要的对象集合转换为简单的 SelfReferentialItem 集合或者个体的公共类型方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class SelfReferentialItemFactory<T> where T : class, IEntity, new()
    {
        /// <summary>
        /// 根据指定的类型，直接转换为 SelfReferentialItem
        /// </summary>
        /// <param name="bo"></param>
        /// <returns></returns>
        public static SelfReferentialItem Get(T bo)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var parentProperty = properties.Where(x => x.PropertyType.Name == typeof(T).Name).FirstOrDefault();

            var pItem = new SelfReferentialItem();
            pItem.ID = bo.ID.ToString();

            var pObject = (T)parentProperty.GetValue(bo);
            pItem.ParentID = pObject.ID.ToString();
            pItem.DisplayName = pObject.Name;
            pItem.SortCode = pObject.SortCode;

            return pItem;

        }

        /// <summary>
        /// 根据指定的类型，直接提取对应的持久层数据，并转换为 List<SelfReferentialItem>。
        /// </summary>
        /// <param name="useSpace">返回的数据中，是否在 Name 属性前面，根据层次添加空格</param>
        /// <returns></returns>
        public static List<SelfReferentialItem> GetCollection(bool useSpace)
        {
            //var service = new EntityRepository<T>(new EntityDbContext());
            PropertyInfo[] properties = typeof(T).GetProperties();
            var parentProperty = properties.FirstOrDefault(x => x.PropertyType.Name == typeof(T).Name);

            //var pCollection = service.GetAll().OrderBy(s => s.SortCode).ToList();
            var pItems = new List<SelfReferentialItem>();
            //foreach (var item in pCollection)
            //{
            //    var pItem = new SelfReferentialItem();
            //    pItem.ID = item.ID.ToString();
            //    pItem.DisplayName = item.Name;
            //    pItem.SortCode = item.SortCode;

            //    var pObject = (T)parentProperty.GetValue(item);
            //    pItem.ParentID = pObject.ID.ToString();

            //    pItems.Add(pItem);
            //}

            if (useSpace)
                return SetHierarchical(pItems);
            else
                return pItems;
        }

        /// <summary>
        /// 根据指定的类型，将对应的对象集合转换为 List<SelfReferentialItem>。
        /// </summary>
        /// <param name="pCollection">对象集合</param>
        /// <param name="useSpace">返回的数据中，是否在 Name 属性前面，根据层次添加空格</param>
        /// <returns></returns>
        public static List<SelfReferentialItem> GetCollection(List<T> pCollection, bool useSpace)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var parentProperty = properties.FirstOrDefault(x => x.PropertyType.Name == typeof(T).Name);
            var pItems = new List<SelfReferentialItem>();
            foreach (var item in pCollection)
            {
                var pItem = new SelfReferentialItem
                {
                    ID = item.ID.ToString(),
                    DisplayName = item.Name,
                    SortCode = item.SortCode
                };

                var pObject = (T)parentProperty.GetValue(item);
                pItem.ParentID = pObject.ID.ToString();

                pItems.Add(pItem);
            }

            if (useSpace)
                return SetHierarchical(pItems);
            else
                return pItems;
        }

        /// <summary>
        /// 根据指定泛型的过滤条件，直接提取对应的对象集合，并转换为 List<SelfReferentialItem>。
        /// </summary>
        /// <param name="useSpace">是否使用中文空格符表达缩进状态</param>
        /// <param name="predicate">抽取对象的基本条件</param>
        /// <returns></returns>
        public static List<SelfReferentialItem> GetCollection(bool useSpace, Expression<Func<T, bool>> predicate)
        {
            //var service = new EntityRepository<T>(new EntityDbContext());
            PropertyInfo[] properties = typeof(T).GetProperties();
            var parentProperty = properties.Where(x => x.PropertyType.Name == typeof(T).Name).FirstOrDefault();

            //var pCollection = service.GetAll().Where(predicate).OrderBy(s => s.SortCode).ToList();
            var pItems = new List<SelfReferentialItem>();
            //foreach (var item in pCollection)
            //{
            //    var pItem = new SelfReferentialItem();
            //    pItem.ID = item.ID.ToString();
            //    pItem.DisplayName = item.Name;
            //    pItem.SortCode = item.SortCode;

            //    var pObject = (T)parentProperty.GetValue(item);
            //    pItem.ParentID = pObject.ID.ToString();

            //    pItems.Add(pItem);
            //}

            if (useSpace)
                return SetHierarchical(pItems);
            else
                return pItems;
        }

        /// <summary>
        /// 将普通的自引用关系的集合转换为具有层次感观的集合，通过在相应的节点之前添加中文空格的方式处理缩进
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<SelfReferentialItem> SetHierarchical(List<SelfReferentialItem> source)
        {
            var result = new List<SelfReferentialItem>();
            var rootItems = source.Where(rn => rn.ID == rn.ParentID || rn.ParentID == "");
            foreach (var item in rootItems)
            {
                result.Add(item);
                _GetHierarchicalStyleSubItems(item, 1, source, result);
            }
            return result;
        }

        /// <summary>
        /// 迭代构建层次节点
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="spaceNumber"></param>
        /// <param name="source"></param>
        /// <param name="result"></param>
        private static void _GetHierarchicalStyleSubItems(SelfReferentialItem rootNode, int spaceNumber, List<SelfReferentialItem> source, List<SelfReferentialItem> result)
        {
            var subItems = source.Where(sn => sn.ParentID == rootNode.ID && sn.ID != rootNode.ParentID).OrderBy(o => o.SortCode);
            foreach (var subItem in subItems.OrderBy(s => s.SortCode))
            {
                var ttt = _SpaceLength(spaceNumber);
                subItem.DisplayName = _SpaceLength(spaceNumber) + subItem.DisplayName;
                result.Add(subItem);
                _GetHierarchicalStyleSubItems(subItem, spaceNumber + 1, source, result);
            }
        }

        /// <summary>
        /// 根据层次节点的缩进空格数，创建中文空格符的字符串
        /// </summary>
        /// <param name="i">缩进格数</param>
        /// <returns></returns>
        private static string _SpaceLength(int i)
        {
            string space = "";
            for (int j = 0; j < i; j++)
            {
                space += "　";  // 全角空格符
            }
            return space + "";
        }
    }
}
