using BankCQRS.Application.Features.Transactions.Commands.CreateTransaction;
using BankCQRS.Application.Features.Transactions.Queries.GetTransactionDetail;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankCQRS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{id}", Name = "GetTransactionById")]
        public async Task<ActionResult<TransactionDetailVm>> GetEventById(Guid id)
        {
            var getEventDetailQuery = new GetTransactionDetailQuery() { Id = id };
            return Ok(await _mediator.Send(getEventDetailQuery));
        }

        [HttpPost(Name = "AddTransaction")]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateTransactionCommand createEventCommand)
        {
            var id = await _mediator.Send(createEventCommand);
            return Ok(id);
        }
    }
}
