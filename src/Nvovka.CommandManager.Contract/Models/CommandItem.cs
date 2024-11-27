using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Nvovka.CommandManager.Contract.Interfaces;
using Nvovka.CommandManager.Contract.Enums;

namespace Nvovka.CommandManager.Contract.Models;

public class CommandItem : IHasDateTime, IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public CommandStatus Status { get; set; }

    public virtual List<CommandReferenceItem>? CommandReferenceItems { get; set; } = new List<CommandReferenceItem>();

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
