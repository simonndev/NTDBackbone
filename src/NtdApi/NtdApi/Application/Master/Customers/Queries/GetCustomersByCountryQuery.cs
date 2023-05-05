using MediatR;
using Microsoft.EntityFrameworkCore;
using NtdEntities.Migrations.SqlServer;

namespace NtdApi.Application.Master.Customers.Queries
{
    public class GetCustomersByCountryResult : NtdResultBase<IEnumerable<CustomerModel>>
    {
    }

    public class GetCustomersByCountryQuery : IRequest<GetCustomersByCountryResult>
    {
        public string Country { get; set; }
    }

    public class GetCustomersByCountryQueryHandler : IRequestHandler<GetCustomersByCountryQuery, GetCustomersByCountryResult>
    {
        private readonly NtdDbContext _dbContext;

        public GetCustomersByCountryQueryHandler(NtdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<GetCustomersByCountryResult> Handle(GetCustomersByCountryQuery request, CancellationToken cancellationToken)
        {
            var customers = _dbContext.Customers
                .Include(c => c.Employees)
                .Include(c => c.MasterOffice)
                .Where(c => c.Country == request.Country
                    && (c.IsActive.HasValue && c.IsActive.Value)
                    && !(c.IsDeleted.HasValue && c.IsDeleted.Value))
                .AsNoTracking()
                .OrderBy(c => c.CreatedOn)
                .Select(c => new CustomerModel
                {
                    Id = c.Id.ToString(),
                    CompanyName = c.CompanyName,
                    Address = c.Address,
                    City = c.City,
                    Region = c.Region,
                    Province = c.Province,
                    PostalCode = c.PostalCode,
                    Country = c.Country,
                    IsActive = c.IsActive.HasValue ? c.IsActive.Value : false,
                    ConcurrencyToken = c.RowVersion
                });

            var response = new GetCustomersByCountryResult();
            if (customers.Any())
            {
                response.Success = true;
                response.HttpStatusCode = StatusCodes.Status200OK;
                response.Payload = customers;
            }
            else
            {
                response.Success = false;
                response.HttpStatusCode = StatusCodes.Status404NotFound;
            }

            return Task.FromResult(response);
        }
    }
}
