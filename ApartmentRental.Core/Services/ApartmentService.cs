using ApartmentRental.Core.DTO;
using ApartmentRental.Infrastructure.Entities;
using ApartmentRental.Infrastructure.Exceptions;
using ApartmentRental.Infrastructure.Repository.Apartment;
using ApartmentRental.Infrastructure.Repository.Landlord;

namespace ApartmentRental.Core.Services;

public class ApartmentService : IApartmentService
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly ILandlordRepository _landlordRepository;
    private readonly IAddressService _addressService;

    public ApartmentService(IApartmentRepository apartmentRepository, ILandlordRepository landlordRepository,
        IAddressService addressService)
    {
        _apartmentRepository = apartmentRepository;
        _landlordRepository = landlordRepository;
        _addressService = addressService;
    }

    public async Task<IEnumerable<ApartmentBasicInformationResponseDto>> GetAllApartmentsBasicInfosAsync()
    {
        var apartments = await _apartmentRepository.GetAllAsync();

        return apartments.Select(x => new ApartmentBasicInformationResponseDto(
            x.RentPrice,
            x.RoomQuantity,
            x.HomeSquareMeterSize,
            x.HasElevator,
            x.Address.City,
            x.Address.Street));
    }

    public async Task AddNewApartmentToExistingLandLordAsync(ApartmentCreationRequestDto dto)
    {
        var landlord = await _landlordRepository.GetByIdAsync(dto.LandLordId);

        if (landlord == null)
        {
            throw new EntityNotFoundException();
        }

        var addressId = await _addressService.GetAddressIdOrCreateAsync(dto.Country, dto.City, dto.ZipCode, dto.Street,
            dto.BuildingNumber, dto.ApartmentNumber);

        await _apartmentRepository.AddAsync(new Apartment
        {
            AddressId = addressId,
            Floor = dto.Floor,
            Images = new List<Image>(),
            LandlordId = landlord.Id,
            HasElevator = dto.IsElevator,
            RentPrice = dto.RentAmount,
            HomeSquareMeterSize = dto.SquareMeters,
            RoomQuantity = dto.NumberOfRooms
        });
    }

    public async Task<ApartmentBasicInformationResponseDto?> GetTheCheapestApartmentAsync()
    {
        var apartments = await _apartmentRepository.GetAllAsync();

        var cheapest = apartments.MinBy(x => x.RentPrice);

        if (cheapest is null) return null;

        return new ApartmentBasicInformationResponseDto(
            cheapest.RentPrice,
            cheapest.RoomQuantity,
            cheapest.HomeSquareMeterSize,
            cheapest.HasElevator,
            cheapest.Address.City,
            cheapest.Address.Street);
    }
}