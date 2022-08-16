How to use Quartz Scheduler

1. create class as below QuartzJobBaseSampleJob

using Quartz;
using System.Threading.Tasks;

public class QuartzJobBaseSampleJob : IQuartzJob
{
    string IQuartzJob.JobDescription { set => value = "***descrip what purpose of this job***"; }

    public Task Execute(IJobExecutionContext context)
    {
        return Task.Factory.StartNew(() =>
        {
            // logic here will be execute once job trigger
        });
    }
}


