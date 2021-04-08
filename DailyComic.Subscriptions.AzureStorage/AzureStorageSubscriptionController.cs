using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyComic.Contracts;
using DailyComic.Model;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace DailyComic.Subscriptions.AzureStorage
{
    public class AzureStorageSubscriptionController : ISubscriberProvider, ISubscriberRegister
    {
        private readonly string storageConnectionString;
        private readonly ILogger<AzureStorageSubscriptionController> logger;
        private readonly string subscriptionsTableName = "ComicSubscriptions";

        public AzureStorageSubscriptionController(string storageConnectionString, ILogger<AzureStorageSubscriptionController> logger)
        {
            this.storageConnectionString = storageConnectionString;
            this.logger = logger;
        }

        public async Task<IEnumerable<SubscriptionSettings>> GetSubscribers(SubscriptionName subscriptionName)
        {
            CloudTable table = await this.GetTable();
            TableContinuationToken token = null;
            List<SubscriptionSettings> entities = new List<SubscriptionSettings>();
            TableQuery<SubscriptionEntity> query = GetPartitionKeyQuery(subscriptionName);
            do
            {
                TableQuerySegment<SubscriptionEntity> queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results.Select(SubscriptionMapper.FromEntity));
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities.AsEnumerable();
        }

        public async Task AddSubscriber(SubscriptionSettings subscriptionSettings)
        {
            CloudTable table = await this.GetTable();

            SubscriptionEntity entity = SubscriptionMapper.FromSettings(subscriptionSettings);

            string filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, subscriptionSettings.SubscriptionName.ToString()),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(SubscriptionEntity.WebhookUrl), QueryComparisons.Equal, subscriptionSettings.WebhookUrl)
            );

            TableQuerySegment<SubscriptionEntity> existingHookQueryResult = await table.ExecuteQuerySegmentedAsync(new TableQuery<SubscriptionEntity>().Where(filter), null);
            if (existingHookQueryResult.Results.Any())
            {
                throw new InvalidOperationException("Specified webhook is already registered for this comic subscription.");
            }

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
            SubscriptionEntity inserted = result.Result as SubscriptionEntity;
            if (result.HttpStatusCode != 204)
            {
                throw new InvalidOperationException($"Failed to insert subscriber. Response: {result.HttpStatusCode}");
            }
        }

        private static TableQuery<SubscriptionEntity> GetPartitionKeyQuery(SubscriptionName subscriptionName)
        {
            TableQuery<SubscriptionEntity> query = new TableQuery<SubscriptionEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                    subscriptionName.ToString()));
            return query;
        }

        private CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException e)
            {
                this.logger.LogError(e, "Invalid storage account information provided.");
                throw;
            }
            catch (ArgumentException e)
            {
                this.logger.LogError(e, "Invalid storage account information provided.");
                throw;
            }

            return storageAccount;
        }

        private async Task<CloudTable> GetTable()
        {
            CloudStorageAccount account = CreateStorageAccountFromConnectionString();
            CloudTableClient tableClient = account.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(this.subscriptionsTableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

    }
}
