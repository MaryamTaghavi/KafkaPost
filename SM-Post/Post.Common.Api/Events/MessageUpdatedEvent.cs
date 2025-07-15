using CQRS.Core.Events;

namespace Post.Common.Api.Events; 

public class MessageUpdatedEvent : BaseEvent
{
    public MessageUpdatedEvent() : base(nameof(MessageUpdatedEvent))
    {
    }
    public string Message { get; set; } = "";
}
