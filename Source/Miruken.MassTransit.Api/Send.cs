namespace Miruken.MassTransit.Api
{
    public class Send
    {
        public Send()
        {
        }

        public Send(object payload)
        {
            Payload = payload;
        }

        public object Payload { get; set; }
    }
}