﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    internal static class IEnumerableExtensions
    {
        internal static bool IsEmpty<T>(this IEnumerable<T> ts)
        {
            return ts == null || ts.Count() == 0;
        }

        internal static bool IsNotEmpty<T>(this IEnumerable<T> ts)
        {
            return !IsEmpty(ts);
        }

        internal static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        internal static DataTable ToDataTable<T>(this IList<T> data, DataColumnCollection columns)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (DataColumn item in columns)
            {
                var prop = properties.Find(item.ColumnName, true);
                table.Columns.Add(new DataColumn(item.ColumnName, item.DataType));
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (DataColumn c in columns)
                {
                    row[c.ColumnName] = DBNull.Value;

                    var prop = properties.Find(c.ColumnName, true);
                    var propValue = prop?.GetValue(item);
                    if (propValue != null)
                    {
                        row[c.ColumnName] = Convert.ChangeType(propValue, c.DataType);
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// 针对IEnumerable的各项进行处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="callback"></param>
        internal static void Each<T>(this IEnumerable<T> src, Action<T> callback)
        {
            if (src == null) return;
            foreach (var item in src)
            {
                callback(item);
            }
        }

        /// <summary>
        /// 数组转集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static List<T> ToList<T>(this T[] array)
        {
            var list = new List<T>();
            if (array == null || array.Length == 0)
            {
                return list;
            }

            array.Each(item =>
            {
                list.Add(item);
            });
            return list;
        }
    }
}
