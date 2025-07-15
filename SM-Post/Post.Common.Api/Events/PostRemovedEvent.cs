using CQRS.Core.Events;

namespace Post.Common.Api.Events; 

public class PostRemovedEvent : BaseEvent
{
    public PostRemovedEvent() : base(nameof(PostRemovedEvent))
    {
    }
}