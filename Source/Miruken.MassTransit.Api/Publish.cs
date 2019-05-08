namespace Miruken.MassTransit.Api
{
    public class Publish
    {
        public Publish()
        {
        }

        public Publish(object payload)
        {
            Payload = payload;
        }

        public object Payload { get; set; }
    }
}

