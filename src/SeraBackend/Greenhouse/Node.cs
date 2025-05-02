using SeraBackend.Controllers.DTO;
using System.Net;

namespace SeraBackend.Greenhouse
{
    public class Node
    {
        public int ID { get; init; }
        
        public DateTime LastSeen { get; private set; } = DateTime.MinValue;

        public bool Connected => DateTime.Now - LastSeen < TimeSpan.FromMilliseconds(GreenhouseConstants.NODE_TIMEOUT_MS);

        public NodeMoisture[] MoistureValues => moistureValues.ToArray();

        public IPAddress? IP { get; private set; } = null;
        
        private List<NodeMoisture> moistureValues = new();


        public Node(int ID)
        {
            this.ID = ID;
        }

        private string? GetSolenoidEndpoint(bool soleneoidState) => IP == null ? null : $"http://{IP}:{GreenhouseConstants.NODE_PORT}{GreenhouseConstants.NODE_SOLENOID_PATH}?state={(soleneoidState ? "1" : "0")}";
        
        public async Task<bool> SetSolenoidAsync(bool state)
        {
            string? url = GetSolenoidEndpoint(state);
            if (url == null) return false;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var res = await client.PostAsync(url, null);

                    return res.IsSuccessStatusCode;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }
        
        
        public void HandleNodeData(int humidVal, IPAddress? ip)
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
