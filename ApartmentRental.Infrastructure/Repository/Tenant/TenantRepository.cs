using ApartmentRental.Infrastructure.Context;
using ApartmentRental.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ApartmentRental.Infrastructure.Repository.Tenant;

public class TenantRepository : ITenantRepository
{
    private readonly MainContext _mainContext;

    public TenantRepository(MainContext mainContext)
    {
        _mainContext = mainContext;
    }

    public async Task<IEnumerable<Entities.Tenant>> GetAllAsync()
    {
        var tenants = await _mainContext.Tenant.ToListAsync();

        foreach (var tenant in tenants)
        {
            await _mainContext.Entry(tenant).Reference(x => x.Apartment).LoadAsync();
            await _mainContext.Entry(tenant).Reference(x => x.Account).LoadAsync();
        }

        return tenants;
    }

    public async Task<Entities.Tenant> GetByIdAsync(int id)
    {
        var tenant = await _mainContext.Tenant.SingleOrDefaultAsync(x => x.Id == id);

        if (tenant != null)
        {
            await _mainContext.Entry(tenant).Reference(x => x.Apartment).LoadAsync();
            await _mainContext.Entry(tenant).Reference(x => x.Account).LoadAsync();
            return tenant;
        }

        throw new EntityNotFoundException();
    }

    public async Task AddAsync(Entities.Tenant entity)
    {
        var existingTenant = _mainContext.Tenant.SingleOrDefault(x => x.Id == entity.Id || (
            x.Account == entity.Account
            && x.Apartment == entity.Apartment
        ));

        if (existingTenant != null)
        {
            throw new EntityAlreadyExistingException();
        }
        
        entity.DateOfCreation = DateTime.UtcNow;
        await _mainContext.AddAsync(entity);
        await _mainContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Entities.Tenant entity)
    {
        var tenantToUpdate = _mainContext.Tenant.SingleOrDefault(x => x.Id == entity.Id);

        if (tenantToUpdate == null)
        {
            throw new EntityNotFoundException();
        }

        tenantToUpdate.Account = entity.Account;
        tenantToUpdate.Apartment = entity.Apartment;

        tenantToUpdate.DateOfUpdate = DateTime.UtcNow;

        await _mainContext.SaveChangesAsync();

    }

    public async Task DeleteByIdAsync(int id)
    {
        var tenantToDelete = await _mainContext.Tenant.SingleOrDefaultAsync(x => x.Id == id);

        if (tenantToDelete != null)
        {
            _mainContext.Tenant.Remove(tenantToDelete);
            await _mainContext.SaveChangesAsync();
        }

        throw new EntityNotFoundException();
    }
}