using Sixpence.ORM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    public class AuditEntity : SormEntity
    {
        [Column]
        public string CreatedBy { get; set; }

        [Column]
        public string CreatedByName { get; set; }

        [Column]
        public string UpdatedBy { get; set; }

        [Column]
        public string UpdatedByName { get; set; }
    }
}
