using ShiKe.Common.JsonModels;
using ShiKe.Common.ViewModelComponents;
using ShiKe.Entities.Attachments;
using ShiKe.Entities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShiKe.ViewModels.Attachments
{
    public class BusinessImageVM : IEntityVM
    {
        [Key]
        public Guid ID { get; set; }
        public string OrderNumber { get; set; }
        public bool IsNew { get; set; }
        public ListPageParameter ListPageParameter { get; set; }

        public string Name { get; set; }             // 图片显示名称

        public string Description { get; set; }      // 图片说明

        public string SortCode { get; set; }         // 内部业务编码

        public string DisplayName { get; set; }      // 图片显示名称

        public string OriginalFileName { get; set; } // 图片原始文件
        public DateTime UploadedTime { get; set; }   // 图片上传时间

        public string UploadPath { get; set; }       // 图片上传保存路径

        public string UploadFileSuffix { get; set; } // 上传文件的后缀名
        public long FileSize { get; set; }

        public string IconString { get; set; }       // 文件物理格式图标

        public bool IsForTitle { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Guid RelevanceObjectID { get; set; }  // 使用该图片的业务对象的 id
        public Guid UploaderID { get; set; }         // 关联上传人ID




        public BusinessImageVM()
        { }
        public BusinessImageVM(BusinessImage bo)
        {
            ID = bo.ID;
            Name = bo.Name;
            Description = bo.Description;
            SortCode = bo.SortCode;
            DisplayName = bo.DisplayName;
            OriginalFileName = bo.OriginalFileName;

            UploadedTime = bo.UploadedTime;
            UploadPath = bo.UploadPath;
            UploadFileSuffix = bo.UploadFileSuffix;
            FileSize = bo.FileSize;
            IconString = bo.IconString;
            IsForTitle = bo.IsForTitle;
            X = bo.X;
            Y = bo.Y;
            Width = bo.Width;
            RelevanceObjectID = bo.RelevanceObjectID;
            UploaderID = bo.UploaderID;
        }

        //public void MapToBo(BusinessImage bo)
        //{
        //     bo.ID =ID;
        //     bo.Name =Name;
        //     bo.Description =Description;
        //    bo.SortCode = SortCode;
        //    bo.DisplayName = DisplayName;
        //    bo.OriginalFileName = OriginalFileName;

        //    bo.UploadedTime = DateTime.Now;
        //    bo.UploadPath = bo.UploadPath;
        //    UploadFileSuffix = UploadFileSuffix;
        //    bo.FileSize = FileSize;
        //    bo.IconString = IconString;
        //    bo.IsForTitle = IsForTitle;
        //    bo.X = X;
        //    Y = bo.Y;
        //    bo.Width = Width;
        //    bo.RelevanceObjectID = RelevanceObjectID;
        //    bo.UploaderID = UploaderID;
        //}
    }
}
