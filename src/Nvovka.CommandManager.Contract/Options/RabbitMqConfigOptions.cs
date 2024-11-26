namespace Nvovka.CommandManager.Contract.Options;

public class RabbitMqConfigOptions
{
    public const string SectionName = "RabbitMq";
    public string HostName { get; set; } = "localhost";

    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public bool BatchPublish { get; set; }
}
