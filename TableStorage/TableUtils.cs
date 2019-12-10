using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TableStorage.Model;

namespace TableStorage
{
    public static class TableUtils
    {
        public static async Task<PersonEntity> InsertOrMergeEntityAsync(CloudTable table,
        PersonEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.
                InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                PersonEntity insertedCustomer = result.Result as PersonEntity;

                return insertedCustomer;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);

                Console.ReadLine();
                throw;
            }
        }

        public static async Task<TableBatchResult> BatchInsertOrMergeEntityAsync
        (CloudTable table, IList<PersonEntity> people)
        {
            if (people == null)
            {
                throw new ArgumentNullException("people");
            }
            try
            {
                TableBatchOperation tableBatchOperation = new TableBatchOperation();

                foreach (PersonEntity person in people)
                {
                    tableBatchOperation.InsertOrMerge(person);
                }

                TableBatchResult tableBatchResult = await table.ExecuteBatchAsync
                (tableBatchOperation);

                return tableBatchResult;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();
                throw;
            }
        }
        public static async Task<PersonEntity> RetrieveEntityUsingPointQueryAsync
        (CloudTable table, string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<PersonEntity>
                (partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                PersonEntity person = result.Result as PersonEntity;
                if (person != null)
                {
                    Console.WriteLine($"Last Name: \t{person.PartitionKey}\n" +
                                        $"First Name:\t{person.RowKey}\n" +
                                        $"Email:\t{person.Email}\n" +
                                        $"Phone Number:\t{person.PhoneNumber}");
                }

                return person;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public static async Task DeleteEntityAsync(CloudTable table,
        PersonEntity deleteEntity)
        {
            try
            {
                if (deleteEntity == null)
                {
                    throw new ArgumentNullException("deleteEntity");
                }

                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                TableResult result = await table.ExecuteAsync(deleteOperation);

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);

                Console.ReadLine();
                throw;
            }
        }
    }
}









