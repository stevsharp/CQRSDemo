using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SenderApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonCommandsController : ControllerBase
    {
        private readonly IMediator mediator;

        public PersonCommandsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Person>> Create(CreatePerson request)
        {
            var person = await mediator.Send(request);

            return person;
        }

        [HttpPost("update")]
        public async Task<ActionResult<Person>> Update(UpdatePerson request)
        {
            var person = await mediator.Send(request);

            return person;
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeletePerson request)
        {
            await mediator.Send(request);
            return Ok();
        }

    }
}
