namespace Blaise.Api.Contracts.Models.Cati
{
    using System;
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Questionnaire;

    public class CatiQuestionnaireDto : QuestionnaireDto
    {
    public List<DateTime> SurveyDays { get; set; } = new List<DateTime>();

    public bool Active { get; set; }

    public bool ActiveToday { get; set; }

    public bool DeliverData { get; set; }
    }
}
