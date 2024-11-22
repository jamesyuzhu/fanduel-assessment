namespace DepthChart.Api.Dtos.Requests
{
    public class RemovePlayerFromDepthChartRequest
    {
        public string PositionCode { get; set; }
        public int PlayerId { get; set; }         
    }
}
