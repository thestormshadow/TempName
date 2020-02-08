using BackEnd.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.EntitiesManager;

namespace BackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            Extensions.AddMongoDBEntities(
                services
                , "mongodb://mexalabsadmin:K27GEkI6oalF7OqN@dcenter-shard-00-00-trvmd.mongodb.net:27017,dcenter-shard-00-01-trvmd.mongodb.net:27017,dcenter-shard-00-02-trvmd.mongodb.net:27017/test?ssl=true&replicaSet=DCenter-shard-0&authSource=admin&retryWrites=true&w=majority"
                , "DBCentral"
            );

            IoC.addRegistration(services);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
