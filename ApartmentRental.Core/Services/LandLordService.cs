using ApartmentRental.Core.DTO;
using ApartmentRental.Infrastructure.Entities;
using ApartmentRental.Infrastructure.Repository.Account;
using ApartmentRental.Infrastructure.Repository.Address;
using ApartmentRental.Infrastructure.Repository.Apartment;
using ApartmentRental.Infrastructure.Repository.Landlord;

namespace ApartmentRental.Core.Services;

public class LandLordService : ILandLordService
{
    private readonly IAddressService _addressService;
    private readonly ILandlordRepository _landlordRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IApartmentService _apartmentService;

    public async Task AddNewLandLordAccount(LandLordCreationRequestDto landLordCreationRequestDto)
    {
        var landLordAddressId = await _addressService.GetAddressIdOrCreateAsync(landLordCreationRequestDto.Country,
            landLordCreationRequestDto.City, landLordCreationRequestDto.ZipCode, landLordCreationRequestDto.Street,
            landLordCreationRequestDto.BuildingNumber, landLordCreationRequestDto.HomeNumber);

        var landLordAddress = await _addressRepository.GetByIdAsync(landLordAddressId);

        var newLandLordAccount = await _accountRepository.CreateAndGetAsync(new Account
        {
            Address = landLordAddress,
            AddressId = landLordAddressId,
            Email = landLordCreationRequestDto.MailAddress,
            IsAccountActive = true,
            Name = landLordCreationRequestDto.Name,
            PhoneNumber = landLordCreationRequestDto.PhoneNumber,
            Surname = landLordCreationRequestDto.Surname
        });

        var newLandlord = await _landlordRepository.CreateAndGetAsync(new Landlord
        {
            Account = newLandLordAccount,
            AccountId = newLandLordAccount.Id,
        });

        await _apartmentService.AddNewApartmentToExistingLandLordAsync(new ApartmentCreationRequestDto(
            landLordCreationRequestDto.RentAmount, landLordCreationRequestDto.NumberOfRooms,
            landLordCreationRequestDto.SquareMeters, landLordCreationRequestDto.Floor,
            landLordCreationRequestDto.IsElevator, landLordCreationRequestDto.City, landLordCreationRequestDto.Street,
            landLordCreationRequestDto.ZipCode, landLordCreationRequestDto.HomeNumber,
            landLordCreationRequestDto.BuildingNumber, landLordCreationRequestDto.Country, newLandlord.Id));
    }
}