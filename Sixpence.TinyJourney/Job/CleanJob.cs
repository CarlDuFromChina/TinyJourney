using Quartz;
using Sixpence.Common.Utils;
using Sixpence.EntityFramework;
using Sixpence.Web.Config;
using Sixpence.Web.Job;
using Sixpence.Web.Utils;

namespace Sixpence.TinyJourney.Job
{
    public class CleanJob : JobBase
    {
        public CleanJob(IEntityManager manager) : base(manager)
        {
        }

        public override string Name => "清理作业";
        public override string Description => "清理日志、资源文件";
        public override IScheduleBuilder ScheduleBuilder => CronScheduleBuilder.CronSchedule("0 0 0 * * ?");

        public override void Executing(IJobExecutionContext context)
        {
            var files = FileHelper.GetFileList("*.log", Web.FolderType.Log);
            var logNameList = new List<string>();
            var days = SystemConfig.Config.LogBackupDays;

            if (days == 0)
                days = 7;

            // 需要保留的log
            for (int i = 0; i < days; i++)
            {
                logNameList.Add(DateTime.Now.AddDays(-i).ToString("yyyyMMdd"));
            }

            // 删除不需要保留的log
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (logNameList.Count(Item => Path.GetFileName(file).Contains(Item)) == 0)
                {
                    FileUtil.DeleteFile(file);
                }
            }
        }
    }
}