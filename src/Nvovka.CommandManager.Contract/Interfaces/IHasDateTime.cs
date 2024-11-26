
namespace Nvovka.CommandManager.Contract.Interfaces;

public interface IHasDateTime
{
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

