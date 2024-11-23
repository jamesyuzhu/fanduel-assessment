using System;

namespace DepthChart.Api.Dtos.Responses
{
    public class RemovePlayerFromDepthChartResponse
    {         
        public int? PlayerId { get; set; }
        public string PlayerName { get; set; }         
    }
}
