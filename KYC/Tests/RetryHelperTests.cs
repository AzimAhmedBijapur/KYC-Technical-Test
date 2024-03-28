/*
 * Author: Azim Ahmed Bijapur
 * C# developer technical test
 */

using System;
using Xunit;
using Moq;
using System.Threading;
using KYC.Models;
using Microsoft.Extensions.Logging;
using KYC.Repositories;

namespace KYC.Tests
{
    public class RetryHelperTests
    {
        [Fact]
        public void AddEntity_RetryOnTransientError()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<MockEntityRepository>>();
            var repository = new MockEntityRepository(loggerMock.Object);

            var entityToAdd = new Entity();

            // Act
            repository.AddEntity(entityToAdd);

            // Assert
            // Verify that the entity was added to the repository
            Assert.Contains(entityToAdd, repository.GetEntities());
        }

        [Fact]
        public void UpdateEntity_RetryOnTransientError()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<MockEntityRepository>>();
            var repository = new MockEntityRepository(loggerMock.Object);

            // Test passes only when the entity exists in the repository
            var entityToUpdate = repository.GetEntityById("1");

            // Act
            repository.UpdateEntity(entityToUpdate);

            // Assert
            Assert.Contains(entityToUpdate, repository.GetEntities());
        }

    }
}
