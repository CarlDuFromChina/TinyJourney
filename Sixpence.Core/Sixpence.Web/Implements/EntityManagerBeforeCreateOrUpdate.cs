using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Entity;
using Sixpence.Web.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Implements
{
    public class EntityManagerBeforeCreateOrUpdate : IEntityManagerBeforeCreateOrUpdate
    {
        public void Execute(EntityManagerPluginContext context)
        {
            var entity = context.Entity as AuditEntity;
            if (entity != null)
            {
                var user = UserIdentityUtil.GetCurrentUser();
                entity.CreatedBy = user.Id;
                entity.CreatedByName = user.Name;
                entity.UpdatedBy = user.Id;
                entity.UpdatedByName = user.Name;
            }
        }
    }
}
