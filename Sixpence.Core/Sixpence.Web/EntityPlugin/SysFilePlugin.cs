using Sixpence.Common;
using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Entity;
using Sixpence.Web.Config;
using Sixpence.Web.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Sixpence.Web.EntityPlugin
{
    public class SysFilePlugin : IEntityManagerPlugin
    {
        public void Execute(EntityManagerPluginContext context)
        {
            var entity = context.Entity as SysFile;
            switch (context.Action)
            {
                case EntityAction.PreCreate:
                    break;
                case EntityAction.PostCreate:
                    break;
                case EntityAction.PreUpdate:
                    break;
                case EntityAction.PostUpdate:
                    break;
                case EntityAction.PreDelete:
                    break;
                case EntityAction.PostDelete:
                    {
                        #region 如果文件没有实体关联就删除
                        var param = new
                        {
                            hash_code = entity.HashCode,
                            id = entity.Id
                        };

                        var fileList = context.EntityManager
                            .Query<SysFile>(param)
                            .Select(item => item.RealName)
                            .ToList();

                        if (fileList.IsNotEmpty())
                        {
                            AppContext.Storage.DeleteAsync(fileList).Wait();
                        }
                        break;
                        #endregion
                    }
                default:
                    break;
            }
        }
    }
}
