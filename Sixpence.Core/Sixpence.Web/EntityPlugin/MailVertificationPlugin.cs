using Sixpence.Web.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.EntityFramework;
using Sixpence.Web.Entity;

namespace Sixpence.Web.EntityPlugin
{
    public class MailVertificationPlugin : IEntityManagerPlugin
    {
        public void Execute(EntityManagerPluginContext context)
        {
            var entity = context.Entity as MailVertification;
            switch (context.Action)
            {
                case EntityAction.PreCreate:
                    {
                        var reciver = entity.MailAddress.ToString();
                        var title = entity.Name.ToString();
                        var content = entity.Content.ToString();
                        MailUtil.SendMail(reciver, title, content);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
