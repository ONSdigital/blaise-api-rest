using System;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Api.Storage.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class InstrumentDataServiceTests
    {
        private InstrumentDataService _sut;

        private Mock<IBlaiseFileService> _fileServiceMock;
        private Mock<ICloudStorageService> _storageServiceMock;
        private MockSequence _mockSequence;

        private string _serverParkName;
        private string _bucketPath;
        private string _instrumentFile;
        private string _instrumentName;

        private DeliverInstrumentDto _deliverInstrumentDto;

        [SetUp]
        public void SetUpTests()
        {
            _fileServiceMock = new Mock<IBlaiseFileService>(MockBehavior.Strict);
            _storageServiceMock = new Mock<ICloudStorageService>(MockBehavior.Strict);
            _mockSequence = new MockSequence();

            _bucketPath = "OPN";
            _instrumentFile = "OPN2010A.zip";
            _serverParkName = "ServerParkA";
            _instrumentName = "OPN2010A";

            _deliverInstrumentDto = new DeliverInstrumentDto
            {
                BucketPath = _bucketPath
            };

            _sut = new InstrumentDataService(
                _fileServiceMock.Object,
                _storageServiceMock.Object);
        }

        [Test]
        public async Task Given_I_Call_DeliverInstrumentPackageWithDataAsync_Then_The_Correct_Services_Are_Called_In_The_Correct_Order()
        {
            //arrange
            const string deliveryFile = @"dd_OPN2004A_08042020_154000.zip";
            const string instrumentFilePath = @"d:\temp\dd_OPN2004A_08042020_154000.zip";
            var expectedUploadBucketPath = $@"{_bucketPath}/data/OPN2010A";

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GetInstrumentPackageName(It.IsAny<string>()))
                .Returns(_instrumentFile);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GenerateUniqueInstrumentFile(It.IsAny<string>()))
                .Returns(deliveryFile);

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadFromBucketAsync(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(instrumentFilePath);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .UpdateInstrumentFileWithData(It.IsAny<string>(), It.IsAny<string>()));

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.UploadToBucketAsync(It.IsAny<string>(),
                It.IsAny<string>())).Returns(Task.FromResult(0));

            _fileServiceMock.InSequence(_mockSequence).Setup(s => s.DeleteFile(It.IsAny<string>()));

            //act
            await _sut.DeliverInstrumentPackageWithDataAsync(_serverParkName, _instrumentName, _deliverInstrumentDto);

            //assert
            _fileServiceMock.Verify(v => v.GenerateUniqueInstrumentFile(_instrumentFile), Times.Once);

            _storageServiceMock.Verify(v => v.DownloadFromBucketAsync(_instrumentFile, 
                deliveryFile), Times.Once);

            _fileServiceMock.Verify(v => v.UpdateInstrumentFileWithData(_serverParkName,
                instrumentFilePath), Times.Once);

            _storageServiceMock.Verify(v => v.UploadToBucketAsync(expectedUploadBucketPath, instrumentFilePath), Times.Once);

            _fileServiceMock.Verify(v => v.DeleteFile(instrumentFilePath), Times.Once);
        }


        [Test]
        public async Task Given_I_Call_DeliverInstrumentPackageWithDataAsync_Then_The_Correct_Uploaded_Bucket_Path_Is_Returned()
        {
            //arrange
            const string deliveryFile = @"dd_OPN2010A_08042020_154000.zip";
            const string instrumentFilePath = @"d:\temp\dd_OPN2010A_08042020_154000.zip";
            var expectedUploadBucketPath = $@"{_bucketPath}/data/OPN2010A";

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GetInstrumentPackageName(It.IsAny<string>()))
                .Returns(_instrumentFile);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GenerateUniqueInstrumentFile(It.IsAny<string>()))
                .Returns(deliveryFile);

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadFromBucketAsync(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(instrumentFilePath);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .UpdateInstrumentFileWithData(It.IsAny<string>(), It.IsAny<string>()));

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.UploadToBucketAsync(It.IsAny<string>(),
                It.IsAny<string>())).Returns(Task.FromResult(0));

            _fileServiceMock.InSequence(_mockSequence).Setup(s => s.DeleteFile(It.IsAny<string>()));

            //act
            var result = await _sut.DeliverInstrumentPackageWithDataAsync(_serverParkName, _instrumentName, _deliverInstrumentDto);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUploadBucketPath, result);
        }

        [Test]
        public void Given_An_Empty_InstrumentName_When_I_Call_DeliverInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.DeliverInstrumentPackageWithDataAsync(_serverParkName, 
                string.Empty, _deliverInstrumentDto));
            Assert.AreEqual("A value for the argument 'instrumentName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentName_When_I_Call_DeliverInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.DeliverInstrumentPackageWithDataAsync(_serverParkName,
               null, _deliverInstrumentDto));
            Assert.AreEqual("instrumentName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_DeliverInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.DeliverInstrumentPackageWithDataAsync(string.Empty, 
                _instrumentName, _deliverInstrumentDto));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_DeliverInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.DeliverInstrumentPackageWithDataAsync(null,
                _instrumentName, _deliverInstrumentDto));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_BucketPath_When_I_Call_DeliverInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            _deliverInstrumentDto.BucketPath = string.Empty;

            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.DeliverInstrumentPackageWithDataAsync(_serverParkName, 
                _instrumentName, _deliverInstrumentDto));
            Assert.AreEqual("A value for the argument 'deliverInstrumentDto.BucketPath' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_BucketPath_When_I_Call_DeliverInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            _deliverInstrumentDto.BucketPath = null;

            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.DeliverInstrumentPackageWithDataAsync(_serverParkName,
                _instrumentName, _deliverInstrumentDto));
            Assert.AreEqual("deliverInstrumentDto.BucketPath", exception.ParamName);
        }

        [Test]
        public async Task Given_I_Call_DownloadInstrumentPackageWithDataAsync_Then_The_Correct_Services_Are_Called_In_The_Correct_Order()
        {
            //arrange
            const string deliveryFile = @"dd_OPN2004A_08042020_154000.zip";
            const string instrumentFilePath = @"d:\temp\dd_OPN2004A_08042020_154000.zip";

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GetInstrumentPackageName(It.IsAny<string>()))
                .Returns(_instrumentFile);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GenerateUniqueInstrumentFile(It.IsAny<string>()))
                .Returns(deliveryFile);

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadFromBucketAsync(It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(instrumentFilePath);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .UpdateInstrumentFileWithData(It.IsAny<string>(), It.IsAny<string>()));

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.UploadToBucketAsync(It.IsAny<string>(),
                It.IsAny<string>())).Returns(Task.FromResult(0));

            _fileServiceMock.InSequence(_mockSequence).Setup(s => s.DeleteFile(It.IsAny<string>()));

            //act
            await _sut.DownloadInstrumentPackageWithDataAsync(_serverParkName, _instrumentName);

            //assert
            _fileServiceMock.Verify(v => v.GetInstrumentPackageName(_instrumentName), Times.Once);
            _fileServiceMock.Verify(v => v.GenerateUniqueInstrumentFile(_instrumentFile), Times.Once);

            _storageServiceMock.Verify(v => v.DownloadFromBucketAsync(_instrumentFile, 
                deliveryFile), Times.Once);

            _fileServiceMock.Verify(v => v.UpdateInstrumentFileWithData(_serverParkName,
                instrumentFilePath), Times.Once);
        }


        [Test]
        public void Given_An_Empty_InstrumentName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.DownloadInstrumentPackageWithDataAsync(_serverParkName, 
                string.Empty));
            Assert.AreEqual("A value for the argument 'instrumentName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.DownloadInstrumentPackageWithDataAsync(_serverParkName,
               null));
            Assert.AreEqual("instrumentName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.DownloadInstrumentPackageWithDataAsync(string.Empty, 
                _instrumentName));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.DownloadInstrumentPackageWithDataAsync(null,
                _instrumentName));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }
    }
}
