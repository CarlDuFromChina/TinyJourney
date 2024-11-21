using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;

namespace Sixpence.Web.EntityPlugin
{
    public class GalleryPlugin : IEntityManagerPlugin
    {
        public void Execute(EntityManagerPluginContext context)
        {
            var obj = context.Entity as Gallery;
            switch (context.Action)
            {
                case EntityAction.PreCreate:
                    break;
                case EntityAction.PreUpdate:
                    break;
                case EntityAction.PostCreate:
                case EntityAction.PostUpdate:
                    var data1 = context.EntityManager.QueryFirst<SysFile>(obj.PreviewId);
                    var data2 = context.EntityManager.QueryFirst<SysFile>(obj.ImageId);
                    data1.ObjectId = obj.Id;
                    data2.ObjectId = obj.Id;
                    context.EntityManager.Update(data1);
                    context.EntityManager.Update(data2);
                    break;
                case EntityAction.PreDelete:
                    break;
                case EntityAction.PostDelete:
                    break;
                default:
                    break;
            }
        }
    }
}
