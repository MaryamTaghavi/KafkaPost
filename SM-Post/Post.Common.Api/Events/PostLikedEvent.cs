using CQRS.Core.Events;

namespace Post.Common.Api.Events; 

public class PostLikedEvent : BaseEvent
{
    public PostLikedEvent() : base(nameof(PostLikedEvent))
    {
    }
}
