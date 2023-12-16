using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.Model
{
    public class SelectOption
    {
        #region 构造函数
        public SelectOption() { }

        public SelectOption(string name, string value)
        {
            Name = name;
            Value = value;
        }
        #endregion

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
