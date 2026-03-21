
using Microsoft.Azure.Cosmos;
using WebApi.Repositories;

namespace WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.ConfigureRepositories();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint("/openapi/v1.json", "Queue Listener API V1"); // /swagger/index
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static WebApplicationBuilder ConfigureRepositories(this WebApplicationBuilder builder)
        {
            var cosmosConnectionString = builder.Configuration.GetConnectionString("CosmosDb")
                ?? throw new InvalidOperationException("Cannot find Cosmos connection string");

            var opt = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
            };

            var cosmosClient = new CosmosClient(cosmosConnectionString, opt);

            var repositoryFactory = new RepositoryFactory(cosmosClient);

            builder.Services.AddSingleton(repositoryFactory.CreateImageRepository());

            return builder;
        }
    }
}
