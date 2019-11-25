using System;
using System.Collections.Generic;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;

namespace AzureLearn
{
    public static class BatchAccount
    {
        public static void CreateBatchProcess()
        {
            const string BATCH_ACCOUNT_URL = "https://lokeshbatch.eastus2.batch.azure.com";""
            const string BATCH_ACCOUNT_NAME = "BatchAccount";
            const string BATCH_ACCOUNT_KEY = "LPSYiCpcyMR8XRojRYt0asDtCFOwSHxkLeUa1Hs6VUmy3M92abTBaHxTR7moIFlauL2NKSZzaglU47/Pw2zbfg==";
           //We start by getting a Batch Account client.
           BatchSharedKeyCredentials batchCredentials = new BatchSharedKeyCredentials(BATCH_ACCOUNT_URL, BATCH_ACCOUNT_NAME, BATCH_ACCOUNT_KEY);
            using (BatchClient batchClient = BatchClient.Open(batchCredentials))
            {
                //create the compute node pool
                Console.WriteLine($"Creating pool {POOL_ID}...");
                //vm configuration and batch pool
                ImageReference imageReference = new ImageReference(
                    publisher: "Canonical",
                    offer: "UbuntuServer",
                    sku: "18.04-LTS",
                    version: "latest"
                    );
                VirtualMachineConfiguration vmConfiguration = new VirtualMachineConfiguration(
                    imageReference: imageReference,
                    nodeAgentSkuId: "batch.node.ubuntu 18.04");
                //create the batch pool

                try
                {
                    CloudPool pool = batchClient.PoolOperations.CreatePool(
                        poolId: POOL_ID,
                        targetDedicatedComputeNodes: POOL_NODE_COUNT,
                        virtualMachineSize: POOL_VM_Size,
                        virtualMachineConfiguration: vmConfiguration);
                    pool.Commit();

                }catch(BatchException be)
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
                //create the actual batch job
                Console.WriteLine($"Creating Job {JOB_ID}");
                try
                {
                    CloudJob job = batchClient.JobOperations.CreateJob();
                    job.Id = JOB_ID;
                    job.PoolInformation = new PoolInformation { PoolId = POOl_ID };
                    job.Commit();
                }catch(BatchException be)
                {
                    if(be.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.JobExists)
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

            }


        }
    }
}
