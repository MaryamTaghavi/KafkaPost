﻿using CQRS.Core.Domain;
using CQRS.Core.Messages;
using Microsoft.AspNetCore.Builder;
using Post.Common.Api.Events;
using System.ComponentModel.Design;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;

    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    // TODO :

    // public bool Active
    // {
    //   get {} ; set {} ;
    // }

    // تفاوت تعریف متد در دو حالت این است که در حالت بالایی
    // کامپایلر یک فیلد پشتیبان پنهان میسازد که ما به آن دسترسی
    // نداریم و نمیتوانیم آن رو کنترل کنیم
    // ولی در حالت پایینی کاملا به آن مسلط و امکان تغییر آن را داریم

    // set
    // {
    //      if (value != _active)
    //      {
    //          Console.WriteLine("Active changed!");
    //          _active = value;
    //      }
    //}

    public bool Active
    {
        get => _active; set => _active = value;
    }

    public PostAggregate()
    {

    }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.Now
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _active = true;
        _author = @event.Author;
    }

    public void EditMessage(string message)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot edit message of an inactive post!");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException($"The value of {nameof(message)} can not be null or empty." +
                                                $" Please provide a valid {nameof(message)}!");
        }

        RaiseEvent(new MessageUpdatedEvent()
        {
            Id = _id,
            Message = message
        });
    }

    public void Apply(MessageUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    public void LikePost()
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot like inactive post!");
        }

        RaiseEvent(new PostLikedEvent()
        {
            Id = _id,
        });
    }

    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;
    }

    public void AddComment(string comment , string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot add a comment to an inactive post!");
        }


        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException($"The value of {nameof(comment)} can not be null or empty." +
                                                $" Please provide a valid {nameof(comment)}!");
        }

        RaiseEvent(new CommentAddedEvent()
        {
            Id = _id,
            CommentId = Guid.NewGuid() ,
            Comment = comment,
            Username = username,
            CommentDate = DateTime.Now
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId , new Tuple<string, string>(@event.Comment , @event.Username));
    }

    public void EditMessage(Guid commentId , string comment , string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot edit a comment to an inactive post!");
        }

        if (_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to edit the comment that was made by another user!");
        }

        RaiseEvent(new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId ,
            Comment = comment,
            Username = username,
            EditedDate = DateTime.Now,
            
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot remove a comment to an inactive post!");
        }

        if (_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to remove the comment that was made by another user!");
        }

        RaiseEvent(new CommentRemovedEvent()
        {
            Id = _id,
            CommentId = commentId,
        });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("The post has already been removed!");
        }

        if (!_author.Equals(username , StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to delete a post that was made by someone else!");
        }

        RaiseEvent(new PostRemovedEvent()
        {
            Id = _id,
        });
    }

    public void Apply(PostRemovedEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }
}
