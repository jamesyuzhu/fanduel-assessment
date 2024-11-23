using Microsoft.Build.Framework;

namespace DepthChart.Api.Dtos.Responses
{
    public class PositionDepthResponse
    {        
        public string PositionCode { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Depth { get; set; }
    }
}
