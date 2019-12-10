using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;


namespace WebJob
{
   public class Functions
    {
        public static void ProcessQueueMessage(
    [QueueTrigger("lokeshqueue")] string message,
    ILogger logger)
        {
            logger.LogInformation($"New message from queue (lokeshqueue):{ message}");
}        
    }
}
