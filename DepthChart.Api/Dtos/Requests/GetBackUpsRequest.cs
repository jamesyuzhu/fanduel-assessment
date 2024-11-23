using System.ComponentModel.DataAnnotations;

namespace DepthChart.Api.Dtos.Requests
{
    public class GetBackUpsRequest
    {
        [Required]
        public string PositionCode { get; set; }
        [Required]
        public int? PlayerId { get; set; }
    }
}
