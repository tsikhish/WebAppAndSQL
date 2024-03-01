using data;
using domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqlAndWebApi.Controllers
{
    [Route("api/[controller]")]
    public class persons : Controller
    {
        private readonly personcontext _context;
        public persons(personcontext context)
        {
            _context=context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            return await _context.Persons.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Person>> Adduser( Person Person)
        {
            var validator = new userValidation();
            var valid =validator.Validate(Person);
            List<string> errorlist = new();
            if (!valid.IsValid)
            {
                foreach(var item in valid.Errors) {
                    errorlist.Add(item.ErrorMessage);
                }
                return BadRequest(errorlist);
            }
            try
            {
                _context.Persons.Add(Person);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                return BadRequest("blabla");
            }
            return CreatedAtAction("GetPersons", new { id=Person.PersonId }, Person);
        }
        [HttpGet]
        public async  Task<ActionResult<IEnumerable<Person>>>  GetUser()
        {
            var persons = await _context.Persons.Include(x => x.PersonAddress).ToListAsync();
            if (!persons.Any())
            {
                return NotFound("there are no person");
            }
            else
            {
                return persons;
            }

        }
        [HttpGet("id")]
        public async Task<ActionResult<IEnumerable<Person>>> GetUserById(int id)
        {
            var persons=await _context.Persons.Include(x=>x.PersonAddress).Where(x=>x.PersonId==id).ToListAsync();
            if (persons.ToList().Count == 0)
            {
                return NotFound("there is no person with this id");
            }
            else
            {
                return persons;
            }
        }
        [HttpGet("filterUser")]
        public async Task<ActionResult<IEnumerable<Person>>> filterUser([FromQuery] string city,double salary)
        {
            var filterUser = await _context.Persons
                .Include(x=>x.PersonAddress)
                .Where(x=>x.Salary>salary && 
                x.PersonAddress.City==city).
                ToListAsync ();
            if(filterUser.ToList().Count == 0)
            {
                return NotFound("there is no person like this");
            }
            return Ok(filterUser.ToList());
        }
        [HttpDelete("/{id}")]
        public async Task<ActionResult<IEnumerable<Person>>> deleteUser(int id)
        {
            var deletedPerson = await _context.Persons.FindAsync(id);
            if (deletedPerson == null)
            {
                return NotFound($"person with this {id}  is not found");
            }
            _context.Persons.Remove(deletedPerson);
            await _context.SaveChangesAsync();
            return Ok($"person with this {id} is found");
        }
        [HttpPut("/{id}")]
        public async Task<ActionResult<IEnumerable<Person>>> putUser(Person Person)
        {
            var validator = new userValidation();
            var valid = validator.Validate(Person);
            List<string> errorlists = new();
            if (!valid.IsValid)
            {
                foreach(var error in valid.Errors)
                {
                    errorlists.Add(error.ErrorMessage);
                }
                return BadRequest(errorlists);
            }
            Person.PersonAddress.Id = Person.PersonId; 
            try
            {
                _context.Update(Person);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) 
            {
                return NotFound($"there is no participant to update with id {Person.PersonId}");
            }
            return Ok(Person);
        }
    }
}
