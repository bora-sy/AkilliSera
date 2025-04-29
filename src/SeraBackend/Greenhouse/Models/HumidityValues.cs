namespace SeraBackend.Greenhouse.Models
{
    public class HumidityValues
    {
        public int[] values { get; private set; }
        public DateTime timestamp { get; private set; }

        public HumidityValues(int[] values, DateTime dt)
        {
            this.values = values;
            this.timestamp = dt;
        }
    }
}
