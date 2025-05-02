using Microsoft.Extensions.Options;
using SeraBackend.Configurations;

namespace SeraBackend.Greenhouse
{
    public class GreenhouseService
    {
        GreenhouseConfiguration _config;

        private List<Node> nodes = new();

        public GreenhouseService(IOptions<GreenhouseConfiguration> config)
        {
            _config = config.Value;

            for(int i = 0; i < config.Value.NodeCount; i++)
            {
                nodes.Add(new(i));
            }
        }


        public void OnNodeData(int nodeID, int humidVal)
        {
            nodes[nodeID].HandleNodeData(humidVal);
        }

        public Node[] GetNodes()
        {
            return nodes.ToArray();
        }
    }
}
