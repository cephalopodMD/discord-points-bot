using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CommandsFunction.Actions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Polly;
using Polly.Retry;

namespace CommandsFunction
{
    public class CommandIntake
    {
        private readonly Func<JsonDocument, IAction> _actionFactory;
        
        private static readonly TimeSpan LeaseSpan = new TimeSpan(0, 0, 1, 0);
        private static readonly AsyncRetryPolicy BlobAccessRetryPolicy = Policy
            .Handle<StorageException>()
            .WaitAndRetryAsync(5,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        public CommandIntake(Func<JsonDocument, IAction> actionFactory)
        {
            _actionFactory = actionFactory;
        }

        [FunctionName("CommandIntake")]
        public async Task Run([ServiceBusTrigger("commands", Connection = "PointsBotQueueConnection")]string commandPayload, 
            [Blob("bot/users.json", FileAccess.ReadWrite, Connection = "Data")] CloudBlockBlob gameStateBlob,
            ILogger log)
        {
            var leaseId = Guid.NewGuid().ToString();
            var lease = await BlobAccessRetryPolicy.ExecuteAsync(() => gameStateBlob.AcquireLeaseAsync(LeaseSpan, leaseId));

            try
            {
                var gameState = await gameStateBlob.DownloadTextAsync();
                var game = new Game(gameState);

                var command = JsonDocument.Parse(commandPayload);
                var action = _actionFactory(command);

                action.Execute(game);

                await BlobAccessRetryPolicy.ExecuteAsync(() => gameStateBlob.UploadTextAsync(JsonSerializer.Serialize(game.Players), new AccessCondition{ LeaseId = lease }, new BlobRequestOptions(), new OperationContext()));
            }
            finally
            {
                await BlobAccessRetryPolicy.ExecuteAsync(() => gameStateBlob.ReleaseLeaseAsync(new AccessCondition { LeaseId = lease }));
            }
        }
    }
}
