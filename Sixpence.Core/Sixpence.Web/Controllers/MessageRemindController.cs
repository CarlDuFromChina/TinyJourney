using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Entity;
using Sixpence.Web.Service;

namespace Sixpence.Web.Controllers
{
    public class MessageRemindController : EntityBaseController<MessageRemind, MessageRemindService>
    {
        public MessageRemindController(MessageRemindService service) : base(service)
        {
        }

        /// <summary>
        /// 获取未读消息数量
        /// </summary>
        /// <returns></returns>
        [HttpGet("unread_message_count")]
        public object GetUnReadMessageCount()
        {
            return _service.GetUnReadMessageCount();
        }
    }
}
