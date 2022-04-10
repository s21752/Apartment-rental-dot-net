namespace ApartmentRental.Infrastructure.Entities;

public class Apartment : BaseEntity
{
    public decimal RentPrice { get; set; }
    public int RoomQuantity { get; set; }
    public int HomeSquareMeterSize { get; set; }
    public int Floor { get; set; }
    public bool HasElevator { get; set; }

    public int LandlordId { get; set; }
    public Landlord Landlord { get; set; }

    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }

    public int AddressId { get; set; }
    public Address Address { get; set; }

    public IEnumerable<Image> Images { get; set; }
}