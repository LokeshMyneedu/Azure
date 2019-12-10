using Microsoft.Azure.Cosmos.Table;
using System;

using System.Threading.Tasks;

namespace TableStorage
{
    public static class CommonTableAccount
    {
        public static CloudStorageAccount CreateStorageAccountFromSASToken(string SASToken,
        string accountName)
        {
            CloudStorageAccount storageAccount;
            try
            {
                //We required that the communication  with the storage service uses HTTPS.
                bool useHttps = true;
                StorageCredentials storageCredentials = new StorageCredentials(SASToken);
                storageAccount = new CloudStorageAccount(storageCredentials, accountName, null, useHttps);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid Storage Account information provided. Please confirm the SAS Token is valid and did not expire");
                throw;
            }
            catch (ArgumentException)
            {

                Console.WriteLine("Invalid Storage Account information provided. Please confirm the SAS Token is valid and did not expire");
  
                  Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        public static async Task<CloudTable> CreateTableAsync(string tableName,string SASToken)
        {
            AppSettings appSettings = AppSettings.LoadAppSettings();
            string storageConnectionString = SASToken;
            string accountName = appSettings.StorageAccountName;

            CloudStorageAccount storageAccount = CreateStorageAccountFromSASToken
             (storageConnectionString, accountName);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new
            TableClientConfiguration());

           // Console.WriteLine($"Creating the table {tableName}");

            // Create a table client for interacting with the table service
            CloudTable table = tableClient.GetTableReference(tableName);
            //if (await table.CreateIfNotExistsAsync())
            //{
            //    Console.WriteLine($"Created Table named: {tableName}");
            //}
            //else
            //{
            //    Console.WriteLine($"Table {tableName} already exists");
            //}

            Console.WriteLine();
            return table;
        }
    }
}


            


 