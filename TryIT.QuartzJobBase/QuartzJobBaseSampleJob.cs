using Quartz;
using System.Threading.Tasks;

namespace TryIT.QuartzJobBase
{
    /// <summary>
    /// sample job
    /// </summary>
    public class QuartzJobBaseSampleJob : IQuartzJob
    {
        /// <summary>
        /// describe this job
        /// </summary>
        string IQuartzJob.JobDescription { get => "***describe what purpose of this job***"; }

        /// <summary>
        /// run job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                // logic here will be execute once job trigger
            });
        }
    }
}
