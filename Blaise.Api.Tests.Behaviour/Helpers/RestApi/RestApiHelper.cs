namespace Blaise.Api.Tests.Behaviour.Helpers.RestApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Blaise.Api.Contracts.Models.Ingest;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
    using Blaise.Api.Tests.Behaviour.Models.Questionnaire;
    using Blaise.Nuget.Api.Contracts.Enums;
    using Newtonsoft.Json;

    public class RestApiHelper
    {
        private static HttpClient _httpClient;
        private static RestApiHelper _currentInstance;

        public RestApiHelper()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(RestApiConfigurationHelper.BaseUrl) };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static RestApiHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new RestApiHelper());
        }

        public async Task<List<QuestionnaireModel>> GetAllActiveQuestionnaires()
        {
            var questionnaires = await GetListOfObjectsASync<QuestionnaireModel>(RestApiConfigurationHelper.QuestionnairesUrl);
            Console.WriteLine($"GetAllActiveQuestionnaires: All questionnaires found {JsonConvert.SerializeObject(questionnaires)}");
            var activeQuestionnaires = questionnaires != null ? questionnaires.Where(q => q.Status == QuestionnaireStatusType.Active).ToList()
                : new List<QuestionnaireModel>();

            Console.WriteLine($"GetAllActiveQuestionnaires: Active questionnaires returned {JsonConvert.SerializeObject(activeQuestionnaires)}");

            return activeQuestionnaires;
        }

        public async Task<HttpStatusCode> DeployQuestionnaire(string url, string questionnaireFile)
        {
            var model = new QuestionnairePackageDto
            {
                QuestionnaireFile = questionnaireFile,
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, stringContent);

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> ImportOnlineCases(string url, string questionnaireDataPath)
        {
            var model = new QuestionnaireDataDto
            {
                QuestionnaireDataPath = questionnaireDataPath,
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, stringContent);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> IngestData(string url, string ingestFilePath)
        {
            var model = new IngestDataDto(ingestFilePath);

            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, stringContent);
            return response.StatusCode;
        }

        private static async Task<List<T>> GetListOfObjectsASync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default;
            }

            var responseAsJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<T>>(responseAsJson);
        }
    }
}
