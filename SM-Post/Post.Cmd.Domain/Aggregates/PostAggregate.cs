using CQRS.Core.Domain;
using Microsoft.AspNetCore.Builder;
using Post.Common.Api.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string author;

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
}
