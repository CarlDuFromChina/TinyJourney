using Sixpence.EntityFramework.Entity;
using Sixpence.EntityFramework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework.Entity
{
    internal class PrimaryEntityColumn : IPrimaryEntityColumn
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public PrimaryType PrimaryType { get; set; }
        public IDbPropertyMap DbPropertyMap { get; set; }
    }
}
