using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Model;
using Sixpence.EntityFramework;

namespace Sixpence.Web.EntityOptionProvider
{
    public class SysEntityEntityOptionProvider : IEntityOptionProvider
    {
        private readonly IEntityManager manager;
        public SysEntityEntityOptionProvider(IEntityManager manager)
        {
            this.manager = manager;
        }

        public IEnumerable<SelectOption> GetOptions()
        {
            return manager.Query<SelectOption>($"select code AS Value, name AS Name from sys_entity");
        }
    }
}
