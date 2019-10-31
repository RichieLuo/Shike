using System;
using System.Collections.Generic;
using System.Text;

namespace ShiKe.DataAccess.SqlServer.Utilities
{
    /// <summary>
    /// 用于处理业务对象数据存储处理状态和相关的数据
    /// </summary>
    public class EditAndSaveStatus
    {
        public bool SaveOk { get; set; }
        public string StatusMessage { get; set; }
    }

}
