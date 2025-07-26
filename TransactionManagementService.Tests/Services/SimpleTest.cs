// TransactionManagementService.Tests/SimpleTest.cs
using Xunit;

namespace TransactionManagementService.Tests
{
    public class SimpleTest
    {
        [Fact]
        public void Simple_Test_Should_Pass()
        {
            // Arrange
            var expected = 2;
            
            // Act
            var actual = 1 + 1;
            
            // Assert
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Another_Simple_Test()
        {
            // Arrange & Act
            var result = "Hello World";
            
            // Assert
            Assert.NotNull(result);
            Assert.Contains("World", result);
        }
    }
}