using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace VowelConsRater
{
    class VowelConsNum
    {
        public int Consonants {get; set;} = 0;
        public int Vowels {get; set;} = 0;
    }
}
