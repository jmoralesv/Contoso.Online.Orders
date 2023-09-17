using AdaptiveCards.Templating;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace ContosoOnlineOrders.Functions
{
    public class InventoryNotifier
    {
        private readonly ILogger logger;

        public InventoryNotifier(ILogger logger)
        {
            this.logger = logger;
        }

        [OpenApiOperation(operationId: nameof(SendLowStockNotification), Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Product[]), Required = true)]
        [Function(nameof(SendLowStockNotification))]
        public static async Task<HttpResponseData> SendLowStockNotification(
            [HttpTrigger(AuthorizationLevel.Anonymous, WebRequestMethods.Http.Post, Route = null)] HttpRequestData request)
        {
            // variable for holding the json data during card-templating
            var json = string.Empty;

            // create the meta for the card
            var meta = new CardMetadata
            {
                Title = "Low Inventory Notification",
                Description = "The products below are low on inventory and should be re-stocked before customers are impacted.",
                CreatedUtc = DateTime.UtcNow,
                Creator = new Creator
                {
                    Name = "Inventory Robot",
                    ProfileImage = "https://github.com/dotnet/brand/blob/main/dotnet-bot-illustrations/dotnet-bot/dotnet-bot.png?raw=true"
                }
            };

            // open the card template
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ContosoOnlineOrders.Functions.InventoryNotificationCard.json"))
            {
                using StreamReader rdr = new(stream);
                json = await rdr.ReadToEndAsync();
            }

            // add all the products that need re-stocking to the card
            meta.Products = JsonSerializer.Deserialize<List<Product>>(await request.ReadAsStringAsync())
                .Where(p => p.InventoryCount <= 5)
                .ToList();

            // render the card
            AdaptiveCardTemplate template = new(json);
            json = template.Expand(meta);

            // return the result
            var response = request.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(json);
            return response;
        }
    }
}
