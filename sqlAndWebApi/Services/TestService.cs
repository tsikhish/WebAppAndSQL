using dataOfSql;
using domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using sqlAndWebApi.helper;
using sqlAndWebApi.Migrations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace sqlAndWebApi.Services
{
    public interface ITestService
    {
        Person GetPersonByUsername(string registration);

        void AddUser(Person personRegistration);
        Task<IEnumerable<Person>> GetAll();
         //Task<Person> GetPersonById(int id);
        Person Find(int id);
       Task<string> LoginPerson([FromQuery] login login);


    }

    public class TestService : ITestService
    {
        private readonly PersonContext _context;
        private readonly Appsettings _appsettings;
        public TestService(PersonContext context, IOptions<Appsettings> appsettings)
        {
            _context=context;
            _appsettings=appsettings.Value;
        }

        public void AddUser(Person personRegistration)
        {
            _context.Persons.Add(personRegistration);
        }

        public Person GetPersonByUsername(string username)
        {
            return _context.Persons.FirstOrDefault(x => x.userName == username);
        }

        public async Task<IEnumerable<Person>> GetAll() => await _context.Persons.Include(x=>x.PersonAddress).ToListAsync();
        //public async Task<Person> GetPersonById(int id) => await _context.Persons.FindAsync(id);

        public Person Find(int id)
        {
            return _context.Persons.Find(id);
        }

        public async Task<string> LoginPerson([FromBody] login loginperson)
        {

            var user = await _context.Persons.FirstOrDefaultAsync(x => x.userName == loginperson.username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginperson.password, user.password))
            {
                return null;
            }
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.userName),
                    new Claim(ClaimTypes.NameIdentifier,user.PersonId.ToString()),
                    new Claim(ClaimTypes.Role,user.Role)
                };
            var tokenString = GenerateToken(authClaims);
            return tokenString;

        }
        private string GenerateToken(List<Claim> claims)
        {
            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);
            var authSecret = new SymmetricSecurityKey(key);
            var tokenObject = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.WriteToken(tokenObject);
            return token;
        }

    }
}
