using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;


namespace TaskApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            TopNWordsTask.TaskMain(args);
        }



    /// <summary>
    /// This class has the code for each task. The task reads the
    /// blob assigned to it and determine TopNWords and writes
    /// them to standard out
    /// </summary>
    public class TopNWordsTask
    {
            
            public static  void TaskMain(string[] args)
        {
           
            if (args == null || args.Length != 6)
            {
                Exception e = new Exception("Usage: TopNWordsSample.exe --Task <blobpath> <numtopwords> <storageAccountName> <storageAccountKey>");
                throw e;
            }

            string blobName = args[0];
            int numTopN = int.Parse(args[1]);
            string storageAccountName = args[2];
            string storageAccountKey = args[3];
            string outputBlob = args[4];
            string fileName = args[5];

            using (WordCount wordCounter = new WordCount())
            {
                wordCounter.CountWords(blobName, numTopN, storageAccountName, storageAccountKey,outputBlob,fileName);
            }
        }
    }

    public class WordCount : IDisposable
    {
            bool disposed = false;
            public void CountWords(string blobName, int numTopN, string storageAccountName, string storageAccountKey,string outputContainerName,string fileName)
        {


                // open the cloud blob that contains the book
                 string storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";
                 CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                 CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("images");
                
                    CloudBlob blob = container.GetBlockBlobReference("SampleTextFile_50kb.txt");
                      using (Stream memoryStream = new MemoryStream())
                    {
                        // Find blob download time
                        DateTime start = DateTime.Now;
                       
                         blob.DownloadToStreamAsync(memoryStream).Wait();
                         memoryStream.Position = 0;



                    Dictionary<String, Double> topWordsMetrics = new Dictionary<string, double>();
                            using (StreamReader sr = new StreamReader(memoryStream))
                            {
                                var myStr = sr.ReadToEnd();
                                string[] words = myStr.Split(' ');
                                var topNWords =
                                    words.
                                     Where(word => word.Length > 0).
                                     GroupBy(word => word, (key, group) => new KeyValuePair<String, long>(key, group.LongCount())).
                                     OrderByDescending(x => x.Value).
                                     Take(numTopN).
                                     ToList();

                                foreach (var pair in topNWords)
                                {
                                    Console.WriteLine("{0} {1}", pair.Key, pair.Value);
                                    topWordsMetrics.Add(pair.Key, pair.Value);
                                }

                                UploadOutPutFile(storageAccountName, storageAccountKey, outputContainerName, topWordsMetrics, fileName);
                            }
                        }
                    
           

               
            }

            public static void UploadOutPutFile(string storageAccountName, string storageAccountKey,string containerName, Dictionary<String, Double> words,string fileName)
            {

                 string storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";
                 CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                 CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer outPutContainer = blobClient.GetContainerReference(containerName);
                outPutContainer.CreateIfNotExistsAsync().Wait();

                string word = string.Join(";", words.Select(x => x.Key + "and the count is" + x.Value).ToArray());
                CloudBlockBlob blobData = outPutContainer.GetBlockBlobReference($"{fileName}");
                blobData.UploadTextAsync(word).Wait();                
            }



            public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Console.WriteLine("Flush client events");      

                // allow insights client to write out
                Console.WriteLine("Waiting for insights to emit logs");
                System.Threading.Thread.Sleep(5000);
            }

            disposed = true;
        }
    }
}
}
