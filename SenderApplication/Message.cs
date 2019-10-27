using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SenderApplication
{
    public class Message
    {
        public string Value { get; set; }
    }

    public class CreatePerson : IRequest<Person>
    {
        public string FirstName { get; set; }
        public int Age { get; set; }
    }

    public class CreatePersonValidator : AbstractValidator<CreatePerson>
    {
        public CreatePersonValidator()
        {
            RuleFor(x => x.Age).NotEmpty();

            RuleFor(x => x.FirstName).NotEmpty();
        }
    }

    public class UpdatePerson : IRequest<Person>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public int Age { get; set; }
    }

    public class DeletePerson : IRequest
    {
        public Guid Id { get; set; }
    }
}
