using Quartz;

namespace QuartzJobBase
{
    /// <summary>
    /// interface for QuartzJob, refer to <see cref="QuartzJobBase.QuartzJobBaseSampleJob"/> for implementation example
    /// </summary>
    public interface IQuartzJob : IJob
    {
        /// <summary>
        /// job description, describe what purpose of this job
        /// </summary>
        string JobDescription { get; }
    }
}
