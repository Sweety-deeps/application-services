using Amazon.SQS;
using Domain;
using Microsoft.Extensions.Logging;
using Services.Abstractions;

namespace Services
{
    public class SQSHandling : ISQSHandling
    {
        private readonly ILogger<SQSHandling> _logger;

        public SQSHandling(ILogger<SQSHandling> logger)
        {
            _logger = logger;
        }

        public async Task<bool> PublishMessageToPublisher(Guid messageId, int delaySeconds)
        {
            try
            {
                // Todo: Use DI to inject aws sqs client 
                var sqsClient = new AmazonSQSClient();
                var queueUrl = Environment.GetEnvironmentVariable(Constants.ENVIRONMENT_VARIABLES_PUBLISHERSQS);                
                
                var messageRequest = new Amazon.SQS.Model.SendMessageRequest
                {
                    QueueUrl = queueUrl,
                    MessageBody = messageId.ToString(),
                    DelaySeconds = delaySeconds
                };

                var sqsResponse = await sqsClient.SendMessageAsync(messageRequest);

                _logger.LogInformation("Sqs response is received as StatusCode {statusCode}, Message Id: {messageId}", sqsResponse.HttpStatusCode, sqsResponse.MessageId);
                return sqsResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while publishing a message using SQS client. {ex}", ex);
                return false;
            }
        }
    }
}
