using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableStorage.Model;

namespace TableStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            AppSettings appSettings = AppSettings.LoadAppSettings();
            var tableName = appSettings.TableName;
            var storageName = appSettings.StorageAccountName;
            var accessKey = appSettings.AccessKey;
            var sasToken = SASGenerator.GetTableSASToken(tableName, storageName, accessKey);
            CloudTable table = Task.Run(async () => await CommonTableAccount.CreateTableAsync
             (tableName, sasToken)).GetAwaiter().GetResult();
            try
            {
                // Demonstrate basic CRUD functionality
                Task.Run(async () => await CreateDemoDataAsync(table)).Wait();
            }
            finally
            {
                // Delete the table
                // await table.DeleteIfExistsAsync();
            }
        }

                private static async Task CreateDemoDataAsync(CloudTable table)
                {
                    // Create an instance of a person entity. See the Model\personEntity.cs for
                    //a description of the entity.
                    PersonEntity person = new PersonEntity("Fernández", "Santiago")
                    {
                        Email = "santiago.fernandez@contoso.com",
                        PhoneNumber = "123-555-0101"
                    };

            // Demonstrate how to insert the  entity
            Console.WriteLine($"Inserting person: {person.PartitionKey}, {person.RowKey}");
            person = await TableUtils.InsertOrMergeEntityAsync(table, person);

            // Demonstrate how to Update the entity by changing the phone number
            Console.WriteLine("Update an existing Entity using the InsertOrMerge Upsert   Operation.");
            person.PhoneNumber = "123-555-0105";
            await TableUtils.InsertOrMergeEntityAsync(table, person);
            Console.WriteLine();

            //Insert new people with same partition keys.
            //If you try to use a batch operation for inserting entities with different
            //partition keys you get an exception.
            var people = new List<PersonEntity>();

            person = new PersonEntity("Smith", "John")
            {
                Email = "john.smith@contoso.com",
                PhoneNumber = "123-555-1111"
            };
            people.Add(person);

            person = new PersonEntity("Smith", "Sammuel")
            {
                Email = "sammuel.smith@contoso.com",
                PhoneNumber = "123-555-2222"
            };
            people.Add(person);

            TableBatchResult insertedPeopleResult = new TableBatchResult();
            insertedPeopleResult = await TableUtils.BatchInsertOrMergeEntityAsync(table,
            people);

            foreach (var res in insertedPeopleResult)
            {
                PersonEntity batchPerson = res.Result as PersonEntity;
                Console.WriteLine($"Inserted person in a batch operation:{ batchPerson.PartitionKey}, { batchPerson.RowKey}");
            }
            // Demonstrate how to Read the updated entity using a point query
            Console.WriteLine("Reading the updated Entity.");
            person = await TableUtils.RetrieveEntityUsingPointQueryAsync(table,
            "Fernández", "Santiago");
            Console.WriteLine();

            // Demonstrate how to Delete an entity
            //Console.WriteLine("Delete the entity. ");
            //await SamplesUtils.DeleteEntityAsync(table, person);
            //Console.WriteLine();

            TableQuery<PersonEntity> query = new TableQuery<PersonEntity>()
    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
    "Smith"));

            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<PersonEntity> resultSegment = await table.ExecuteQuerySegmentedAsync
                <PersonEntity>(query, token);
                token = resultSegment.ContinuationToken;

                foreach (PersonEntity personSegment in resultSegment.Results)
                {

                    Console.WriteLine($"Last Name: \t{personSegment.PartitionKey}\n" +
         $"First Name:\t{personSegment.RowKey}\n" +
         $"Email:\t{personSegment.Email}\n" +
         $"Phone Number:\t{personSegment.PhoneNumber}");
                    Console.WriteLine();
                }
            } while (token != null);

           
                    }
    }
}


            











