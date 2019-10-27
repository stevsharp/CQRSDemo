using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static SenderApplication.ErrorHandlerMiddleware;

namespace SenderApplication.Controllers
{
    //public abstract class BaseHandler
    //{
    //    protected IActionResult OkOrBadRequest(BusinessResult businessResult)
    //    {
    //        if (!businessResult.IsSuccess())
    //            return BadRequestSerialized(businessResult);

    //        return Ok();
    //    }

    //    private IActionResult BadRequestSerialized(BusinessResult businessResult) =>
    //           BadRequest(new ErrorResponse(businessResult.BrokenRules.Select(x => x.Rule)));
    //}

    public class PersonHandler : 
          IRequestHandler<CreatePerson, Person>
        , IRequestHandler<UpdatePerson, Person>
        , IRequestHandler<DeletePerson>
    {
        private readonly DataContext ctx;

        private readonly IBus _bus;

        public PersonHandler(DataContext ctx, IBus bus)
        {
            this.ctx = ctx;

            _bus = bus;
        }
        
        public async Task<Person> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var person = new Person
            {
                Age = request.Age,
                FirstName = request.FirstName
            };
            ctx.Add(person);
            await ctx.SaveChangesAsync();

            ///await _bus.Publish<Message>(new { Value = person.Id.ToString() });

            return person;
        }

        public async Task<Person> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await ctx.Persons.SingleOrDefaultAsync(v => v.Id == request.Id);
            if (person == null)
                throw new ApiException("Record does not exist");

            person.Age = request.Age;
            person.FirstName = request.FirstName;
            ctx.Persons.Update(person);
            await ctx.SaveChangesAsync();

            // await _bus.Publish<Message>(new { Value = person.Id.ToString() });

            return person;
        }

        public async Task<Unit> Handle(DeletePerson request, CancellationToken cancellationToken)
        {
            var person = await ctx.Persons.SingleOrDefaultAsync(v => v.Id == request.Id);
            if (person == null)
                throw new Exception("Record does not exist");

            ctx.Persons.Remove(person);
            await ctx.SaveChangesAsync();

            // await _bus.Publish<Message>(new { Value = person.Id.ToString() });

            return Unit.Value;
        }
    }
}
