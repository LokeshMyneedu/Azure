using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Batch;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureLearn
{
    public static class StorageAccount
    {
        static string storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName=lokeshbatchstorage1;AccountKey=5dwFcui4lJtwgnXVdt9gj9TAuHucIccUbBPQrTuVL/kF3L7Lf1KiBHDfxrx2630FKK7+rHZH6uEU8jWwFprhHA==;EndpointSuffix=core.windows.net";
        static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        static CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        const string STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME = "textfiles";
        static List<string> inputFilePaths = new List<string>(Directory.GetFiles("C:/Git/Azure/Azure/TextFiles/"));


        public static void ConnnectStorageAccount()
        {

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference(STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME);
            imagesContainer.CreateIfNotExistsAsync().Wait();

            List<string> inputFilePaths = new List<string>(Directory.GetFiles("C:/Git/Azure/Azure/TextFiles/"));

            foreach (string imagePath in inputFilePaths)
            {

                //Console.WriteLine("Uploading file {0} to container [{1}]...", filePath,
                //STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME);
                string blobName = Path.GetFileName(imagePath);
                CloudBlobContainer container = blobClient.
                GetContainerReference(STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME);
                CloudBlockBlob blobData = container.GetBlockBlobReference(blobName);
                blobData.UploadFromFileAsync(imagePath).Wait();
            }
        }

        public static List<ResourceFile> GetResourceFile()
        {
            List<ResourceFile> inputImages = new List<ResourceFile>();
            List<string> inputFilePaths = new List<string>(Directory.GetFiles("C:/Git/Azure/Azure/TextFiles/"));

            foreach (string imagePath in inputFilePaths)
            {

                //Console.WriteLine("Uploading file {0} to container [{1}]...", filePath,
                //STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME);
                string blobName = Path.GetFileName(imagePath);
                CloudBlobContainer container = blobClient.GetContainerReference(STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME);                
                CloudBlockBlob blobData = container.GetBlockBlobReference(blobName);
                blobData.UploadFromFileAsync(imagePath).Wait();
                //We access the storage account by using a Shared Access Signature (SAS) token.
                //You need to start the upload operation as soon as possible, so we set no start
                //time for making the token immediately available.
                SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
                {
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(2),
                    Permissions = SharedAccessBlobPermissions.Read
                };
                // Construct the SAS URL for blob
                string sasBlobToken = blobData.GetSharedAccessSignature(sasConstraints);

                string blobSasUri = String.Format("{0}{1}", blobData.Uri, sasBlobToken);
                //ResourceFile resourceFile = new ResourceFile(blobSasUri, blobName);

                inputImages.Add(ResourceFile.FromUrl(blobSasUri, blobName));
            }
            return inputImages;
        }

      
       
    }
}
