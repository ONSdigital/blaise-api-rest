namespace Blaise.Api.Tests.Unit.Providers
{
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Providers;
    using NUnit.Framework;

    public class ConfigurationProviderTests
    {
        /// <summary>
        /// Please ensure the App.config in the test project has values that relate to the tests.
        /// </summary>
        private IConfigurationProvider _sut;

        [SetUp]
        public void SetUpTests()
        {
            _sut = new ConfigurationProvider();
        }

        [Test]
        public void Given_BaseUrl_Value_Is_Set_When_I_Call_BaseUrl_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.BaseUrl;

            // assert
            Assert.NotNull(result);
            Assert.AreEqual("http://*:80/", result);
        }

        [Test]
        public void Given_TempPath_Value_Is_Set_When_I_Call_TempPath_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.TempPath;

            // assert
            Assert.NotNull(result);
            Assert.True(result.StartsWith(@"c:\Blaise\Temp"));
        }

        [Test]
        public void Given_I_Call_TempPath_I_Get_A_Unique_Path_Back_Each_Time()
        {
            // act
            var result1 = _sut.TempPath;
            var result2 = _sut.TempPath;

            // assert
            Assert.AreNotEqual(result1, result2);
        }

        [Test]
        public void Given_DqsBucket_Value_Is_Set_When_I_Call_DqsBucket_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.DqsBucket;

            // assert
            Assert.NotNull(result);
            Assert.AreEqual("dqs-bucket", result);
        }

        [Test]
        public void Given_NisraBucket_Value_Is_Set_When_I_Call_NisraBucket_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.NisraBucket;

            // assert
            Assert.NotNull(result);
            Assert.AreEqual("nisra-bucket", result);
        }

        [Test]
        public void Given_IngestBucket_Value_Is_Set_When_I_Call_NisraBucket_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.IngestBucket;

            // assert
            Assert.NotNull(result);
            Assert.AreEqual("ingest-bucket", result);
        }

        [Test]
        public void Given_PackageExtension_Value_Is_Set_When_I_Call_PackageExtension_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.PackageExtension;

            // assert
            Assert.NotNull(result);
            Assert.AreEqual("bpkg", result);
        }
    }
}
