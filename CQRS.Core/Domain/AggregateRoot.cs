using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    protected Guid _id;
    private readonly List<BaseEvent> _changes = new() ;

    public Guid Id
    { 
        get { return _id; } 
    }

    // Control Concurrency
    // مقدار اولیه -1 است یعنی هیچ رویدادی اضافه نشده است
    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUnCommitedChanges()
    {
        return _changes ;
    }

    public void MarkChangesAsCommited()
    {
        _changes.Clear();
    }

    private void ApplyChange(BaseEvent @event , bool isNew)
    {
        var method = this.GetType().GetMethod("Apply" , new Type[] { @event.GetType() });

        if(method == null)
        {
            throw new ArgumentNullException(nameof(method) , $"The Apply Method was not found in the aggregate for {@event.GetType().Name}!");
        }

        method.Invoke(this, new object[] { @event });

        if (isNew)
        {
            _changes.Add( @event );
        }
    }

    // ایجاد رویداد جدید که به لیست اضافه میشوند
    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    //مسئول بازسازی ایونت ها را دارند و به تغییرات اضافه نمیشوند
    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach(var @event in events) 
        {
            ApplyChange(@event, false);
        }
    }
}
