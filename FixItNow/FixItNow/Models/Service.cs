namespace FixItNow.Models
{
    public class Service
    {
        public int id { get; set; }
        public string title {  get; set; }
        public string description { get; set; } 
        public List<string> features { get; set; }
        public string category {  get; set; }
        public float pricing {  get; set; }
        public string provider {  get; set; }
        public int providerID { get; set; }
        public string referenceImagePath {  get; set; }



    }
}
