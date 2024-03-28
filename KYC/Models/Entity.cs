/*
 * Author: Azim Ahmed Bijapur
 * C# developer technical test
 */

using KYC.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KYC.Models
{

    public class Entity : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Example of generating a unique ID

        // Other properties remain unchanged
        public List<Address>? Addresses { get; set; }
        public List<Date> Dates { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }
        public List<Name> Names { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class Address
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class Date
    {
        public string? DateType { get; set; }
        public DateTime? DateValue { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class Name
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Surname { get; set; }
    }
}
