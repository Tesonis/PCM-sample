namespace PCM.Models
{
    public class ComponentRow
    {
        public string ComponentCode { get; set; }
        public string ComponentText { get; set; }
        public string SortClass { get; set; }
        public float PercVal { get; set; }
        public float DollarVal { get; set; }
        public bool TotalIden { get; set; }
        public string Relation { get; set; }
        public string Unit { get; set; }
        public float Ratio { get; set; }
    }
}