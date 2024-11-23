using Microsoft.Build.Framework;

namespace DepthChart.Api.Dtos.Requests
{
    public class RemovePlayerFromDepthChartRequest
    {
        [Required]
        public string PositionCode { get; set; }
        [Required]
        public int? PlayerId { get; set; }         
    }
}
