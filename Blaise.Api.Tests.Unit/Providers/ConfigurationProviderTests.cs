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
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("http://*:80/"));
        }

        [Test]
        public void Given_TempPath_Value_Is_Set_When_I_Call_TempPath_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.TempPath;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.StartWith(@"c:\Blaise\Temp"));
        }

        [Test]
        public void Given_I_Call_TempPath_I_Get_A_Unique_Path_Back_Each_Time()
        {
            // act
            var result1 = _sut.TempPath;
            var result2 = _sut.TempPath;

            // assert
            Assert.That(result1, Is.Not.EqualTo(result2));
        }

        [Test]
        public void Given_DqsBucket_Value_Is_Set_When_I_Call_DqsBucket_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.DqsBucket;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("dqs-bucket"));
        }

        [Test]
        public void Given_NisraBucket_Value_Is_Set_When_I_Call_NisraBucket_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.NisraBucket;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("nisra-bucket"));
        }

        [Test]
        public void Given_IngestBucket_Value_Is_Set_When_I_Call_NisraBucket_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.IngestBucket;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("ingest-bucket"));
        }

        [Test]
        public void Given_PackageExtension_Value_Is_Set_When_I_Call_PackageExtension_I_Get_The_Correct_Value_Back()
        {
            // act
            var result = _sut.PackageExtension;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("bpkg"));
        }
    }
}
