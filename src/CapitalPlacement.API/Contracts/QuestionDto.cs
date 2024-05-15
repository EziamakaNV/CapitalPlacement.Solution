using CapitalPlacement.API.Models;

namespace CapitalPlacement.API.Contracts
{
    public class QuestionDto
    {
        public string questionText { get; set; }
        public QuestionType type { get; set; }
        public List<string> choices { get; set; }
        public bool enableOther { get; set; }
        public int maxChoices { get; set; }
    }
}
