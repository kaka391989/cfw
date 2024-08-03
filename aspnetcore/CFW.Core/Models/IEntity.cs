namespace CFW.Core.Models
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
