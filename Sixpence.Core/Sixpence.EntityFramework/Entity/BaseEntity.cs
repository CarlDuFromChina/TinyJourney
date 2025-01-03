﻿using Sixpence.EntityFramework.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework.Entity
{
    /// <summary>
    /// 实体类基类，实体类可以继承此类
    /// </summary>
    public abstract class BaseEntity : IEntity
    {
        [JsonIgnore]
        public IDbEntityMap EntityMap => ServiceCollectionExtensions.Options.EntityMaps[this.GetType().FullName];

        [JsonIgnore]
        public IPrimaryEntityColumn PrimaryColumn
        {
            get
            {
                var propertyMap = EntityMap.Properties.FirstOrDefault(item => item.IsKey);
                var property = EntityCommon.GetPrimaryPropertyInfo(GetType());

                if (propertyMap == null)
                    throw new Exception("实体未定义主键");

                return new PrimaryEntityColumn()
                {
                    Name = propertyMap.Name,
                    Value = property.GetValue(this) ?? "",
                    DbPropertyMap = propertyMap,
                    PrimaryType = propertyMap.PrimaryType.GetValueOrDefault(PrimaryType.GUID)
                };
            }
        }

        [JsonIgnore]
        public IList<IEntityColumn> Columns
        {
            get
            {
                var columns = new List<IEntityColumn>();
                var attributes = EntityCommon.GetProperties(this);
                foreach (var item in attributes)
                {
                    if (item.Key != PrimaryColumn.Name)
                    {
                        var column = new EntityColumn()
                        {
                            Name = item.Key,
                            Value = item.Value,
                            DbPropertyMap = EntityMap.Properties.FirstOrDefault(p => p.Name == EntityCommon.PascalToUnderline(item.Key)),
                        };
                        columns.Add(column);
                    }
                }
                return columns;
            }
        }

        public string NewId() => EntityCommon.GenerateID(this.PrimaryColumn.PrimaryType)?.ToString();
    }
}
