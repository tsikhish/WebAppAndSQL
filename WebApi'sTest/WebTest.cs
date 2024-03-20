using dataOfSql;
using domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using sqlandwebapi.services;
using sqlAndWebApi.Controllers;
using sqlAndWebApi.helper;
using sqlAndWebApi.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebApi_sTest
{
    public class WebTest
    {
        private readonly Mock<ITestService> _mockWebTest;
        private readonly PersonsController _costumerservice;
        //private readonly PersonContext _context;
        private readonly Appsettings _appsesettings;

        public WebTest()
        {
            _mockWebTest = new Mock<ITestService>();
            var mockAppSettings = new Mock<IOptions<Appsettings>>();
            //_context = new PersonContext(new DbContextOptionsBuilder<PersonContext>().UseInMemoryDatabase(databaseName: "myproject").Options);
            mockAppSettings.Setup(x => x.Value).Returns(new Appsettings());
            _costumerservice = new PersonsController(
                context: null,  
                appSettings: mockAppSettings.Object,
                testservice: _mockWebTest.Object
            );
        }
        [Fact]
        public async Task GetUserByIdWhenExists_ReturnsOk()
        {
            //Arrange
            var existingPerson = GetPerson();
            _mockWebTest.Setup(x => x.Find(It.IsAny<int>())).Returns(existingPerson);
            //Act
            var result =await  _costumerservice.GetPersonById(0);
            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
        [Fact]
        public async Task GetAllPerson_ReturnsOk()
        {
            //Arrange
            var existingPerson = new List<Person> { GetPerson(), GetPerson(), GetPerson() };
            _mockWebTest.Setup(x => x.GetAll()).ReturnsAsync(existingPerson);
            //Act
            var actionResult = await _costumerservice.GetAllPerson();
    
            //Assert
            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<IEnumerable<Person>>>(actionResult);

        }
       
        [Fact]
        public async Task RegirsterUser_ReturnsTrue()
        {
            //Arrange
            var registerPerson = new Registration
            {
                userName = "testuser",
                password = "password",
                email = "test@example.com",
                Role = "ADMIN",
            };
            var person = new Person
            {
                userName = "testuser",
                password = "password",
                email = "test@example.com",
                Role = "ADMIN",
            };

            _mockWebTest.Setup(x => x.GetPersonByUsername(It.IsAny<string>()))  
                        .Returns((string username) =>
                        {
                            if (username == registerPerson.userName)
                            {
                                return null;
                            }
                            else
                            {
                                return person;
                            }
                        });
            //Act
            var resultTask = _costumerservice.RegirsterUser(registerPerson);
            var result = await resultTask;

            //Assert
            Assert.IsType<ObjectResult>(result);

        }
        //[Fact]
        //public async void LoginExistedPerson_ReturnsOk()
        //{
        //    //Arrange
        //    var loginCredentials = new login
        //    {
        //        username = "tsikhish",
        //        password = "tsikhish"
        //    };
        //    var expectedToken = "sample_token";
        //    _mockWebTest.Setup(x => x.LoginPerson(It.IsAny<login>())).ReturnsAsync(expectedToken);
        //    //Act
        //    IActionResult actionResult = await _costumerservice.Login(loginCredentials);
        //    var okObjectResult = actionResult as OkObjectResult;
        //    string result = okObjectResult?.Value?.ToString(); 
        //    //Assert
        //    Assert.Equal(expectedToken, result);

        //}
       
        //[Fact]
        //public async Task GetAllPersonWhenExist_ReturnsOk()
        //{
        //    var expectedParticipant = new List<Person> { };
        //    _mockWebTest.Setup(x => x.getAll()).Returns(expectedParticipant);
        //    //Act
        //    var result = _costumerservice.GetAllParticipants();
        //    //Arrange
        //    Assert.True(result);

        //}
        //[Fact]
        //public async Task DeleteUserWhenExists_returnsOk()
        //{
        //    //Arrange
        //    var expectedParticipant=GetPerson() ;
        //    _mockWebTest.Setup(x => x.Find(It.IsAny<int>())).Returns(expectedParticipant);
        //    //Act
        //    var result = _costumerservice.DeleteUser(0);
        //    //Arrange
        //    Assert.True(result);
        //}
        //[Fact]
        //public async Task UpdateUserWhenIdExists_ReturnsOk()
        //{
        //    var expectedParticipant=GetPerson() ;

        //}
        private Person GetPerson()
        {
            return new Person()
            {
                PersonId = 1,
                CreateTime = DateTime.Now,
                FirstName = "string",
                userName = "string",
                password = "string",
                email = "string",
                LastName = "string",
                JobPosition = "string",
                Salary = 0,
                WorkExperience = 0,
                Role = "string",

                PersonAddress = new Address()
                {
                    Id = 0,
                    Country = "string",
                    City = "string",
                    HomeNumber = 0,
                    PersonId = 0
                }
            };

        }

    }
}

