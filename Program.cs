using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SuperFunction.Contexts;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((config, services) =>
    {
        services.AddDbContext<CosmosContext>(x => x.UseCosmos(config.Configuration.GetConnectionString("CosmosDb"), "iotdbbb"));
    })
    .Build();

host.Run();