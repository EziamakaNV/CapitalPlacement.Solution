﻿namespace CapitalPlacement.API.Models
{
    public class EmployerProgram
    {
        public string id { get; set; }
        public string employerid { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool phoneInternal { get; set; }
        public bool phoneHide { get; set; }
        public bool nationalityInternal { get; set; }
        public bool nationalityHide { get; set; }
        public bool currentResidenceInternal { get; set; }
        public bool currentResidenceHide { get; set; }
        public bool idNumberInternal { get; set; }
        public bool idNumberHide { get; set; }
        public bool dateOfBirthInternal { get; set; }
        public bool dateOfBirthHide { get; set; }
        public bool genderInternal { get; set; }
        public bool genderHide { get; set; }
        public List<Question> questions { get; set; } = new List<Question>();
    }
}
