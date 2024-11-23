using System;

namespace DepthChart.Api.Exceptions
{
    public class PlayerNotInPositionException : Exception
    {
        public PlayerNotInPositionException() { }

        public PlayerNotInPositionException(string message)
            : base(message) { }
    }
}
