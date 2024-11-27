namespace Nvovka.CommandManager.Api.Dto;

public class CommandItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public IEnumerable<CommandReferenceDto> Items { get; set; }

}
