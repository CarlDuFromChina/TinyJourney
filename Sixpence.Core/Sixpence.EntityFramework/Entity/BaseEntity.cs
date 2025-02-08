using Sixpence.EntityFramework.Interface;
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
    /// 实体设计原则：
    /// 1、实体类必须继承BaseEntity
    /// 2、实体类的属性必须是public，且必须有get和set方法
    /// 3、实体类Key只有一个（Id）类型是Guid
    /// 4、实体类业务类主键是 Unique，自动创建 index
    /// 5、实体类命名规则为 PascalCase：如：public string PhoneNumber { get;set; }
    /// </summary>
    public abstract class BaseEntity : IEntity
    {
        /// <summary>
        /// 实体映射
        /// 作用：用于获取实体的属性信息，如：字段名、字段类型、表信息等
        /// </summary>
        [JsonIgnore]
        public IDbEntityMap EntityMap => ServiceCollectionExtensions.Options.EntityMaps[this.GetType().FullName];

        /// <summary>
        /// 主键，通过属性反射获取
        /// 在属性中标记 [Key] 特性
        /// </summary>
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

        /// <summary>
        /// 字段集合
        /// </summary>
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
                return columns.AsReadOnly();
            }
        }

        /// <summary>
        /// 生成新的主键
        /// </summary>
        /// <returns></returns>
        public string NewId() => EntityCommon.GenerateID(this.PrimaryColumn.PrimaryType)?.ToString();
    }
}
