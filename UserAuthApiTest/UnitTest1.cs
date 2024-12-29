using Application.EncryptingService;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Application.Tests
{
    [TestClass]
    public class SaltAndPepperServiceTests
    {
        [TestMethod]
        public async Task CheckPasswordAsync_ShouldReturnTrue_WhenPasswordMatches()
        {
            // Arrange
            string password = "string";
            string hashedPassword = "j7+4U7eZUJnbwStHcpF4ZtO3ruVDFcOe4RPt5A3hU0k=";
            string salt = "16)(2&($07$)6$87";

            // Configure mock settings
            var mockSettings = new Mock<IOptions<SaltAndPepperSettings>>();
            var saltAndPepperSettings = new SaltAndPepperSettings
            {
                SaltLettersLength = 16,
                PepperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
                PepperLength = 4
            };
            mockSettings.Setup(o => o.Value).Returns(saltAndPepperSettings);

            // Create the service
            var service = new SaltAndPepperService(mockSettings.Object);

            // Act
            bool result = await service.CheckPasswordAsync(hashedPassword, password, salt);

            // Assert
            Assert.IsTrue(result, "The password should match.");
        }

        [TestMethod]
        public async Task CheckPasswordAsync_ShouldReturnFalse_WhenPasswordDoesNotMatch()
        {
            // Arrange
            string password = "stringf";
            string hashedPassword = "j7+4U7eZUJnbwStHcpF4ZtO3ruVDFcOe4RPt5A3hU0k=";
            string salt = "16)(2&($07$)6$87";

            // Configure mock settings
            var mockSettings = new Mock<IOptions<SaltAndPepperSettings>>();
            var saltAndPepperSettings = new SaltAndPepperSettings
            {
                SaltLettersLength = 16,
                PepperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
                PepperLength = 4
            };
            mockSettings.Setup(o => o.Value).Returns(saltAndPepperSettings);

            // Create the service
            var service = new SaltAndPepperService(mockSettings.Object);

            // Act
            bool result = await service.CheckPasswordAsync(hashedPassword, password, salt);

            // Assert
            Assert.IsFalse(result, "The password should not match.");
        }
    }
}
