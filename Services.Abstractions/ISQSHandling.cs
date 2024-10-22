using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface ISQSHandling
    {
        public Task<bool> PublishMessageToPublisher(Guid messageId, int delaySeconds);
    }
}
