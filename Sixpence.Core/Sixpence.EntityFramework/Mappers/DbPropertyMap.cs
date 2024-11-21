using Sixpence.EntityFramework.Entity;
using Sixpence.EntityFramework.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework.Mappers
{
    /// <summary>
    /// 字段
    /// </summary>
    public class DbPropertyMap : IDbPropertyMap
    {
        public string Name { get; set; }
        public string DbType { get; set; }
        public string? Remark { get; set; }
        public int? Length { get; set; }
        public bool? CanBeNull { get; set; }
        public object? DefaultValue { get; set; }
        public bool IsKey { get; set; }
        public bool IsUnique { get; set; }
        public PrimaryType? PrimaryType { get; set; }
    }
}
