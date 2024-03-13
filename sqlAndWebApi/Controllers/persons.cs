using data;
using domain;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using sqlandwebapi.services;
using sqlAndWebApi.helper;
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
    public class persons : Controller
    {
        private readonly personcontext _context;
        private readonly Appsettings _appsettings;
        private readonly IUserservices _userservices;
        public persons( personcontext context,Appsettings appsettings, IUserservices userservices)
        {
            _context = context;
            _userservices = userservices;
            _appsettings = appsettings;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<IEnumerable<Person>>> RegirsterUser(Registration user)
        {
            var validator = new registerValidation();
            var valid = validator.Validate(user);
            var errorMessage = "";
            if (!valid.IsValid)
            {
                foreach (var item in valid.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                return BadRequest(errorMessage);
                
            }
            if (await _context.Persons.AnyAsync(x => x.email == user.email))
                    return Conflict("email is already registered");
            try
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
                var newperson = new Person
                {
                    userName = user.userName,
                    password = passwordHash,
                    email = user.email,
                    Role = user.Role
                };
                _context.Persons.Add(newperson);
                _context.SaveChanges();
                return Ok("person registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"person cant be added,{ex.Message}");

            }
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] login loginperson)
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
            var user = _userservices.Login(loginperson);
            
            var tokenString = GenerateToken(user);
            return Ok(
                new
                {
                    Id = user.PersonId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = tokenString
                });

        }
        [Authorize]
        [HttpGet("getuser")]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
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
            return Ok(persons);
        }
        [HttpGet("/{id}")]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersonById(int id)
        {
            var persons = await _context.Persons.FirstOrDefaultAsync(x => x.PersonId == id);
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
        [Authorize(Roles ="ADMIN")]
        public  async Task<IActionResult> UpdatePersonID(int id,Person person)
        {
            var validator = new userValidation();
            var validationResult=validator.Validate(person);
            
            var errorMessage = "";
            if (!validationResult.IsValid)
            {
                foreach (var item in validationResult.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                return BadRequest(errorMessage);

            }
            var existingPerson=await _context.Persons.Include(x=>x.PersonAddress).FirstOrDefaultAsync(x=>x.PersonId==id);
            if (existingPerson != null)
            {
                return BadRequest("person not found");
            }
            existingPerson.email=person.email;
            existingPerson.password=person.password;
            existingPerson.CreateTime= DateTime.Now;
            existingPerson.FirstName=person.FirstName;
            existingPerson.LastName=person.LastName;
            existingPerson.PersonAddress.City = person.PersonAddress.City;
            existingPerson.PersonAddress.Country=person.PersonAddress.Country;
            existingPerson.PersonAddress.HomeNumber=person.PersonAddress.HomeNumber;
            existingPerson.JobPosition=person.JobPosition;
            _context.Update(existingPerson);
             _context.SaveChanges();
            return Ok(existingPerson);
        }
        [HttpDelete("id")]
        [Authorize(Roles ="ADMIN")]
        public async Task<IActionResult> deletePerson(int id)
        {
            var existingPerson=await _context.Persons.FirstOrDefaultAsync(x=>x.PersonId== id);
            if (existingPerson == null)
            {
                return NotFound();
            }
            _context.Persons.Remove(existingPerson);
            _context.SaveChanges();
            return Ok(existingPerson);
        }
        
        private string GenerateToken(Person persons)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, persons.PersonId.ToString()),
                    new Claim(ClaimTypes.Name, persons.FirstName.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

    }

   

   
}
