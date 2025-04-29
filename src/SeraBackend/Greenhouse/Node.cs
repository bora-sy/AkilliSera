using SeraBackend.Greenhouse.Models;

namespace SeraBackend.Greenhouse
{
    public class Node
    {
        public int ID { get; init; }
        
        public DateTime LastSeen { get; private set; } = DateTime.MinValue;

        public bool Connected => DateTime.Now - LastSeen < TimeSpan.FromMilliseconds(GreenhouseConstants.NODE_TIMEOUT_MS);

        public HumidityValues[] HumidityValues => humidityValues.ToArray();
        
        private List<HumidityValues> humidityValues = new();


        public Node(int ID)
        {
            this.ID = ID;
        }


        public void HandleNodeData(int[] humidVals)
        {
            LastSeen = DateTime.Now;

            
            lock (humidityValues)
            {
                humidityValues.Add(new(humidVals, DateTime.Now));

                if (humidityValues.Count >= GreenhouseConstants.HUMIDITY_VAL_HISTORY) humidityValues.RemoveAt(0);
            }
        }
    }
}
