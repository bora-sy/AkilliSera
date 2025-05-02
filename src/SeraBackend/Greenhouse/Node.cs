using SeraBackend.Controllers.DTO;

namespace SeraBackend.Greenhouse
{
    public class Node
    {
        public int ID { get; init; }
        
        public DateTime LastSeen { get; private set; } = DateTime.MinValue;

        public bool Connected => DateTime.Now - LastSeen < TimeSpan.FromMilliseconds(GreenhouseConstants.NODE_TIMEOUT_MS);

        public NodeMoisture[] MoistureValues => moistureValues.ToArray();
        
        private List<NodeMoisture> moistureValues = new();


        public Node(int ID)
        {
            this.ID = ID;
        }


        public void HandleNodeData(int humidVal)
        {
            LastSeen = DateTime.Now;

            
            lock (moistureValues)
            {
                moistureValues.Add(new(humidVal, DateTime.Now));

                if (moistureValues.Count >= GreenhouseConstants.HUMIDITY_VAL_HISTORY) moistureValues.RemoveAt(0);
            }
        }
    }
}
