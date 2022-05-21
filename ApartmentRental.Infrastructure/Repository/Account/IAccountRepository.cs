namespace ApartmentRental.Infrastructure.Repository.Account;

public interface IAccountRepository : IRepository<Entities.Account>
{
    Task<Entities.Account> CreateAndGetAsync(Entities.Account account);

}