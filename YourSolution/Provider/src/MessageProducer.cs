using System;
using shared;

namespace provider
{
    public class MessageProducer
    {
        private readonly Data _data;

        public MessageProducer()
        {
            _data = new Data();
        }
        public TestMessage ProduceMessage()
        {
            if (_data.DataIsMissing())
            {
                return null;
            }

            return new TestMessage()
            {
                Name = "Producer",
                SequenceNumber = (int)DateTime.Now.Ticks,
                Timestamp = DateTime.Now
            };
        }
    }
}
