using Sixpence.Web.Job;
using Quartz;
using Sixpence.Common;
using System.Collections.Generic;
using System.Linq;
using Sixpence.Common.Utils;
using Sixpence.Web.Auth;
using Sixpence.ORM;
using Sixpence.Web.Model;

namespace Sixpence.Web.Service
{
    public class JobService : BaseService
    {
        public JobService() : base() { }
        public JobService(IEntityManager manager) : base(manager) { }

        /// <summary>
        /// 查询所有的job
        /// </summary>
        /// <returns></returns>
        public IList<JobModel> GetDataList()
        {
            var sql = @"
SELECT
	qjd.job_name AS name,
	qjd.description,
	qt.prev_fire_time AS prev_fire_time_ticks,
	qt.next_fire_time AS next_fire_time_ticks,
	qt.trigger_state,
	qct.cron_expression
FROM qrtz_job_details AS qjd
LEFT JOIN qrtz_triggers AS qt ON qjd.job_name = qt.job_name
LEFT JOIN qrtz_cron_triggers AS qct ON qt.trigger_name = qct.trigger_name
";
            var dataList = _manager.Query<JobModel>(sql).ToList();
            return dataList;
        }

        /// <summary>
        /// 运行一次job
        /// </summary>
        /// <param name="name"></param>
        public void RunOnceNow(string name)
        {
            ServiceFactory.ResolveAll<IJob>().Each(item =>
            {
                var job = item as JobBase;
                if (job.Name == name)
                {
                    var paramList = new Dictionary<string, object>() { { "User", UserIdentityUtil.GetCurrentUser() } };
                    JobHelpers.RunOnceNow(job.Name, job.GetType().Namespace, paramList);
                }
            });
        }

        /// <summary>
        /// 暂停Job
        /// </summary>
        /// <param name="jobName"></param>
        public void Pause(string jobName)
        {
            var job = ServiceFactory.ResolveAll<IJob>().FirstOrDefault(item => (item as JobBase).Name == jobName) as JobBase;
            AssertUtil.IsNull(job, $"未找到名为[{jobName}]作业");
            JobHelpers.PauseJob(job.Name, job.GetType().Namespace);
        }

        /// <summary>
        /// 重启job
        /// </summary>
        /// <param name="jobName"></param>
        public void Resume(string jobName)
        {
            var job = ServiceFactory.ResolveAll<IJob>().FirstOrDefault(item => (item as JobBase).Name == jobName) as JobBase;
            AssertUtil.IsNull(job, $"未找到名为[{jobName}]作业");
            JobHelpers.ResumeJob(job.Name, job.GetType().Namespace);
        }
    }
}