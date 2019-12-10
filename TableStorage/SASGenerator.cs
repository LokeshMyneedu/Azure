
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using System;

namespace TableStorage
{
    public static class SASGenerator
    {
        public static string GetTableSASToken(string tableName,string accountName, string key)
        {
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName=az203lokeshstorage;AccountKey=sHmBt4HPkkt00QwIAgt+X168Wm8d4c4NEs+QCnXEV5Q1XTnwi6AebrjC6vHrox41EmZT9hbRMvpxcP/YeGctFA==;EndpointSuffix=core.windows.net";  
              CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);

            SharedAccessTablePolicy tablePolicy = new SharedAccessTablePolicy();
            
            tablePolicy.SharedAccessStartTime = DateTimeOffset.UtcNow;
            tablePolicy.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);
            tablePolicy.Permissions = SharedAccessTablePermissions.Query | SharedAccessTablePermissions.Update | SharedAccessTablePermissions.Delete | SharedAccessTablePermissions.Add;
            return table.GetSharedAccessSignature(tablePolicy);
        }
    }
}
