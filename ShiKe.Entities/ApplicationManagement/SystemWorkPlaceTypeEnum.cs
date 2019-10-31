using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiKe.Entities.ApplicationOrganization
{
    /// <summary>
    /// 公共用途类型的可以考虑应用在类似电子邮件、个人消息等等，业务用途的，一般是为授权用户来使用的，例如系统管理
    /// </summary>
    public enum SystemWorkPlaceTypeEnum
    {
        公共用途,业务用途
    }
}
