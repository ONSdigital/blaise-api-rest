using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Tests.Helpers.Configuration;
using Blaise.Api.Tests.Models.Questionnaire;
using Blaise.Nuget.Api.Contracts.Enums;
using Newtonsoft.Json;

namespace Blaise.Api.Tests.Helpers.RestApi
{
    public class RestApiHelper
    {
        private static HttpClient _httpClient;
        private static RestApiHelper _currentInstance;

        public RestApiHelper()
        {
            _httpClient = new HttpClient {BaseAddress = new Uri(RestApiConfigurationHelper.BaseUrl)};
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static RestApiHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new RestApiHelper());
        }

        public async Task<List<Questionnaire>> GetAllActiveQuestionnaires()
        {
            var questionnaires =
                await GetListOfObjectsASync<Questionnaire>(RestApiConfigurationHelper.InstrumentsUrl);
            return questionnaires != null ? questionnaires.Where(q => q.Status == SurveyStatusType.Active).ToList()
                : new List<Questionnaire>();
        }

        public async Task DeployQuestionnaire(string url, string bucketPath, string instrumentPackage)
        {
            var model = new InstrumentPackageDto
            {
                BucketPath = bucketPath,
                InstrumentFile = instrumentPackage
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(model));
            await _httpClient.PostAsync(url, stringContent);
        }


        public async Task<string> DeliverInstrumentWithData(string url, string bucketPath, string instrumentPackage)
        {
            var model = new InstrumentPackageDto
            {
                BucketPath = bucketPath,
                InstrumentFile = instrumentPackage
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(model));
            var response = await _httpClient.PostAsync(url, stringContent);

            return response.Headers.Location.AbsoluteUri;
        }

        private static async Task<List<T>> GetListOfObjectsASync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK) return default;

            var responseAsJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<T>>(responseAsJson);
        }
    }
}