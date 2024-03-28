using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using KYC.Models;

namespace KYC.Repositories
{
    public class MockEntityRepository : IEntityRepository
    {
        private readonly List<Entity> _entities;
        private readonly ILogger<MockEntityRepository> _logger;

        public MockEntityRepository(ILogger<MockEntityRepository> logger)
        {
            _entities = new List<Entity>();
            _logger = logger;
            InitializeEntities();
        }

        // Populating data repository with dummy data
        private void InitializeEntities()
        {
            _entities.AddRange(new List<Entity>
            {
                new Entity
                {
                    Id = "1",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "123 Main St", City = "City1", Country = "Country1" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "startDate", DateValue = DateTime.Now },
                        new Date { DateType = "endDate", DateValue = DateTime.Now.AddDays(1) } // Adding another date
                    },
                    Deceased = false,
                    Gender = "Male",
                    Names = new List<Name>
                    {
                        new Name { FirstName = "John", MiddleName = "James", Surname = "Doe" }
                    }
                },
                new Entity
                {
                    Id = "2",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "456 Elm St", City = "City2", Country = "Country2" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "startDate", DateValue = DateTime.Now.AddDays(1) },
                        new Date { DateType = "endDate", DateValue = DateTime.Now.AddDays(2) } // Adding another date
                    },
                    Deceased = true,
                    Gender = "Female",
                    Names = new List<Name>
                    {
                        new Name { FirstName = "Jane", MiddleName="Jacob",Surname = "Smith" }
                    }
                },
                new Entity
                {
                    Id = "3",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "789 Oak St", City = "City3", Country = "Country3" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "startDate", DateValue = DateTime.Now.AddDays(3) },
                        new Date { DateType = "endDate", DateValue = DateTime.Now.AddDays(4) } // Adding another date
                    },
                    Deceased = false,
                    Gender = "Male",
                    Names = new List<Name>
                    {
                        new Name { FirstName = "Michael", MiddleName="David", Surname = "Johnson" }
                    }
                },
                new Entity
                {
                    Id = "4",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "1011 Pine St", City = "City4", Country = "Country4" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "startDate", DateValue = DateTime.Now.AddDays(5) },
                        new Date { DateType = "endDate", DateValue = DateTime.Now.AddDays(6) } // Adding another date
                    },
                    Deceased = true,
                    Gender = "Female",
                    Names = new List<Name>
                    {
                        new Name { FirstName = "Emily", MiddleName="Grace", Surname = "Williams" }
                    }
                },
                new Entity
                {
                    Id = "5",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "1213 Maple St", City = "City5", Country = "Country5" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "startDate", DateValue = DateTime.Now.AddDays(7) },
                        new Date { DateType = "endDate", DateValue = DateTime.Now.AddDays(8) } // Adding another date
                    },
                    Deceased = false,
                    Gender = "Male",
                    Names = new List<Name>
                    {
                        new Name { FirstName = "Sophia", MiddleName="Rose", Surname = "Brown" }
                    }
                }

            });
        }


        // Get entities
        public IQueryable<Entity> GetEntities()
        {
            return _entities.AsQueryable();
        }


        // Get entity by ID
        public Entity GetEntityById(string id)
        {
            return _entities.FirstOrDefault(e => e.Id == id);
        }

        // Adding entity with retry mechanism
        public void AddEntity(Entity entity)
        {
            Retry(() =>
            {
                entity.Id = Guid.NewGuid().ToString();
                _entities.Add(entity);
            }, 3);
        }

        // Update entity with retry mechanism
        public void UpdateEntity(Entity entity)
        {
            Retry(() =>
            {
                var existingEntity = _entities.FirstOrDefault(e => e.Id == entity.Id);
                if (existingEntity != null)
                {
                    existingEntity.Addresses = entity.Addresses;
                    existingEntity.Dates = entity.Dates;
                    existingEntity.Deceased = entity.Deceased;
                    existingEntity.Gender = entity.Gender;
                    existingEntity.Names = entity.Names;
                }
                else
                {
                    throw new Exception("Entity not found in the repository.");
                }
            });
        }


        // Deleting entity
        public void DeleteEntity(Entity entity)
        {
            _entities.Remove(entity);
        }


        // Retry mechanism for write ops with max 3 attempts and exponential backoff
        private void Retry(Action operation, int maxAttempts = 3, TimeSpan initialDelay = default, TimeSpan maxDelay = default, double multiplier = 2.0)
        {
            int attempts = 0;
            TimeSpan delay = initialDelay == default ? TimeSpan.FromSeconds(1) : initialDelay;

            do
            {
                try
                {
                    operation();
                    return;
                }
                catch (Exception ex) when (IsTransientError(ex) && attempts < maxAttempts)
                {
                    attempts++;
                    LogRetryAttempt(attempts, delay, ex);
                    Thread.Sleep(delay); // Wait before retrying

                    // Exponential backoff: increase delay exponentially
                    delay = TimeSpan.FromTicks(Math.Min((long)(delay.Ticks * multiplier), maxDelay.Ticks));
                }
            } while (attempts < maxAttempts);

            throw new ApplicationException();
        }

        private bool IsTransientError(Exception ex)
        {
            if (ex is TimeoutException || ex is System.Net.Sockets.SocketException)
            {
                return true;
            }

            return false;
        }

        private void LogRetryAttempt(int attemptNumber, TimeSpan delayBetweenAttempts, Exception ex)
        {
            _logger?.LogWarning($"Retry attempt {attemptNumber} after {delayBetweenAttempts.TotalSeconds} seconds delay. Error: {ex.Message}");
        }
    }
}
