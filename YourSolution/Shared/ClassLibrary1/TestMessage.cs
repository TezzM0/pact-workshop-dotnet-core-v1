using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace shared
{
    public class TestMessage : ICommand
    {
        public string Name { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
