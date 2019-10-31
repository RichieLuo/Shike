#region

using System;
using System.IO;

#endregion

namespace ShiKe.Common.JsonModels
{
    /// <summary>
    ///     文件的基本信息
    /// </summary>
    public class FileStatus
    {
        public string Name { get; set; }        //文件名
        public string Type { get; set; }        //类型
        public long Size { get; set; }          //长度
        public string Url { get; set; }         //获取文件链接
        public string ThumbnailUrl { get; set; }//获取缩略图的链接
        public string DeleteUrl { get; set; }   //删除文件的链接
        public string Message { get; set; }     //其他信息

        public FileStatus()
        {
        }

        public FileStatus(FileInfo fileInfo)
        {
            _SetValues(fileInfo.Name, fileInfo.Length, fileInfo.FullName);
        }

        public FileStatus(string fileName, long fileLength, string fullPath)
        {
            _SetValues(fileName, fileLength, fullPath);
        }

        private void _SetValues(string fileName, long fileLength, string fullPath)
        {
            Name = fileName;
            Type = "image/png";
            Size = fileLength;
            var ext = Path.GetExtension(fullPath);

            var fileSize = ConvertBytesToMegabytes(new FileInfo(fullPath).Length);
            if (fileSize > 3 || !_IsImage(ext))
            {
                ThumbnailUrl = "/Content/img/generalFile.png";
            }
            else
            {
                ThumbnailUrl = @"data:image/png;base64," + _EncodeFile(fullPath);
            }
        }

        private bool _IsImage(string ext)
        {
            return ext == ".gif" || ext == ".jpg" || ext == ".png";
        }

        private string _EncodeFile(string fileName)
        {
            return Convert.ToBase64String(File.ReadAllBytes(fileName));
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}