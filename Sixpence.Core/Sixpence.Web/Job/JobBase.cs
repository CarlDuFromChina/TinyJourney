﻿using Sixpence.Web.Auth;
using Sixpence.EntityFramework.Entity;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Sixpence.Common.Current;
using Sixpence.Web.Entity;
using Microsoft.Extensions.Logging;
using Sixpence.EntityFramework;

namespace Sixpence.Web.Job
{
    /// <summary>
    /// Job基类（所有Job继承该基类）
    /// </summary>
    [DisallowConcurrentExecution]
    public abstract class JobBase : IJob
    {
        private IEntityManager _manager;
        private ILogger _logger;
        public JobBase(IEntityManager manager, ILoggerFactory loggerFactory)
        {
            _manager = manager;
            _logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// 作业名
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Job Key
        /// </summary>
        public virtual JobKey JobKey => new JobKey(Name, GetType().Namespace);

        /// <summary>
        /// 默认触发器状态
        /// </summary>
        public virtual TriggerState DefaultTriggerState => TriggerState.Normal;

        /// <summary>
        /// 作业描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 任务
        /// </summary>
        public abstract void Executing(IJobExecutionContext context);

        /// <summary>
        /// 任务执行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            var user = context.JobDetail.JobDataMap.Get("User") as CurrentUserModel;
            return Task.Factory.StartNew(() =>
            {
                _logger.LogInformation($"作业：{Name} 开始执行");

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                UserIdentityUtil.SetCurrentUser(user);
                var history = new JobHistory()
                {
                    Id = Guid.NewGuid().ToString(),
                    JobName = this.Name,
                    StartTime = DateTime.Now,
                    Status = "成功"
                };
                try
                {
                    Executing(context);
                }
                catch (Exception e)
                {
                    history.Status = "失败";
                    history.ErrorMsg = e.Message;
                    _logger.LogError($"作业：{Name}执行异常", e);
                    throw e;
                }
                finally
                {
                    history.EndTime = DateTime.Now;
                    _manager.Create(history);
                }
                stopWatch.Stop();
                _logger.LogInformation($"作业：{Name} 执行结束，耗时{stopWatch.ElapsedMilliseconds}ms");
            });
        }

        /// <summary>
        /// 获取Job生成器
        /// </summary>
        /// <returns></returns>
        public virtual JobBuilder GetJobBuilder()
        {
            return JobBuilder.Create(GetType())
                    .WithIdentity(JobKey.Name, JobKey.Group)
                    .WithDescription(Description);
        }

        /// <summary>
        /// 计划生成
        /// </summary>
        public virtual IScheduleBuilder ScheduleBuilder => null;
        public virtual TriggerBuilder GetTriggerBuilder()
        {
            if (ScheduleBuilder == null)
            {
                return null;
            }

            return TriggerBuilder
                    .Create()
                    .WithIdentity(JobKey.Name, JobKey.Group)
                    .WithDescription(Description)
                    .WithSchedule(ScheduleBuilder)
                    .StartAt(SystemTime.UtcNow().AddSeconds(5));
        }
    }
}
