using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using ArgoJanitor.WebApi.Domain.Events;
using ArgoJanitor.WebApi.EventHandlers;
using ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD;
using ArgoJanitor.WebApi.Infrastructure.Facades.SSM;
using ArgoJanitor.WebApi.Infrastructure.Messaging;
using ArgoJanitor.WebApi.Infrastructure.Middleware;
using ArgoJanitor.WebApi.Infrastructure.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Prometheus;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ArgoJanitor.WebApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var options = Configuration.GetAWSOptions();
            
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSimpleSystemsManagement>();
            
            services.AddTransient<ISSMFacade, SSMFacade>();
            services.AddTransient<JsonSerializer>();

            services.AddHttpClient<IArgoCDAuthentication, ArgoCdAuthentication>(cfg =>
            {
                var baseUrl = Configuration["ARGO_API_BASE_URL"];
                if (baseUrl != null)
                {
                    cfg.BaseAddress = new Uri(baseUrl);
                }
            });
            
            services.AddHttpClient<IArgoCDFacade, ArgoCDFacade>(cfg =>
            {
                var baseUrl = Configuration["ARGO_API_BASE_URL"];
                if (baseUrl != null)
                {
                    cfg.BaseAddress = new Uri(baseUrl);
                }
            })
                .AddHttpMessageHandler<BearerTokenHandler>();
            

            ConfigureDomainEvents(services);

            services.AddHostedService<MetricHostedService>();
            services.AddHostedService<ConsumerHostedService>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());

            services.AddSwaggerDocument();
        }

        private static void ConfigureDomainEvents(IServiceCollection services)
        {
            var eventRegistry = new DomainEventRegistry();
            services.AddSingleton(eventRegistry);

            services.AddTransient<IEventHandler<CapabilityCreatedDomainEvent>, ArgoCDCapabilityCreatedDomainEventHandler>();
   
            var serviceProvider = services.BuildServiceProvider();

            eventRegistry
            .Register<CapabilityCreatedDomainEvent>(
                eventTypeName: "capability_created",
                topicName: "build.capabilities",
                eventHandler: serviceProvider.GetRequiredService<IEventHandler<CapabilityCreatedDomainEvent>>());

            services.AddTransient<IEventDispatcher, EventDispatcher>();

            services.AddTransient<KafkaConsumerFactory.KafkaConfiguration>();
            services.AddTransient<KafkaConsumerFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseMvc();

            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                ResponseWriter = MyPrometheusStuff.WriteResponseAsync
            });
        }
    }

    public class MetricHostedService : IHostedService
    {
        private const string Host = "0.0.0.0";
        private const int Port = 8080;

        private IMetricServer _metricServer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Staring metric server on {Host}:{Port}");

            _metricServer = new KestrelMetricServer(Host, Port).Start();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using (_metricServer)
            {
                Console.WriteLine("Shutting down metric server");
                await _metricServer.StopAsync();
                Console.WriteLine("Done shutting down metric server");
            }
        }
    }

    public static class MyPrometheusStuff
    {
        private const string HealthCheckLabelServiceName = "service";
        private const string HealthCheckLabelStatusName = "status";

        private static readonly Gauge HealthChecksDuration;
        private static readonly Gauge HealthChecksResult;

        static MyPrometheusStuff()
        {
            HealthChecksResult = Metrics.CreateGauge("healthcheck",
                "Shows health check status (status=unhealthy|degraded|healthy) 1 for triggered, otherwise 0", new GaugeConfiguration
                {
                    LabelNames = new[] { HealthCheckLabelServiceName, HealthCheckLabelStatusName },
                    SuppressInitialValue = false
                });

            HealthChecksDuration = Metrics.CreateGauge("healthcheck_duration_seconds",
                "Shows duration of the health check execution in seconds",
                new GaugeConfiguration
                {
                    LabelNames = new[] { HealthCheckLabelServiceName },
                    SuppressInitialValue = false
                });
        }

        public static Task WriteResponseAsync(HttpContext httpContext, HealthReport healthReport)
        {
            UpdateMetrics(healthReport);

            httpContext.Response.ContentType = "text/plain";
            return httpContext.Response.WriteAsync(healthReport.Status.ToString());
        }

        private static void UpdateMetrics(HealthReport report)
        {
            foreach (var (key, value) in report.Entries)
            {
                HealthChecksResult.Labels(key, "healthy").Set(value.Status == HealthStatus.Healthy ? 1 : 0);
                HealthChecksResult.Labels(key, "unhealthy").Set(value.Status == HealthStatus.Unhealthy ? 1 : 0);
                HealthChecksResult.Labels(key, "degraded").Set(value.Status == HealthStatus.Degraded ? 1 : 0);

                HealthChecksDuration.Labels(key).Set(value.Duration.TotalSeconds);
            }
        }
    }
}