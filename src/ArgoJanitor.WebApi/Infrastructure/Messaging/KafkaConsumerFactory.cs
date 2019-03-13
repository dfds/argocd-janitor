using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ArgoJanitor.WebApi.Infrastructure.Messaging
{
    public class KafkaConsumerFactory
    {
        private readonly KafkaConfiguration _configuration;
        private readonly ILogger<KafkaConsumerFactory> _logger;

        public KafkaConsumerFactory(KafkaConfiguration configuration, ILogger<KafkaConsumerFactory> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Consumer<string, string> Create()
        {
            _logger.LogInformation("Creating KafkaConsumer");
            var config = _configuration.AsEnumerable().ToArray();
            _logger.LogInformation("Configuration loaded");
            
            return new Consumer<string, string>(
                config: config,
                keyDeserializer: new StringDeserializer(Encoding.UTF8),
                valueDeserializer: new StringDeserializer(Encoding.UTF8)
            );
        }

        public class KafkaConfiguration
        {
            private const string KEY_PREFIX = "ARGOJANITOR_KAFKA_";
            private readonly IConfiguration _configuration;

            public KafkaConfiguration(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            private string Key(string keyName) => string.Join("", KEY_PREFIX, keyName.ToUpper().Replace('.', '_'));

            private Tuple<string, string> GetConfiguration(string key)
            {
                var value = _configuration[Key(key)];

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return Tuple.Create<string, string>(key, value);
            }

            public IEnumerable<KeyValuePair<string, object>> AsEnumerable()
            {
                var configurationKeys = new[]
                {
                    "group.id",
                    "enable.auto.commit",
                    "bootstrap.servers",
                    "broker.version.fallback",
                    "api.version.fallback.ms",
                    "ssl.ca.location",
                    "sasl.username",
                    "sasl.password",
                    "sasl.mechanisms",
                    "security.protocol",
                    "auto.offset.reset"
                };

                var config = configurationKeys
                    .Select(key => GetConfiguration(key))
                    .Where(pair => pair != null)
                    .Select(pair => new KeyValuePair<string, object>(pair.Item1, pair.Item2))
                    .ToList();
                
                config.Add(new KeyValuePair<string, object>("request.timeout.ms", "3000"));

                return config;
            }
        }
    }
}