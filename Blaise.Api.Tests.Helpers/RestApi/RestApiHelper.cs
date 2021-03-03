using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        public async Task<HttpStatusCode> DeployQuestionnaire(string url, string instrumentFile)
        {
            var model = new InstrumentPackageDto
            {
                InstrumentFile = instrumentFile
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, stringContent);
            return response.StatusCode;
        }

        public async Task<string> GetInstrumentWithData(string url, string path)
        {
            var response = await _httpClient.GetAsync(url);

            var fileName = response.Content.Headers.ContentDisposition.FileName;
            var localFilePath = Path.Combine(path, Guid.NewGuid().ToString());

            if (!Directory.Exists(localFilePath))
            {
                Directory.CreateDirectory(localFilePath);
            }

            var filePath = Path.Combine(localFilePath, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                await response.Content.CopyToAsync(fileStream);
            }

            return filePath;
        }

        public async Task<HttpStatusCode> ImportOnlineCases(string url, string instrumentDataPath)
        {
            var model = new InstrumentDataDto
            {
                InstrumentDataPath = instrumentDataPath
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, stringContent);
            return response.StatusCode;
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