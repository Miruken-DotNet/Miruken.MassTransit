// ReSharper disable RedundantAssignment
namespace IntegrationTests.Aws
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Docker.DotNet.Models;
    using Domain;
    using MassTransit;
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.Configuration;
    using Miruken.MassTransit;
    using Miruken.MassTransit.Api;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Setup;

    public class LocalstackSetup : DockerMassTransitSetup
    {
        private const int Port = 4566;

        private static readonly JsonConverter Json = new MirukenJsonConverter();
        public LocalstackSetup()
            : base("localstack/localstack", "latest", Port)
        {
        }

        public override Uri CreateQueueUri(string queueName)
        {
            var sqsUri = new Uri($"amazonsqs://localhost:{Port}");
            return new Uri(sqsUri, queueName);
        }

        public override Action<IServiceCollectionBusConfigurator> Configure(string queueName) =>
            mt => mt.AddBus(sp => Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                cfg.Host(new Uri($"amazonsqs://localhost:{Port}"), h =>
                {
                    h.AccessKey("foo");
                    h.SecretKey("bar");
                    h.Config(new AmazonSQSConfig {ServiceURL = $"http://localhost:{Port}"});
                    h.Config(new AmazonSimpleNotificationServiceConfig {ServiceURL = $"http://localhost:{Port}"});
                });
                
                cfg.UseInMemoryOutbox();

                cfg.ReceiveEndpoint(queueName, ep =>
                {
                    ep.PrefetchCount            = 1;
                    ep.WaitTimeSeconds          = 0;
                    ep.PurgeOnStartup           = true;
                    ep.ConfigureConsumeTopology = false;
                    
                    ep.Subscribe("miruken-topic", s =>
                    {
                        
                    });

                    ep.Consumer<QueueThisConsumer>(sp);
                    ep.Consumer<SendConsumer>(sp);
                    ep.Consumer<PublishConsumer>(sp);
                    ep.Consumer<RequestConsumer>(sp);
                });

                cfg.ConfigureJsonSerializer(x =>
                {
                    x.Converters.Insert(0, Json);
                    return x;
                });

                cfg.ConfigureJsonDeserializer(x =>
                {
                    x.Converters.Insert(0, Json);
                    return x;
                });
            }));
        
        public override IBusControl CreateClientBus()
        {
            return Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                cfg.Host(new Uri($"amazonsqs://localhost:{Port}"), h =>
                {
                    h.AccessKey("foo");
                    h.SecretKey("bar");
                    h.Config(new AmazonSQSConfig {ServiceURL = $"http://localhost:{Port}"});
                    h.Config(new AmazonSimpleNotificationServiceConfig {ServiceURL = $"http://localhost:{Port}"});
                });
                
                cfg.ConfigureJsonSerializer(x =>
                {
                    x.Converters.Insert(0, Json);
                    return x;
                });

                cfg.ConfigureJsonDeserializer(x =>
                {
                    x.Converters.Insert(0, Json);
                    return x;
                });
            });
        }

        protected override async Task<bool> TestReady(int externalPort)
        {
            var client = new HttpClient();
            try
            {
                var serverUrl = $"http://localhost:{Port}";
                var health    = await client.GetStringAsync($"{serverUrl}/health");
                var status    = JObject.Parse(health);
                var services  = status["services"]?.ToObject<Dictionary<string, string>>();
                return services?.All(service => service.Value == "running") == true;
            }
            catch
            {
                return false;
            }
        }

        protected override CreateContainerParameters ConfigureContainer(
            ConfigurationBuilder configuration,
            ref int              externalPort)
        {
            externalPort = Port;
            return new CreateContainerParameters
            {
                Hostname = HostName,
                HostConfig = new HostConfig
                {
                    Binds = new List<string>
                    {
                        "localstack-data:/tmp/localstack:rw"
                    },
                    Mounts = new []
                    {
                        new Mount
                        {
                            Type = "bind",
                            Source = "/var/run/docker.sock",
                            Target = "/var/run/docker.sock"
                        }
                    },
                    PublishAllPorts = false,
                    VolumesFrom = new List<string>()
                },
                Env = new List<string>(Environment ?? Enumerable.Empty<string>())
                {
                    //"DEBUG=1",
                    //$"EDGE_PORT={Port}",
                    "DEFAULT_REGION=us-west-1",
                    "SERVICES=sns,sqs",
                    "DOCKER_HOST=unix:///var/run/docker.sock",
                },
                Volumes = new Dictionary<string, EmptyStruct>
                {
                    { "/tmp/localstack", default }
                }
            };
        }
    }
}