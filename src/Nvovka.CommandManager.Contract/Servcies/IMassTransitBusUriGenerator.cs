using Microsoft.Extensions.Options;
using Nvovka.CommandManager.Contract.Options;

namespace Nvovka.CommandManager.Contract.Servcies;

public interface IMassTransitBusUriGenerator
{
    public Uri GetBusUri(string queueName);
}

public class MassTransitBusUriGenerator : IMassTransitBusUriGenerator
{
    private readonly RabbitMqConfigOptions _options;
    public MassTransitBusUriGenerator(IOptions<RabbitMqConfigOptions> options)
    {
        _options = options.Value;
    }
    public Uri GetBusUri(string? queueName)
    {
        Uri baseUri = _options.Url;
        return new Uri(baseUri, queueName);
    }
}
