﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Blaise.Api.Tests.Behaviour.Models.Questionnaire;
using Blaise.Api.Tests.Behaviour.Stubs;
using Blaise.Nuget.Api.Contracts.Enums;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;

namespace Blaise.Api.Tests.Behaviour.Helpers.RestApi
{
    public class RestApiHelper
    {
        private static HttpClient _httpClient;
        private static RestApiHelper _currentInstance;
        private static IDisposable _webApp;

        public RestApiHelper()
        {
            _httpClient = new HttpClient {BaseAddress = new Uri(RestApiConfigurationHelper.BaseUrl)};
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static RestApiHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new RestApiHelper());
        }

        public void StartWebApi()
        {
            _webApp = WebApp.Start<StartupStub>("http://localhost:9443/");
        }

        public void StopWebApi()
        {
            _webApp.Dispose();
        }

        public async Task<List<Questionnaire>> GetAllActiveQuestionnaires()
        {
            var questionnaires = await GetListOfObjectsASync<Questionnaire>(RestApiConfigurationHelper.InstrumentsUrl);
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