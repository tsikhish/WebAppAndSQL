using dataOfSql;
using domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using sqlAndWebApi.Controllers;
using sqlAndWebApi.helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace sqlandwebapi.services
{
    public interface IUserservices
    {
        Task<string> Login(login personmodel);
        IEnumerable<Person> getAll();
        Task<Person> GetPersonById(int id);
    }
    //public class UserServices : IUserservices
    //{
    //    private readonly PersonContext _context;
    //    private readonly Appsettings _appsettings;

    //    public UserServices(PersonContext context, IOptions<Appsettings> appSettings)
    //    {
    //        _context = context;
    //        _appsettings = appSettings.Value;
    //    }
        
    //    public async Task<string> Login([FromBody] login loginperson)
    //    {

    //        var user = await _context.Persons.FirstOrDefaultAsync(x => x.userName == loginperson.username);
    //        if (user == null || !BCrypt.Net.BCrypt.Verify(loginperson.password, user.password))
    //        {
    //            return null;
    //        }
    //        var authClaims = new List<Claim>
    //        {
    //            new Claim(ClaimTypes.Name,user.userName),
    //            new Claim(ClaimTypes.NameIdentifier,user.PersonId.ToString()),
    //            new Claim(ClaimTypes.Role,user.Role)
    //        };
    //        var tokenString = GenerateToken(authClaims);
    //        return tokenString;

    //    }
    //    private string GenerateToken(List<Claim> claims)
    //    {
    //        var key = Encoding.ASCII.GetBytes(_appsettings.Secret);
    //        var authSecret = new SymmetricSecurityKey(key);
    //        var tokenObject = new JwtSecurityToken(
    //            expires: DateTime.Now.AddDays(1),
    //            claims: claims,
    //            signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
    //            );
    //        var tokenHandler = new JwtSecurityTokenHandler();

    //        var token = tokenHandler.WriteToken(tokenObject);
    //        return token;
    //    }


    //}
}
