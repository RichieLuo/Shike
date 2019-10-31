using System;
using System.Collections.Generic;
using System.Text;

namespace ShiKe.Entities
{
    public interface IEntity:IEntityBase
    {
        string Name { get; set; }
        string Description { get; set; }
        string SortCode { get; set; }
    }
}
