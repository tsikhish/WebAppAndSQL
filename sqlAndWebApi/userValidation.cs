using System;
using System.Data;
using domain;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
namespace sqlAndWebApi
{
    public class userValidation : AbstractValidator<Person>
    {
        public userValidation()
        {
            RuleFor(x => x.CreateTime < DateTime.Now);
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName should be filled");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName should be filled");
            RuleFor(x => x.JobPosition).NotEmpty().WithMessage("JobPosition should be filled");
            RuleFor(x => x.Salary).GreaterThan(0).LessThan(10000).WithMessage("Salary should be between 0-10000");
            RuleFor(x => x.WorkExperience).NotEmpty().WithMessage("WorkExperience should be filled");
            RuleFor(x => x.PersonAddress.Country).NotEmpty().WithMessage("country should be filled");
            RuleFor(x => x.PersonAddress.City).NotEmpty().WithMessage("city should be filled");
            RuleFor(x => x.PersonAddress.HomeNumber).NotEmpty().WithMessage("homenumber should be filled");

        }
    }
    public class loginValidation : AbstractValidator<login>
    {
        public loginValidation()
        {
            RuleFor(x => x.username).NotEmpty().WithMessage("FirstName should be filled");
            RuleFor(x => x.password).NotEmpty().WithMessage("LastName should be filled");

        }
    }
    public class registerValidation : AbstractValidator<Registration>
    {
        public registerValidation()
        {
            RuleFor(x => x.userName).NotEmpty().WithMessage("FirstName should be filled");
            RuleFor(x => x.password).NotEmpty().WithMessage("LastName should be filled");
            RuleFor(x => x.email).EmailAddress().NotEmpty().WithMessage("email should be filled");
        }
    }
}
