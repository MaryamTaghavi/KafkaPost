﻿using CQRS.Core.Events;

namespace Post.Common.Api.Events; 

public class CommentRemovedEvent : BaseEvent
{
    public CommentRemovedEvent() : base(nameof(CommentRemovedEvent))
    {
    }
    public Guid CommentId { get; set; } 
}