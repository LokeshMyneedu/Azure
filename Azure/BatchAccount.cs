using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;


namespace AzureLearn
{
    public static class BatchAccount
    {
        public static  void CreateBatchProcessAsync()
        {
            const string BATCH_ACCOUNT_URL = "https://batchaccount0311.eastus2.batch.azure.com";
            const string BATCH_ACCOUNT_NAME = "batchaccount0311";
            const string BATCH_ACCOUNT_KEY = "ocVSWhETl66ttbpNsd5sn8AABC8a+q0RTN2o/8zUyDUglDPwg07tWJCqocMhN6gzeJ1qlTGSyS5N96FZx4wbWA==";
             const string POOL_ID = "DotNetQuickstartPool";
             const string JOB_ID = "DotNetQuickstartJob";
             const int POOL_NODE_COUNT = 1;
              const string POOL_VM_Size = "STANDARD_A1_v2";
            Stopwatch timer = new Stopwatch();
            timer.Start();
          

          
            BatchSharedKeyCredentials batchCredentials = new BatchSharedKeyCredentials(BATCH_ACCOUNT_URL, BATCH_ACCOUNT_NAME, BATCH_ACCOUNT_KEY);
            using (BatchClient batchClient = BatchClient.Open(batchCredentials))
            {
                //create the compute node pool
                Console.WriteLine($"Creating pool {POOL_ID}...");
                //vm configuration and batch pool
                ImageReference imageReference = new ImageReference(
                    publisher: "MicrosoftWindowsServer",
                    offer: "WindowsServer",
                    sku: "2012-R2-DataCenter",
                    version: "latest"
                    );
               
                VirtualMachineConfiguration vmConfiguration = new VirtualMachineConfiguration(
                    imageReference: imageReference,
                    nodeAgentSkuId: "batch.node.windows amd64");
                //create the batch pool

                try
                {
                    CloudPool pool = batchClient.PoolOperations.CreatePool(
                        poolId: POOL_ID,
                        targetDedicatedComputeNodes: POOL_NODE_COUNT,
                        virtualMachineSize: POOL_VM_Size,
                        virtualMachineConfiguration: vmConfiguration);
                    pool.ApplicationPackageReferences = new List<ApplicationPackageReference> { new ApplicationPackageReference { ApplicationId = "TaskApplication", Version = "1" } };
                    pool.Commit();

                }
                catch (BatchException be)
                {
                    if (be.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.PoolExists)
                    {
                        Console.WriteLine("The pool {0} already exists", POOL_ID);
                    }
                    else
                    {
                        throw;
                    }
                }
                var resourceFile = StorageAccount.GetResourceFile();
                //create the actual batch job
                Console.WriteLine($"Creating Job {JOB_ID}");
                try
                {
                    CloudJob batchJob = batchClient.JobOperations.CreateJob();
                    batchJob.Id = JOB_ID;
                    batchJob.PoolInformation = new PoolInformation { PoolId = POOL_ID };
                    batchJob.OnAllTasksComplete = OnAllTasksComplete.TerminateJob;
                    batchJob.Commit();
                }
                catch (BatchException be)
                {
                    if (be.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.JobExists)
                    {
                        Console.WriteLine($"The job {JOB_ID} already exists");
                    }
                    else
                    {
                        throw;
                    }
                }

                //create task
                List<CloudTask> tasks = new List<CloudTask>();

                for(int i = 0; i < resourceFile.Count; i++)
                {
                    string taskId = $"Task{i}";
                    string imageFileName = resourceFile[i].FilePath;
                    string taskCommandLine = String.Format("cmd /c %AZ_BATCH_NODE_SHARED_DIR%\\TaskApplication.exe {0} 3 lokeshbatchstorage zN3s4yNWI+TSahs53dIaS0c5qUZFtLc7mWXIKiHgBzU3ugXav1u2QfkHVU2UTchQb25nRcjLBfCcGCtTsS450g== output {1}", imageFileName, imageFileName);
                    CloudTask task = new CloudTask(taskId, taskCommandLine);
                    task.ResourceFiles = new List<ResourceFile> { resourceFile[i] };
                    tasks.Add(task);
                }

                //add all tasks to job
                batchClient.JobOperations.AddTask(JOB_ID, tasks);
                // Monitor task success/failure, specifying a maximum amount of time to wait for the tasks to complete.

                TimeSpan timeout = TimeSpan.FromMinutes(30);
                Console.WriteLine("Monitoring all tasks for 'Completed' state, timeout in {0}...", timeout);

                IEnumerable<CloudTask> addedTasks = batchClient.JobOperations.ListTasks(JOB_ID);

                batchClient.Utilities.CreateTaskStateMonitor().WaitAll(addedTasks, TaskState.Completed, timeout);

                Console.WriteLine("All tasks reached state Completed.");

                // Print task output
                Console.WriteLine();
                Console.WriteLine("Printing task output...");

                IEnumerable<CloudTask> completedtasks = batchClient.JobOperations.ListTasks(JOB_ID);

                foreach (CloudTask task in completedtasks)
                {
                    string nodeId = String.Format(task.ComputeNodeInformation.ComputeNodeId);
                    Console.WriteLine("Task: {0}", task.Id);
                    Console.WriteLine("Node: {0}", nodeId);
                    Console.WriteLine("Standard out:");
                    Console.WriteLine(task.GetNodeFile(Constants.StandardOutFileName).ReadAsString());
                }
                // Print out some timing info
                timer.Stop();
                Console.WriteLine();
                Console.WriteLine("Sample end: {0}", DateTime.Now);
                Console.WriteLine("Elapsed time: {0}", timer.Elapsed);

                // Clean up Storage resources
              

                // Clean up Batch resources (if the user so chooses)
                Console.WriteLine();
                Console.Write("Delete job? [yes] no: ");
                string response = Console.ReadLine().ToLower();
                if (response != "n" && response != "no")
                {
                    batchClient.JobOperations.DeleteJob(JOB_ID);
                }

                Console.Write("Delete pool? [yes] no: ");
                response = Console.ReadLine().ToLower();
                if (response != "n" && response != "no")
                {
                    batchClient.PoolOperations.DeletePool(POOL_ID);
                }


            }


        }
    }
}
