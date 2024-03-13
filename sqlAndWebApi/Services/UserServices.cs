using data;
using domain;
using Microsoft.AspNetCore.Mvc;
using sqlAndWebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace sqlandwebapi.services
{
    public interface IUserservices
    {
        Person Login(login personmodel);
        IEnumerable<Person> getAll();

    }
    public class userservices : IUserservices
    {
        private readonly personcontext _context;
        public userservices(personcontext context)
        {
            _context = context;
        }

        public Person Login(login loginPerson)
        {
            
            var user = _context.Persons.SingleOrDefault(x => x.userName == loginPerson.username);
            if (user == null)
                return null;
            if (user.password != loginPerson.password)
                return null;

            return user;
        }
        public IEnumerable<Person> getAll() => _context.Persons;


    }
}
