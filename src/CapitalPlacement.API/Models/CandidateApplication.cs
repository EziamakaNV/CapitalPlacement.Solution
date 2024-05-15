namespace CapitalPlacement.API.Models
{
    public class CandidateApplication
    {
        public string id { get; set; }
        public string programid { get; set; }
        public string employerid { get; set; }
        public List<Answer> answers { get; set; }
    }
}
