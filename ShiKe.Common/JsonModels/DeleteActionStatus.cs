namespace ShiKe.Common.JsonModels
{
    /// <summary>
    ///     用于加载数据删除操作完成之后返回前端的信息
    /// </summary>
    public class DeleteActionStatus
    {
        public bool IsOK { get; set; }                       //是否删除成功
        public string ErrorMassage { get; set; }             //错误信息
        public string PageIndex { get; set; }                //当前页码
        public string TypeID { get; set; }                   //当前过滤对象 ID
        public string ExtenssionFunctionString { get; set; } // 约定数据持久化之后，除了执行返回列表的方法外，还需要执行的刷新导航树的另外的方法 

        public DeleteActionStatus()
        {
            ExtenssionFunctionString = "";
        }
    }
}