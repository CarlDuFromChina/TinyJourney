﻿using Sixpence.Web.Auth;
using Sixpence.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;
using Microsoft.Extensions.Logging;

namespace Sixpence.Web.Service
{
    public class MessageRemindService : EntityService<MessageRemind>
    {
        public MessageRemindService(IEntityManager manager, ILogger<EntityService<MessageRemind>> logger, IRepository<MessageRemind> repository) : base(manager, logger, repository)
        {
        }

        public override IList<EntityView> GetViewList()
        {
            return new List<EntityView>()
            {
                new EntityView()
                {
                    ViewId = "9E778EBC-9961-4CF7-B352-36DF30F33735",
                    Name = "评论消息",
                    Sql = @"
SELECT * FROM message_remind
WHERE message_type IN ('comment', 'reply')",
                    OrderBy = "created_at DESC"
                },
                new EntityView()
                {
                    ViewId = "FBAF583B-4B25-477B-B5DC-C5D110976A9A",
                    Name = "点赞消息",
                    Sql = @"
SELECT * FROM message_remind
WHERE message_type = 'upvote'",
                    OrderBy = "created_at DESC"
                },
                new EntityView()
                {
                    ViewId = "F7A3E0A9-4951-4EA7-A486-5B35056AC17A",
                    Name = "系统消息",
                    Sql = @"
SELECT * FROM message_remind
WHERE message_type = 'system'",
                    OrderBy = "created_at DESC"
                },
            };
        }

        public void ReadMessage(IEnumerable<string> ids)
        {
            var inSqlResult = _manager.Driver.SqlBuilder.BuildInClauseSql("id", 0, ids.Cast<object>().ToList());
            var sql = $@"
UPDATE message_remind
SET	is_read = true
WHERE id {inSqlResult.sql}
";
            _manager.Execute(sql, inSqlResult.param);
        }

        public override DataModel<MessageRemind> GetDataList(IList<SearchCondition> searchList, int pageSize, int pageIndex, string viewId = "", string searchValue = "")
        {
            if (searchList.IsEmpty())
            {
                searchList = new List<SearchCondition>();
            }
            searchList.Add(new SearchCondition() { Name = "receiver_id", Type = SearchType.Equals, Value = UserIdentityUtil.GetCurrentUserId() });
            var model = base.GetDataList(searchList, pageSize, pageIndex, viewId, searchValue);
            var ids = model.Data.Where(item => !item.IsRead.Value).Select(item => item.Id);
            ReadMessage(ids);
            return model;
        }

        public override IEnumerable<MessageRemind> GetDataList(IList<SearchCondition> searchList, string viewId = "", string searchValue = "")
        {
            if (searchList.IsEmpty())
            {
                searchList = new List<SearchCondition>();
            }
            searchList.Add(new SearchCondition() { Name = "receiver_id", Type = SearchType.Equals, Value = UserIdentityUtil.GetCurrentUserId() });
            var model = base.GetDataList(searchList, viewId, searchValue);
            var ids = model.Where(item => !item.IsRead.Value).Select(item => item.Id);
            ReadMessage(ids);
            return model;
        }

        /// <summary>
        /// 获取未读消息数量
        /// </summary>
        /// <returns></returns>
        public object GetUnReadMessageCount()
        {
            var userid = UserIdentityUtil.GetCurrentUserId();
            var paramList = new Dictionary<string, object>() { { "@id", userid } };
            var sql = @"
SELECT COUNT(1)
FROM message_remind
WHERE receiver_id = @id AND is_read is false";
            var total = _manager.ExecuteScalar(sql, paramList);
            var upvote = _manager.ExecuteScalar($"{sql} AND message_type = 'upvote'", paramList);
            var comment = _manager.ExecuteScalar($"{sql} AND message_type IN ('comment', 'reply')", paramList);
            var system = _manager.ExecuteScalar($"{sql} AND message_type = 'system'", paramList);
            return new
            {
                total = Convert.ToInt32(total),
                upvote = Convert.ToInt32(upvote),
                comment = Convert.ToInt32(comment),
                system = Convert.ToInt32(system)
            };
        }
    }
}
