using System;

namespace IntegrationTests.Domain
{
    public class TestException : Exception
    {
        public TestException()
        {
        }

        public TestException(string message)
            :base (message)
        {
        }
    }
}