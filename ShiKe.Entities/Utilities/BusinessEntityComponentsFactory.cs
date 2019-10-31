using System;
using System.Globalization;
using System.Linq;

namespace ShiKe.Entities.Utilities
{
    public class BusinessEntityComponentsFactory
    {
        /// <summary>
        /// 提取根据系统时间生成 SortCode 所需要的字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string SortCodeByDefaultDateTime<T>()
        {
            var result = "Default";
            var timeStampString = "";

            var nowTime = DateTime.Now;
            timeStampString = nowTime.ToString("yyyy-MM-dd-hh-mm-ss-ffff", DateTimeFormatInfo.InvariantInfo);

            var entityName = typeof(T).Name;
            result = entityName + "_" + timeStampString;
            return result;
        }

        public static string GetRandomCode(int num)
        {
            string[] source = { "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            string code = "";
            int p = int.Parse(DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString());
            //Thread.Sleep(1);
            Random rd = new Random(p);
            for (int i = 0; i < num; i++)
            {
                code += source[rd.Next(0, source.Length)];
            }
            return code;
        }

        /// <summary>
        /// 提取指定的业务对象属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(Object obj, string propertyName)
        {
            //var property = obj.GetType().GetProperties().Where(pn => pn.Name == propertyName).FirstOrDefault();
            //var propertyValue = property.GetValue(obj);
            return new object(); //propertyValue;
        }

    }
}
