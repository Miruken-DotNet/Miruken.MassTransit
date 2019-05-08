using System;

namespace Miruken.MassTransit.Api
{
    public class Response
    {
        public Response()
        {
        }

        public Response(object payload)
        {
            Payload = payload;
        }

        public Response(Exception exception)
        {
            Exception = exception;
        }

        public object Payload { get; set; }

        public Exception Exception { get; set; }
    }
}