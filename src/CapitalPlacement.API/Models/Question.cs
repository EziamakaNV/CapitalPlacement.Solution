namespace CapitalPlacement.API.Models
{
    public class Question
    {
        public string id { get; private set; }
        public string questionText { get; private set; }
        public QuestionType type { get; private set; }
        public List<string> choices { get; private set; }
        public bool enableOther { get; private set; }
        public int maxChoices { get; private set; }

        public Question(string questionTextx, QuestionType typex, List<string> choicesx = null, bool enableOtherx = false, int maxChoicesx = 0)
        {
            id = Guid.NewGuid().ToString();
            questionText = questionTextx;
            type = typex;
            choices = choicesx ?? new List<string>();
            enableOther = enableOtherx;
            maxChoices = maxChoicesx;
        }
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
