namespace CapitalPlacement.API.Models
{
    public class Question
    {
        public string id { get; set; }
        public string questionText { get; set; }
        public QuestionType type { get; set; }
        public List<string> choices { get; set; }
        public bool enableOther { get; set; }
        public int maxChoices { get; set; }
    }

    public enum QuestionType
    {
        Paragraph = 1,
        YesNo = 2,
        Dropdown = 3,
        MultipleChoice = 4,
        Date = 5,
        Number = 6
    }
}
