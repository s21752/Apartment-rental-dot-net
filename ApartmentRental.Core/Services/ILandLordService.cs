using ApartmentRental.Core.DTO;

namespace ApartmentRental.Core.Services;

public interface ILandLordService
{
    Task AddNewLandLordAccount(LandLordCreationRequestDto landLordCreationRequestDto);
}