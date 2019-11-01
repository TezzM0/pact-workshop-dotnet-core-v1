﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Antiforgery.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NServiceBus;
using provider;

namespace Provider
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var messageSender = new MessageSender();
            await messageSender.SetupNServiceBusAsync().ConfigureAwait(false);
            messageSender.MessageProducer = new MessageProducer();
            Task.Run(() => { while (true) { messageSender.SendMessages(); } });
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
