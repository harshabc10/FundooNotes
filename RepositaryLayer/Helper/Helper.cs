using Confluent.Kafka;
using RepositaryLayer.DTO.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Helper
{
    public class Helper
    {
        public static ProducerConfig GetProducerConfig()
        {
            return new ProducerConfig
            {
                BootstrapServers = "localhost:9092" // Kafka broker(s) address
            };
        }
    }

    
}
