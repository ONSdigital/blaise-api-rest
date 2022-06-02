
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Behaviour.Stubs.Responses
{
    public static class QuestionnaireResponses
    {
        public static List<ISurvey2> ActiveQuestionnaires => new List<ISurvey2>
        {
            new Survey
            {
                Name = BlaiseConfigurationHelper.QuestionnaireName,
                ServerPark = BlaiseConfigurationHelper.ServerParkName,
                Status = "Active",
                Configuration = new MachineConfigurationCollection
                {
                    Configurations = new List<IConfiguration>()
                }
            }
        };
    }

    public class Survey :ISurvey2
    {
        public void RefreshConfiguration()
        {
        }

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public string Name { get; set; }
        public Guid InstrumentID { get; set; }
        public string ServerPark { get; set; }
        public string Status { get; set; }
        public DateTime InstallDate { get; set; }
        public IMachineConfigurationCollection Configuration { get; set; }
        public ISurveyReportingInfo GetReportingInfo()
        {
            return new SurveyReportingInfo
            {
                DataRecordCount = 0
            };
        }
    }

    public class MachineConfigurationCollection : IMachineConfigurationCollection
    {
        public IEnumerator<KeyValuePair<string, IConfiguration>> GetEnumerator()
        {
            return new ConfigurationEnumerator
            {
                Current = new KeyValuePair<string, IConfiguration>("", new Configuration
                {
                    InstrumentName = BlaiseConfigurationHelper.QuestionnaireName,
                    ServerParkName = BlaiseConfigurationHelper.ServerParkName,
                    Status = "Active"
                })
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(string machine)
        {
            return true;
        }

        public int Count { get; set; }

        public IConfiguration this[string machine] => Configurations.FirstOrDefault();

        public IEnumerable<string> Machines { get; set; }
        public IEnumerable<IConfiguration> Configurations { get; set; }
    }

    public class Configuration : IConfiguration
    {
        public int Version { get; set; }
        public string InstrumentName { get; set; }
        public Guid InstrumentId { get; set; }
        public string MetaFileName { get; set; }
        public string ResourceFileName { get; set; }
        public string DataFileName { get; set; }
        public string InitialDataEntrySettingsName { get; set; }
        public string InitialLayoutSetGroupName { get; set; }
        public string InitialLayoutSetName { get; set; }
        public string SurveyRoot { get; set; }
        public string StartPageFileName { get; set; }
        public string SilverlightApplicationFileName { get; set; }
        public string Status { get; set; }
        public DateTime InstallDate { get; set; }
        public string ServerParkName { get; set; }
        public string PackageFileName { get; set; }
        public string DataChecksum { get; set; }
    }

    public class SurveyReportingInfo : ISurveyReportingInfo
    {
        public DateTime ReportDate { get; set; }
        public int DataRecordCount { get; set; }
        public IDictionary<string, int> DataStatusCount { get; set; }
        public int SessionRecordCount { get; set; }
        public IDictionary<string, int> SessionStatusCount { get; set; }
        public int AuditTrailSessionCount { get; set; }
        public IDictionary<string, int> AuditTrailDeviceCount { get; set; }
    }

    public class ConfigurationEnumerator : IEnumerator<KeyValuePair<string, IConfiguration>>
    {
        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public KeyValuePair<string, IConfiguration> Current { get; set; }

        object IEnumerator.Current => Current;
    }
}
