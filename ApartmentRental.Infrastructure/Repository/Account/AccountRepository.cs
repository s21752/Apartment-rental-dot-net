using ApartmentRental.Infrastructure.Context;
using ApartmentRental.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ApartmentRental.Infrastructure.Repository.Account;

public class AccountRepository : IAccountRepository
{
    
    private readonly MainContext _mainContext;

    public AccountRepository(MainContext mainContext)
    {
        _mainContext = mainContext;
    }

    public async Task<IEnumerable<Entities.Account>> GetAllAsync()
    {
        var accounts = await _mainContext.Account.ToListAsync();
        
        foreach (var account in accounts)
        {
            await _mainContext.Entry(account).Reference(x => x.Address).LoadAsync();
        }

        return accounts;
    }

    public async Task<Entities.Account> GetByIdAsync(int id)
    {
        var account = await _mainContext.Account.SingleOrDefaultAsync(x => x.Id == id);

        if (account != null)
        {
            await _mainContext.Entry(account).Reference(x => x.Address).LoadAsync();

            return account;
        }

        throw new EntityNotFoundException();
    }

    public async Task<Entities.Account> CreateAndGetAsync(Entities.Account account)
    {
        account.DateOfCreation = DateTime.UtcNow;
        account.DateOfUpdate = DateTime.UtcNow;
        await _mainContext.AddAsync(account);
        await _mainContext.SaveChangesAsync();

        return account;
    }

    public async Task AddAsync(Entities.Account entity)
    {
        var existingAccount = _mainContext.Account.SingleOrDefault(x => x.Id == entity.Id || (
            x.Address == entity.Address
            && x.Email == entity.Email
            && x.Name == entity.Name
            && x.Surname == entity.Surname
            && x.PhoneNumber == entity.PhoneNumber
            && x.IsAccountActive == entity.IsAccountActive
            && x.Name == entity.Name
        ));

        if (existingAccount != null)
        {
            throw new EntityAlreadyExistingException();
        }
        
        entity.DateOfCreation = DateTime.UtcNow;
        await _mainContext.AddAsync(entity);
        await _mainContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Entities.Account entity)
    {
        var accountToUpdate = _mainContext.Account.SingleOrDefault(x => x.Id == entity.Id);

        if (accountToUpdate == null)
        {
            throw new EntityNotFoundException();
        }

        accountToUpdate.Address = entity.Address;
        accountToUpdate.Email = entity.Email;
        accountToUpdate.Name = entity.Name;
        accountToUpdate.Surname = entity.Surname;
        accountToUpdate.PhoneNumber = entity.PhoneNumber;
        accountToUpdate.IsAccountActive = entity.IsAccountActive;

        accountToUpdate.DateOfUpdate = DateTime.UtcNow;

        await _mainContext.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var accountToDelete = await _mainContext.Account.SingleOrDefaultAsync(x => x.Id == id);

        if (accountToDelete != null)
        {
            _mainContext.Account.Remove(accountToDelete);
            await _mainContext.SaveChangesAsync();
        }

        throw new EntityNotFoundException();
    }
}