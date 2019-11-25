using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.Batch;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureLearn
{
    public static class StorageAccount
    {
        public static void ConnnectStorageAccount()
        {
            string storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName=lokeshbatchstorage;AccountKey=zN3s4yNWI+TSahs53dIaS0c5qUZFtLc7mWXIKiHgBzU3ugXav1u2QfkHVU2UTchQb25nRcjLBfCcGCtTsS450g==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            const string STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME = "images";
            CloudBlobContainer imagesContainer = blobClient.GetContainerReference(STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME);
            imagesContainer.CreateIfNotExistsAsync().Wait();

            List<string> inputFilePaths = new List<string>(Directory.GetFiles("/Users/lokesh/Projects/Azure/Azure/Images/"));
            List<ResourceFile> inputImages = new List<ResourceFile>();
            foreach (string imagePath in inputFilePaths)
            {

                //Console.WriteLine("Uploading file {0} to container [{1}]...", filePath,
                //STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME);
                string blobName = Path.GetFileName(imagePath);
                CloudBlobContainer container = blobClient.
                GetContainerReference(STORAGE_ACCOUNT_IMAGES_CONTAINER_NAME);
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
        }
    }
}
