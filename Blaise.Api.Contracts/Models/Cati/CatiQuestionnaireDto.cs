using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Questionnaire;

namespace Blaise.Api.Contracts.Models.Cati
{
    public class CatiQuestionnaireDto : QuestionnaireDto
    {
        public List<DateTime> SurveyDays = new List<DateTime>();

        public bool Active;

        public bool ActiveToday;

        public bool DeliverData;
    }
}