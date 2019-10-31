using System;

namespace ShiKe.Common.JsonModels
{
    /// <summary>
    ///     上传文件条目状态
    /// </summary>
    public class UploadedItemStatus
    {
        public Guid ID { get; set; }             //文件ID
        public string Name { get; set; }         //文件名称
        public string SizeString { get; set; }   //文件大小
        public string IconString { get; set; }   //文件图标 象形符号
        public bool IsSucceded { get; set; }     //是否保存成功并存在
        public string Message { get; set; }      

        public UploadedItemStatus()
        {
            IconString = "<sapan class='mif-attachment'></span>";
        }
    }
}