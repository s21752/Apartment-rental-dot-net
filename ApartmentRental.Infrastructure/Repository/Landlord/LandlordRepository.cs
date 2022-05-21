using ApartmentRental.Infrastructure.Context;
using ApartmentRental.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ApartmentRental.Infrastructure.Repository.Landlord;

public class LandlordRepository : ILandlordRepository
{
    private readonly MainContext _mainContext;

    public LandlordRepository(MainContext mainContext)
    {
        _mainContext = mainContext;
    }

    public async Task<IEnumerable<Entities.Landlord>> GetAllAsync()
    {
        var landlords = await _mainContext.Landlord.ToListAsync();

        foreach (var landlord in landlords)
        {
            await _mainContext.Entry(landlord).Reference(x => x.Apartments).LoadAsync();
            await _mainContext.Entry(landlord).Reference(x => x.Account).LoadAsync();
        }

        return landlords;
    }

    public async Task<Entities.Landlord> GetByIdAsync(int id)
    {
        var landlord = await _mainContext.Landlord.SingleOrDefaultAsync(x => x.Id == id);

        if (landlord != null)
        {
            await _mainContext.Entry(landlord).Reference(x => x.Apartments).LoadAsync();
            await _mainContext.Entry(landlord).Reference(x => x.Account).LoadAsync();

            return landlord;
        }

        throw new EntityNotFoundException();
    }

    public async Task<Entities.Landlord> CreateAndGetAsync(Entities.Landlord landlord)
    {
        landlord.DateOfCreation = DateTime.UtcNow;
        landlord.DateOfUpdate = DateTime.UtcNow;
        await _mainContext.AddAsync(landlord);
        await _mainContext.SaveChangesAsync();

        return landlord;
    }

    public async Task AddAsync(Entities.Landlord entity)
    {
        var existingLandlord = _mainContext.Landlord.SingleOrDefault(x => x.Id == entity.Id || (
            x.Account == entity.Account
            && x.Apartments == entity.Apartments
        ));

        if (existingLandlord != null)
        {
            throw new EntityAlreadyExistingException();
        }
        
        entity.DateOfCreation = DateTime.UtcNow;
        await _mainContext.AddAsync(entity);
        await _mainContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Entities.Landlord entity)
    {
        var landlordToUpdate = _mainContext.Landlord.SingleOrDefault(x => x.Id == entity.Id);

        if (landlordToUpdate == null)
        {
            throw new EntityNotFoundException();
        }

        landlordToUpdate.Account = entity.Account;
        landlordToUpdate.Apartments = entity.Apartments;

        landlordToUpdate.DateOfUpdate = DateTime.UtcNow;

        await _mainContext.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var landlordToDelete = await _mainContext.Landlord.SingleOrDefaultAsync(x => x.Id == id);

        if (landlordToDelete != null)
        {
            _mainContext.Landlord.Remove(landlordToDelete);
            await _mainContext.SaveChangesAsync();
        }

        throw new EntityNotFoundException();
    }
}