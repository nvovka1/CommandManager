using Nvovka.CommandManager.Worker;

////var builder = Host.CreateApplicationBuilder(args);
////builder.Services.AddHostedService<Worker>();

////var host = builder.Build();
////var startup = new Startup(hostContext.Configuration);
////host.Run();



public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
       .ConfigureAppConfiguration((context, config) =>
       {
           config.AddJsonFile("appsettings.json", optional: true);
           config.AddEnvironmentVariables();
       })
       .ConfigureServices((hostContext, services) =>
       {
           var startup = new Startup(hostContext.Configuration);
           startup.ConfigureServices(services);
       });
}