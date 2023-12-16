using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.Model
{
    public class EntityPrivilegeResponse
    {
        public bool create { get; set; }
        public bool read { get; set; }
        public bool delete { get; set; }
    }
}
