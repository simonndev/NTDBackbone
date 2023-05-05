namespace NtdApi.Application.Master.Customers
{
    public class CustomerModel
    {
        public string? Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string? Region { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string Country { get; set; }
        public bool? IsActive { get; set; }
        public byte[]? ConcurrencyToken { get; set; }
    }
}
