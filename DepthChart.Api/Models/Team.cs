namespace DepthChart.Api.Models
{
    public class Team
    {
        // Assume the code is unique identifier.
        // use the code string instead of integer
        // as identifier just to make the value in
        // url path more readable
        public string Code { get; set; }
        public string Name { get; set; }
        public string SportCode { get; set; }
    }
}
