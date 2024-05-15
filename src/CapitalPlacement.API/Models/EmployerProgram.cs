namespace CapitalPlacement.API.Models
{
    public class EmployerProgram
    {
        public string id { get; private set; }
        public string title { get; private set; }
        public string employerid { get; private set; }
        public string description { get; private set; }
        public bool phoneInternal { get; private set; }
        public bool phoneHide { get; private set; }
        public bool nationalityInternal { get; private set; }
        public bool nationalityHide { get; private set; }
        public bool currentResidenceInternal { get; private set; }
        public bool currentResidenceHide { get; private set; }
        public bool idNumberInternal { get; private set; }
        public bool idNumberHide { get; private set; }
        public bool dateOfBirthInternal { get; private set; }
        public bool dateOfBirthHide { get; private set; }
        public bool genderInternal { get; private set; }
        public bool genderHide { get; private set; }
        public List<Question> questions { get; private set; } = new List<Question>();

        public EmployerProgram(string titleText, string descriptionText, string employerIdx)
        {
            id = Guid.NewGuid().ToString();
            title = titleText;
            description = descriptionText;
            employerid = employerIdx;
        }

        public void AddQuestion(string questionText, QuestionType type, List<string> choices = null, bool enableOther = false, int maxChoices = 0)
        {
            var question = new Question(questionText, type, choices, enableOther, maxChoices);
            questions.Add(question);
        }

        public void UpdateDetails(string titleText, string descriptionText)
        {
            title = titleText;
            description = descriptionText;
        }
    }
}
