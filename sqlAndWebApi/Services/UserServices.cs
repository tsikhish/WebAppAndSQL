using dataOfSql;
using domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sqlAndWebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace sqlandwebapi.services
{
    public interface IUserservices
    {
        //Person Login(login personmodel);
        IEnumerable<Person> getAll();

    }
    public class userservices : IUserservices
    {
        private readonly personcontext _context;
        public userservices(personcontext context)
        {
            _context = context;
        }

        //public async Task<IActionResult>  Login([FromBody] login loginPerson)
        //{
            
        //    var user =await _context.Persons.FirstOrDefaultAsync(x => x.userName == loginPerson.username);
        //    if (user == null)
        //        return Unauthorized;
            

        //    return user;
        //}
        public IEnumerable<Person> getAll() => _context.Persons;


    }
}
