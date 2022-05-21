using System.Net.Mail;

namespace ApartmentRental.Core.DTO;

public class LandLordCreationRequestDto
{
    public String Name { get; set; }
    
    public int Floor { get; set; }

    public bool IsElevator { get; set; }

    public int NumberOfRooms { get; set; }

    public decimal RentAmount { get; set; }

    public int SquareMeters { get; set; }

    public String Surname { get; set; }
    public String MailAddress { get; set; }
    public String PhoneNumber { get; set; }
    public String Street { get; set; }
    public String HomeNumber { get; set; }
    public String BuildingNumber { get; set; }
    public String City { get; set; }
    public String ZipCode { get; set; }
    public String Country { get; set; }
}