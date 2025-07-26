// TransactionManagementService.Tests/UnitTest1.cs
using System;
using Xunit;

namespace TransactionManagementService.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.True(true);
        }

        [Fact]
        public void Test_Simple_Math()
        {
            // Arrange
            int a = 2;
            int b = 3;
            
            // Act
            int result = a + b;
            
            // Assert
            Assert.Equal(5, result);
        }
    }
}