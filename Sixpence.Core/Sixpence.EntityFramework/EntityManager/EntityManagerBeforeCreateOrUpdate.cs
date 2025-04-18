﻿using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    public class EntityManagerBeforeCreateOrUpdate : IEntityManagerBeforeCreateOrUpdate
    {
        public void Execute(EntityManagerPluginContext context)
        {
            var entity = context.Entity as TrackedBaseEntity;
            var manager = context.EntityManager;

            if (entity != null)
            {
                switch (context.Action)
                {
                    case EntityAction.PreCreate:
                        {
                            entity.UpdatedAt = DateTime.Now;
                            entity.CreatedAt = DateTime.Now;
                        }
                        break;
                    case EntityAction.PreUpdate:
                        {
                            entity.UpdatedAt = DateTime.Now;
                            if (entity.CreatedAt == null)
                            {
                                entity.CreatedAt = default(DateTime);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}