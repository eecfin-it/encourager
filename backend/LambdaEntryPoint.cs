using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Encourager.Api;

public class LambdaEntryPoint : APIGatewayProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder.UseStartup<Startup>();
    }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        AppConfiguration.ConfigureServices(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseCors();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            AppConfiguration.ConfigureEndpoints(endpoints);
        });
    }
}
