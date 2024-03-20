using dataOfSql;
using domain;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using sqlandwebapi.services;
using sqlAndWebApi.helper;
using sqlAndWebApi.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace sqlAndWebApi.Controllers
{
    [Route("api/[controller]")]
    public class PersonsController : Controller
    {

        private readonly PersonContext _context;
        private readonly Appsettings _appsettings;
        private readonly ITestService _testservice;
        public PersonsController(PersonContext context, IOptions<Appsettings> appSettings,
             ITestService testservice)
        {
            _context = context;
            _appsettings = appSettings.Value;
            _testservice = testservice;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> RegirsterUser([FromBody] Registration user)
        {
            try
            {
                await RegistrationValidation(user);

                var existingPerson = _testservice.GetPersonByUsername(user.userName);
                if (existingPerson != null)
                {
                    return Conflict($"{existingPerson} already exists");
                }
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
                var newperson = new Person
                {
                    userName = user.userName,
                    password = passwordHash,
                    email = user.email,
                    Role = user.Role
                };
                _testservice.AddUser(newperson);
                _context.SaveChanges();
                return Ok("person registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"person cant be added,{ex.Message}");

            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] login loginperson)
        {
            await LoginValidation(loginperson);
            var tokenString = await _testservice.LoginPerson(loginperson);
            if (tokenString != null)
            {
                var user = await _context.Persons.FirstOrDefaultAsync(x => x.userName == loginperson.username);

                return Ok(
                    new
                    {
                        Id = user.PersonId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Token = tokenString
                    });
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }
        }
        [Authorize]
        [HttpGet("getuser")]
        public async Task<ActionResult<IEnumerable<Person>>> GetAllPerson()
        {
            var persons = await _testservice.GetAll();
            if (!persons.Any())
            {
                return NotFound("there are no person");
            }
            else
            {
                return Ok(persons);
            }
        }
        [HttpGet("/{id}")]
        public async Task<ActionResult<Person>> GetPersonById(int id)
        {
            var persons =_testservice.Find(id);
            if (persons == null)
            {
                return NotFound();
            }
            return Ok(persons);
        }
        [HttpGet("queryString")]
        public async Task<ActionResult<IEnumerable<Person>>> filterUser([FromQuery] double salary, string city)
        {
            var filterUser = await _context.Persons
                    .Include(x => x.PersonAddress)
                    .Where(x => x.Salary == salary && x.PersonAddress.City == city)
                    .ToListAsync();
            if (filterUser.ToList().Count == 0)
            {
                return NotFound();
            }
            return Ok(filterUser.ToList());
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdatePersonID(int id, Person person)
        {
            var validator = new userValidation();
            var validationResult = validator.Validate(person);

            var errorMessage = "";
            if (!validationResult.IsValid)
            {
                foreach (var item in validationResult.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                return BadRequest(errorMessage);

            }
            var existingPerson = await _context.Persons.Include(x => x.PersonAddress).FirstOrDefaultAsync(x => x.PersonId == id);
            if (existingPerson != null)
            {
                return BadRequest("person not found");
            }
            existingPerson.email = person.email;
            existingPerson.password = person.password;
            existingPerson.CreateTime = DateTime.Now;
            existingPerson.FirstName = person.FirstName;
            existingPerson.LastName = person.LastName;
            existingPerson.PersonAddress.City = person.PersonAddress.City;
            existingPerson.PersonAddress.Country = person.PersonAddress.Country;
            existingPerson.PersonAddress.HomeNumber = person.PersonAddress.HomeNumber;
            existingPerson.JobPosition = person.JobPosition;
            _context.Update(existingPerson);
            _context.SaveChanges();
            return Ok(existingPerson);
        }
        [HttpDelete("id")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> deletePerson(int id)
        {
            var existingPerson = await _context.Persons.FirstOrDefaultAsync(x => x.PersonId == id);
            if (existingPerson == null)
            {
                return NotFound();
            }
            _context.Persons.Remove(existingPerson);
            _context.SaveChanges();
            return Ok(existingPerson);
        }

        
        private async Task<ActionResult<string>> LoginValidation([FromBody] login loginperson)
        {
            var validator = new loginValidation();
            var valid = validator.Validate(loginperson);
            var errorMessage = "";
            if (!valid.IsValid)
            {
                foreach (var item in valid.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                return BadRequest(errorMessage);

            }
            return Ok("you can continue login, there is no error in validation");
        }
        private async Task<ActionResult<string>> RegistrationValidation([FromBody] Registration registration)
        {
            var validator = new registerValidation();
            var valid = validator.Validate(registration);
            var errorMessage = "";
            if (!valid.IsValid)
            {
                foreach (var item in valid.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                return BadRequest(errorMessage);

            }

            if (await _context.Persons.AnyAsync(x => x.email == registration.email))
                return Conflict("email is already registered");
            return Ok("There is no validation");
        }

    }

   
}
