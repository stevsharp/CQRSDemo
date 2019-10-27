using System;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Newtonsoft.Json;

namespace SenderApplication
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
            services.ConfigureCors();

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidatorActionFilter));
                options.EnableEndpointRouting = false;
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddScoped<SendMessageConsumer>();

            services.AddMassTransit(c =>
            {
                c.AddConsumer<SendMessageConsumer>();
            });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host("localhost", "/", h => { });

                cfg.ReceiveEndpoint(host, "web-service-endpoint", e =>
                {
                    e.PrefetchCount = 16;
                    //e.UseMessageRetry(x => x.Interval(2, 100));
                    e.LoadFrom(provider);
                    EndpointConvention.Map<SendMessageConsumer>(e.InputAddress);
                });
            }));

            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
            //if do this we can use IRequestClient in controller at this time being I only use IBus
            //services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<SendMessageConsumer>());
            services.AddSingleton<IHostedService, BusService>();

            services.AddDbContext<DataContext>(options =>
            {
                options.UseInMemoryDatabase("inmem");
            });

            services.AddMediatR(typeof(Startup).Assembly);

            services.AddSwaggerGen(x => {
                x.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Yield Api", Description = "Yield Api" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            //app.UseMvc(builder =>
            //{
            //    builder.Select().Expand().Filter().OrderBy().Count().MaxTop(100);
            //    builder.MapVersionedODataRoutes("odata", "odata", modelBuilder.GetEdmModels());
            //});
        }

        private IBusControl Register()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc => { sbc.Host(new Uri("rabbitmq://localhost"), h => { }); });
            return bus;
        }
    }

    public class ModelBuilder
    {
        public static IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);

            return builder.GetEdmModel();
        }
    }
}
