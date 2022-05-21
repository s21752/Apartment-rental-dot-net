namespace ApartmentRental.Infrastructure.Repository.Address;

public interface IAddressRepository : IRepository<Entities.Address>
{
    Task<int> GetAddressIdByItsAttributesAsync(string country, string city, string zipCode, string street,
        string buildingNumber, string apartmentNumber);

    Task<Entities.Address> CreateAndGetAsync(Entities.Address address);
}