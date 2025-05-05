using Microsoft.Extensions.Options;
using SeraBackend.Configurations;
using System.Net;

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


        public void OnNodeData(int nodeID, int humidVal, IPAddress? ip)
        {
            nodes[nodeID].HandleNodeData(humidVal, ip);
        }

        public async Task<bool> SetNodeSolenoidState(int nodeID, bool solenoidState)
        {
            return await nodes[nodeID].SetSolenoidStateAsync(solenoidState);
        }

        public Node[] GetNodes()
        {
            return nodes.ToArray();
        }
    }
}
