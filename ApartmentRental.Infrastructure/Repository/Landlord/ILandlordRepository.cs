namespace ApartmentRental.Infrastructure.Repository.Landlord;

public interface ILandlordRepository : IRepository<Entities.Landlord>
{
    Task<Entities.Landlord> CreateAndGetAsync(Entities.Landlord landlord);
}