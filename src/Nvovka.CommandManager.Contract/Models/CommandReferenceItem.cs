using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Nvovka.CommandManager.Contract.Interfaces;

namespace Nvovka.CommandManager.Contract.Models;

public class CommandReferenceItem : IEntity, IHasDateTime
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("CommandItemId")]
    public required int CommandItemId { get; set; }

    public required string Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
