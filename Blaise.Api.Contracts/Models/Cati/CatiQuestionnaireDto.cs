using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Questionnaire;

namespace Blaise.Api.Contracts.Models.Cati
{
    public class CatiQuestionnaireDto : QuestionnaireDto
    {
        public CatiQuestionnaireDto()
        {
            SurveyDays = new List<DateTime>();
        }

        public List<DateTime> SurveyDays;

        public bool Active;

        public bool ActiveToday;

        public bool DeliverData;
    }
}