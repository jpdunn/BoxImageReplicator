using BoxImageReplicator;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		Console.WriteLine("Creating host builder services");

		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();

		services
			.AddOptions<MySettings>()
			.Configure<IConfiguration>(
				(settings, configuration) =>
				{
					configuration.GetSection("MySettings").Bind(settings);
				}
			);

		services.AddTransient<BoxImageUploadWorker>();

		Console.WriteLine("Created host builder services");
	})
	.Build();

host.Run();
