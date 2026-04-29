namespace FixItNow.Models.ViewModel
{
    public class ServicesDetails
    {
        public Service service { get; set; }
        public List<Service> RelatedServices { get; set; }
        public List<Review> Reviews { get; set; }
        public int totalReviews {  get; set; }
        public int oneStar {  get; set; }
        public int twoStar {  get; set; }
        public int threeStar { get; set; }  
        public int fourStar { get; set; }
        public int fiveStar { get; set; }

    }
}
