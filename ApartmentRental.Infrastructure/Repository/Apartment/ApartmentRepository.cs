using ApartmentRental.Infrastructure.Context;
using ApartmentRental.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ApartmentRental.Infrastructure.Repository.Apartment;

public class ApartmentRepository : IApartmentRepository
{
    private readonly MainContext _mainContext;

    public ApartmentRepository(MainContext mainContext)
    {
        _mainContext = mainContext;
    }

    public async Task<IEnumerable<Entities.Apartment>> GetAllAsync()
    {
        var apartments = await _mainContext.Apartment.ToListAsync();

        foreach (var apartment in apartments)
        {
            await _mainContext.Entry(apartment).Reference(x => x.Address).LoadAsync();
            await _mainContext.Entry(apartment).Reference(x => x.Landlord).LoadAsync();
            await _mainContext.Entry(apartment).Reference(x => x.Tenant).LoadAsync();
            await _mainContext.Entry(apartment).Reference(x => x.Images).LoadAsync();
        }

        return apartments;
    }

    public async Task<Entities.Apartment> GetByIdAsync(int id)
    {
        var apartment = await _mainContext.Apartment.SingleOrDefaultAsync(x => x.Id == id);

        if (apartment != null)
        {
            await _mainContext.Entry(apartment).Reference(x => x.Address).LoadAsync();
            await _mainContext.Entry(apartment).Reference(x => x.Landlord).LoadAsync();
            await _mainContext.Entry(apartment).Reference(x => x.Tenant).LoadAsync();
            await _mainContext.Entry(apartment).Reference(x => x.Images).LoadAsync();

            return apartment;
        }

        throw new EntityNotFoundException();
    }

    public async Task AddAsync(Entities.Apartment entity)
    {
        var existingApartment = _mainContext.Apartment.SingleOrDefault(x => x.Id == entity.Id || (
            x.Address == entity.Address
            && x.Floor == entity.Floor
            && x.Landlord == entity.Landlord
            && x.Tenant == entity.Tenant
            && x.HasElevator == entity.HasElevator
            && x.RentPrice == entity.RentPrice
            && x.RoomQuantity == entity.RoomQuantity
            && x.HomeSquareMeterSize == entity.HomeSquareMeterSize
        ));

        if (existingApartment != null)
        {
            throw new EntityAlreadyExistingException();
        }
        
        entity.DateOfCreation = DateTime.UtcNow;
        entity.DateOfUpdate = DateTime.UtcNow;

        await _mainContext.AddAsync(entity);
        await _mainContext.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var apartmentToDelete = await _mainContext.Apartment.SingleOrDefaultAsync(x => x.Id == id);

        if (apartmentToDelete != null)
        {
            _mainContext.Apartment.Remove(apartmentToDelete);
            await _mainContext.SaveChangesAsync();
        }

        throw new EntityNotFoundException();
    }

    public async Task UpdateAsync(Entities.Apartment entity)
    {
        var apartmentToUpdate = _mainContext.Apartment.SingleOrDefault(x => x.Id == entity.Id);

        if (apartmentToUpdate == null)
        {
            throw new EntityNotFoundException();
        }

        apartmentToUpdate.Address = entity.Address;
        apartmentToUpdate.HasElevator = entity.HasElevator;
        apartmentToUpdate.RentPrice = entity.RentPrice;
        apartmentToUpdate.HomeSquareMeterSize = entity.HomeSquareMeterSize;
        apartmentToUpdate.RoomQuantity = entity.RoomQuantity;
        
        apartmentToUpdate.DateOfUpdate = DateTime.UtcNow;

        await _mainContext.SaveChangesAsync();
    }
}