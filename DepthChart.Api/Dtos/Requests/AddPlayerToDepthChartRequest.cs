using System;
using System.ComponentModel.DataAnnotations;

namespace DepthChart.Api.Dtos.Requests
{
    public class AddPlayerToDepthChartRequest
    {
        [Required]
        public string PositionCode { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public string PlayerName { get; set; }                
        public int? Depth { get; set; }         
    }
}
