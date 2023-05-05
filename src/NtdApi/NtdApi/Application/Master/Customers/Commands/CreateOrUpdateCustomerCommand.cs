using MediatR;
using NtdEntities.Master;
using NtdEntities.Migrations.SqlServer;

namespace NtdApi.Application.Master.Customers.Commands
{
    public class CreateOrUpdateCustomerResult : NtdResultBase<CustomerModel> { }

    public class CreateOrUpdateCustomerCommand : ICommand<CreateOrUpdateCustomerResult>
    {
        public CreateOrUpdateCustomerCommand(CustomerModel customer, string? customerId = null)
        {
            CustomerId = customerId;
            Customer = customer;
        }

        public string? CustomerId { get; set; }
        public CustomerModel Customer { get; set; }
    }

    public class CreateOrUpdateCustomerCommandHandler : ICommandHandler<CreateOrUpdateCustomerCommand, CreateOrUpdateCustomerResult>
    {
        private readonly NtdDbContext _dbContext;

        public CreateOrUpdateCustomerCommandHandler(NtdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CreateOrUpdateCustomerResult> Handle(CreateOrUpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            Guid customerId = Guid.NewGuid();
            var result = new CreateOrUpdateCustomerResult();

            if (!string.IsNullOrEmpty(request.CustomerId) && Guid.TryParse(request.CustomerId, out customerId))
            {
                Customer? customer = await _dbContext.Customers.FindAsync(customerId, cancellationToken);
                if (customer is null)
                {
                    result.Success = false;
                    result.HttpStatusCode = StatusCodes.Status404NotFound;

                    return result;
                }

                customer.CompanyName = request.Customer.CompanyName;
                customer.Address = request.Customer.Address;
                customer.City = request.Customer.City;
                customer.Region = request.Customer.Region;
                customer.PostalCode = request.Customer.PostalCode;
                customer.Country = request.Customer.Country;
                customer.IsActive = request.Customer.IsActive;
                customer.UpdatedOn = DateTime.UtcNow;
                customer.UpdatedBy = "NTD API";

                _dbContext.Update(customer);

                result.HttpStatusCode = StatusCodes.Status202Accepted;
            }
            else
            {
                var customer = new Customer
                {
                    Id = customerId,
                    CompanyName = request.Customer.CompanyName,
                    Address = request.Customer.Address,
                    City = request.Customer.City,
                    Region = request.Customer.Region,
                    PostalCode = request.Customer.PostalCode,
                    Country = request.Customer.Country,
                    IsActive = request.Customer.IsActive,
                    IsDeleted = false,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "NTD API"
                };

                _dbContext.Add(customer);

                result.HttpStatusCode = StatusCodes.Status201Created;
            }

            int saved;
            try
            {
                saved = await _dbContext.SaveChangesAsync(cancellationToken);
                if (saved > 0)
                {
                    result.Success = true;
                    request.Customer.Id = customerId.ToString();

                    result.Payload = request.Customer;
                }
                else
                {
                    result.Success = false;
                    result.HttpStatusCode = StatusCodes.Status304NotModified;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.HttpStatusCode = StatusCodes.Status500InternalServerError;
#if DEBUG
                result.Errors.Add(ex.ToString());
#else
                result.Errors.Add(ex.Message);
#endif
            }

            return result;
        }
    }
}
