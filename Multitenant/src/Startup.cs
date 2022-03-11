using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using src.Data;
using src.Data.Interceptors;
using src.Data.ModelFactory;
using src.Domain;
using src.Extensions;
using src.Middlewares;
using src.Provider;

namespace EFCore.Multitenant
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
            services.AddScoped<TenantData>();
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EFCore.Multitenant", Version = "v1" });
            });
            

            // Estrategia 1 - Identificador na tabela
            /*
            services.AddScoped<StrategySchemaInterceptor>();
            
            services.AddDbContext<ApplicationContext>(p=>p
                .UseSqlServer("Data source=(localdb)\\mssqllocaldb; Initial Catalog=Tenant99;Integrated Security=true;")
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging());
            */
            
            // Estrategia 2 - Schema
            /*services.AddDbContext<ApplicationContext>((provider,options)=>
            {
                options
                    .UseSqlServer("Data source=(localdb)\\mssqllocaldb; Initial Catalog=Tenant99;Integrated Security=true;")
                    .LogTo(Console.WriteLine)
                    .ReplaceService<IModelCacheKeyFactory, StrategySchemaModelCacheKey>()
                    .EnableSensitiveDataLogging();

                //var interceptor = provider.GetRequiredService<StrategySchemaInterceptor>();
                // options.AddInterceptors(interceptor);
            });*/


            // Estrategia 3 - Banco de dados
            services.AddHttpContextAccessor();
            
            services.AddScoped<ApplicationContext>(provider => 
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

                var httpContext = provider.GetService<IHttpContextAccessor>()?.HttpContext;
                var tenantId = httpContext?.GetTenantId();

                //var connectionString = Configuration.GetConnectionString(tenantId);
                var connectionString = Configuration.GetConnectionString("custom").Replace("_DATABASE_", tenantId);

                optionsBuilder
                    .UseSqlServer(connectionString)
                    .LogTo(Console.WriteLine)
                    .EnableSensitiveDataLogging();

                return new ApplicationContext(optionsBuilder.Options);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EFCore.Multitenant v1"));
            }

           //DatabaseInitialize(app);
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            //app.UseMiddleware<TenantMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /*private void DatabaseInitialize(IApplicationBuilder app)
        {
            using var db = app.ApplicationServices
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ApplicationContext>();
            
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            for(var i=1;i<=5;i++)
            {
                db.People.Add(new Person{ Name = $"Person {i}"});
                db.Products.Add(new Product { Description = $"Product {i}"});
            }            

            db.SaveChanges();
        }*/
    }
}
