using ApartmentRental.Infrastructure.Context;
using ApartmentRental.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ApartmentRental.Infrastructure.Repository.Address;

public class AddressRepository : IAddressRepository
{
    private readonly MainContext _mainContext;

    public AddressRepository(MainContext mainContext)
    {
        _mainContext = mainContext;
    }

    public async Task<Entities.Address> CreateAndGetAsync(Entities.Address address)
    {
        address.DateOfCreation = DateTime.UtcNow;
        address.DateOfUpdate = DateTime.UtcNow;
        await _mainContext.AddAsync(address);
        await _mainContext.SaveChangesAsync();

        return address;
    }

    public async Task<int> GetAddressIdByItsAttributesAsync(string country, string city, string zipCode, string street,
        string buildingNumber,
        string apartmentNumber)
    {
        var address = await _mainContext.Address.FirstOrDefaultAsync(x =>
            x.Country == country && x.City == city && x.ZipCode == zipCode && x.Street == street &&
            x.BuildingNumber == buildingNumber && x.HomeNumber == apartmentNumber);

        return address?.Id ?? 0;
    }

    public async Task<IEnumerable<Entities.Address>> GetAllAsync()
    {
        var addresses = await _mainContext.Address.ToListAsync();

        return addresses;
    }

    public async Task<Entities.Address> GetByIdAsync(int id)
    {
        var address = await _mainContext.Address.SingleOrDefaultAsync(x => x.Id == id);

        if (address != null)
        {
            return address;
        }

        throw new EntityNotFoundException();
    }

    public async Task AddAsync(Entities.Address entity)
    {
        var existingAddress = _mainContext.Address.SingleOrDefault(x => x.Id == entity.Id || (
            x.City == entity.City
            && x.Country == entity.Country
            && x.Street == entity.Street
            && x.BuildingNumber == entity.BuildingNumber
            && x.HomeNumber == entity.HomeNumber
            && x.ZipCode == entity.ZipCode
        ));

        if (existingAddress != null)
        {
            throw new EntityAlreadyExistingException();
        }

        entity.DateOfCreation = DateTime.UtcNow;
        await _mainContext.AddAsync(entity);
        await _mainContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Entities.Address entity)
    {
        var addressToUpdate = _mainContext.Address.SingleOrDefault(x => x.Id == entity.Id);

        if (addressToUpdate == null)
        {
            throw new EntityNotFoundException();
        }

        addressToUpdate.City = entity.City;
        addressToUpdate.Country = entity.Country;
        addressToUpdate.Street = entity.Street;
        addressToUpdate.BuildingNumber = entity.BuildingNumber;
        addressToUpdate.HomeNumber = entity.HomeNumber;
        addressToUpdate.ZipCode = entity.ZipCode;

        addressToUpdate.DateOfUpdate = DateTime.UtcNow;

        await _mainContext.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var addressToDelete = await _mainContext.Address.SingleOrDefaultAsync(x => x.Id == id);

        if (addressToDelete != null)
        {
            _mainContext.Address.Remove(addressToDelete);
            await _mainContext.SaveChangesAsync();
        }

        throw new EntityNotFoundException();
    }
}