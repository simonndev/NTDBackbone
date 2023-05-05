using NtdEntities.Migrations.SqlServer;

namespace NtdApi.Application.Master.Customers.Queries
{
    public class GetCustomerByIdResult : NtdResultBase<CustomerModel> { }

    public class GetCustomerByIdQuery : IQuery<GetCustomerByIdResult>
    {
        public GetCustomerByIdQuery(string customerId)
        {
            CustomerId = customerId;
        }

        public string CustomerId { get; private set; }
    }

    public class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, GetCustomerByIdResult>
    {
        private readonly NtdDbContext _dbContext;

        public GetCustomerByIdQueryHandler(NtdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetCustomerByIdResult> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new GetCustomerByIdResult();

            var customerId = Guid.Empty;
            if (!Guid.TryParse(request.CustomerId, out customerId))
            {
                result.Success = false;
                result.HttpStatusCode = StatusCodes.Status400BadRequest;

                return result;
            }

            var found = await _dbContext.Customers.FindAsync(customerId);
            if (found == null)
            {
                result.Success = false;
                result.HttpStatusCode = StatusCodes.Status404NotFound;

                return result;
            }

            if (found.IsDeleted.HasValue && found.IsDeleted.Value)
            {
                result.Success = false;
                result.HttpStatusCode = StatusCodes.Status404NotFound;

                return result;
            }

            var customer = new CustomerModel
            {
                Id = found.Id.ToString(),
                CompanyName = found.CompanyName,
                Address = found.Address,
                City = found.City,
                Region = found.Region,
                PostalCode = found.PostalCode,
                Country = found.Country,
                IsActive = found.IsActive
            };

            result.Success = true;
            result.HttpStatusCode = StatusCodes.Status200OK;
            result.Payload = customer;

            return result;
        }
    }
}
