namespace SeraBackend.Controllers.DTO
{
    public class GreenhouseData
    {
        public int[] ActiveNodeIDs { get; set; }

        public NodeMoisture[][] NodeMoistures { get; set; }
    }

    public class NodeMoisture
    {
        public int Moisture { get; set; }
        public DateTime Timestamp { get; set; }

        public NodeMoisture(int moisture, DateTime timestamp)
        {
            Timestamp = timestamp;
            Moisture = moisture;
        }
    }
}
