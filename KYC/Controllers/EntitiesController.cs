/*
 * Author: Azim Ahmed Bijapur
 * C# developer technical test
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KYC.Models;
using KYC.Extensions;
using KYC.Repositories;

namespace KYC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntitiesController : ControllerBase
    {
        private readonly EntityContext _context;
        private readonly IEntityRepository _entityRepository;

        public EntitiesController(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        // GET: api/Entities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entity>>> GetEntities([FromQuery] string? search = null, [FromQuery] string? gender = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] string[] countries = null, [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool ascending = true)
        {
            var entities = _entityRepository.GetEntities();

            // Search query based on Name and Address
            if (!string.IsNullOrWhiteSpace(search))
            {
                // Split the search query into individual terms and remove empty strings
                var searchNames = search.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                entities = entities.Where(entity =>
                    (entity.Addresses != null && entity.Addresses.Any(address =>
                        address.AddressLine != null && address.City != null && address.Country != null &&
                        (address.AddressLine + " " + address.City + " " + address.Country)
                            .Contains(search, StringComparison.OrdinalIgnoreCase))) ||
                    (entity.Names != null && entity.Names.Any(name =>
                        searchNames.Any(searchName =>
                            name.FirstName != null && name.FirstName.Contains(searchName, StringComparison.OrdinalIgnoreCase) ||
                            name.MiddleName != null && name.MiddleName.Contains(searchName, StringComparison.OrdinalIgnoreCase) ||
                            name.Surname != null && name.Surname.Contains(searchName, StringComparison.OrdinalIgnoreCase))))
                );
            }


            // Gender filter
            if (!string.IsNullOrWhiteSpace(gender))
            {
                entities = entities.Where(entity =>
                    entity.Gender != null && entity.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Date filters 

            if (startDate != null && endDate != null)
            {
                // Filter entities based on start and/or end date
                entities = entities.Where(entity =>
                    entity.Dates != null &&
                    entity.Dates.Any(date =>
                        date.DateValue.HasValue &&  
                        ((startDate == null || date.DateValue.Value.Date >= startDate.Value.Date) && 
                         (endDate == null || date.DateValue.Value.Date <= endDate.Value.Date))      
                    )
                );
            }

            // Filter entities based on start date only
            if (startDate != null)
            {
                entities = entities.Where(entity =>
                    entity.Dates != null &&
                    entity.Dates.Any(date =>
                        date.DateType == "startDate" && date.DateValue.Value.Date >= startDate.Value.Date
                    )
                );
            }

            // Filter entities based on end date only
            if ( endDate != null)
            {
                entities = entities.Where(entity =>
                    entity.Dates != null &&
                    entity.Dates.Any(date =>
                        date.DateType == "endDate" && date.DateValue.Value.Date <= endDate.Value.Date
                    )
                );
            }


            // Filter based on country 

            if (countries != null && countries.Length > 0)
            {
                entities = entities.Where(entity =>
                    entity.Addresses != null && entity.Addresses.Any(address =>
                        address.Country != null && countries.Contains(address.Country, StringComparer.OrdinalIgnoreCase)
                    )
                );
            }

            // Sorting by ID
            entities = ascending ? entities.OrderByProperty("Id") : entities.OrderByDescendingProperty("Id");
            
            // Pagination
            entities = entities.Skip((page - 1) * pageSize).Take(pageSize);

            // Return the paginated and sorted entities
            return Ok(entities);

        }

        // GET: api/Entities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entity>> GetEntity(string id)
        {
            var entity = _entityRepository.GetEntityById(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        // PUT: api/Entities/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntity(string id, Entity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            _entityRepository.UpdateEntity(entity);
            return NoContent();
        }

        // POST: api/Entities
        [HttpPost]
        public async Task<ActionResult<Entity>> PostEntity(Entity entity)
        {
            _entityRepository.AddEntity(entity);
            return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity);
        }

        // DELETE: api/Entities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntity(string id)
        {
            var entity = _entityRepository.GetEntityById(id);
            if (entity == null)
            {
                return NotFound();
            }

            _entityRepository.DeleteEntity(entity);
            return NoContent();
        }

        private bool EntityExists(string id)
        {
            return _entityRepository.GetEntities().Any(e => e.Id == id);
        }
    }
}
