using MediatR;
using Microsoft.AspNetCore.Mvc;
using NtdApi.Application.Master.Customers;
using NtdApi.Application.Master.Customers.Commands;
using NtdApi.Application.Master.Customers.Queries;

namespace NtdApi.Controllers
{
    [Route("api/v1/master/customers")]
    [ApiController]
    public class MasterCustomersController : ControllerBase
    {
        private IMediator _mediator;

        public MasterCustomersController(IMediator mediator)
        {
            _mediator = mediator;    
        }

        [HttpGet("country/{country}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCustomersByCountryResult))]
        public async Task<ActionResult> GetCustomers(string country)
        {
            var response = await _mediator.Send(new GetCustomersByCountryQuery { Country = country});
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCustomerByIdResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GetCustomerByIdResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GetCustomerByIdResult))]
        public async Task<ActionResult> GetCustomer(string id)
        {
            var response = await _mediator.Send(new GetCustomerByIdQuery(id));
            return response.HttpStatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                _ => Ok(response),
            };
        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomer([FromBody] CustomerModel customer, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateOrUpdateCustomerCommand(customer), cancellationToken);
            return response.HttpStatusCode switch
            {
                StatusCodes.Status201Created => RedirectToAction(nameof(GetCustomer), new { id = response.Payload?.Id }),
                _ => Ok(response),
            };
        }

        [HttpPut("{customerId}")]
        public async Task<ActionResult> UpdateCustomer(string customerId, [FromBody] CustomerModel customer, CancellationToken cancellationToken)
        {
            customer.Id = customerId;

            var response = await _mediator.Send(new CreateOrUpdateCustomerCommand(customer), cancellationToken);
            return response.HttpStatusCode switch
            {
                StatusCodes.Status202Accepted => RedirectToAction(nameof(GetCustomer), new { id = response.Payload?.Id }),
                StatusCodes.Status404NotFound => NotFound(response),
                _ => Ok(response)
            };
        }
    }
}
