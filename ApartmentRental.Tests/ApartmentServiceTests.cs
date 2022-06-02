using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentRental.Core.DTO;
using ApartmentRental.Core.Services;
using ApartmentRental.Infrastructure.Entities;
using ApartmentRental.Infrastructure.Exceptions;
using ApartmentRental.Infrastructure.Repository.Apartment;
using ApartmentRental.Infrastructure.Repository.Landlord;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApartmentRental.Tests;

public class ApartmentServiceTests
{
    [Fact]
    public async Task GetTheCheapestApartmentAsync_ShouldReturnNull_WhenApartmentsCollectionIsNull()
    {
        var sut = new ApartmentService(Mock.Of<IApartmentRepository>(), Mock.Of<ILandlordRepository>(),
            Mock.Of<IAddressService>());

        var result = await sut.GetTheCheapestApartmentAsync();

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTheCheapestApartmentAsync_ShouldReturnTheCheapestApartment()
    {
        var apartments = new List<Apartment>
        {
            new()
            {
                Address = new Address()
                {
                    City = "city 1",
                    Country = "Country 1",
                    Street = "Street 1",
                    HomeNumber = "Home number 1",
                    BuildingNumber = "Building number 1",
                    ZipCode = "zip code 1"
                },
                Floor = 1,
                RentPrice = 2000,
                HomeSquareMeterSize = 45,
                RoomQuantity = 3,
                HasElevator = true
            },
            new()
            {
                Address = new Address()
                {
                    City = "city 2",
                    Country = "Country 2",
                    Street = "Street 2",
                    HomeNumber = "Home number 2",
                    BuildingNumber = "Building number 2",
                    ZipCode = "zip code 2"
                },
                Floor = 2,
                RentPrice = 1000,
                HomeSquareMeterSize = 37,
                RoomQuantity = 1,
                HasElevator = false
            }
        };

        var apartmentRepositoryMock = new Mock<IApartmentRepository>();
        apartmentRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(apartments);

        var sut = new ApartmentService(apartmentRepositoryMock.Object, Mock.Of<ILandlordRepository>(),
            Mock.Of<IAddressService>());

        var result = await sut.GetTheCheapestApartmentAsync();

        result.Should().NotBeNull();
        result.City.Should().Be("city 2");
        result.Street.Should().Be("Street 2");
        result.RentAmount.Should().Be(1000);
        result.SquareMeters.Should().Be(37);
        result.NumberOfRooms.Should().Be(1);
        result.IsElevatorInBuilding.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllApartmentsAsync_ShouldReturnAll()
    {
        var apartments = new List<Apartment>
        {
            new()
            {
                Address = new Address()
                {
                    City = "city 1",
                    Country = "Country 1",
                    Street = "Street 1",
                    HomeNumber = "Home number 1",
                    BuildingNumber = "Building number 1",
                    ZipCode = "zip code 1"
                },
                Floor = 1,
                RentPrice = 2000,
                HomeSquareMeterSize = 45,
                RoomQuantity = 3,
                HasElevator = true
            },
            new()
            {
                Address = new Address()
                {
                    City = "city 2",
                    Country = "Country 2",
                    Street = "Street 2",
                    HomeNumber = "Home number 2",
                    BuildingNumber = "Building number 2",
                    ZipCode = "zip code 2"
                },
                Floor = 2,
                RentPrice = 1000,
                HomeSquareMeterSize = 37,
                RoomQuantity = 1,
                HasElevator = false
            },
            new()
            {
                Address = new Address()
                {
                    City = "city 3",
                    Country = "Country 3",
                    Street = "Street 3",
                    HomeNumber = "Home number 3",
                    BuildingNumber = "Building number 3",
                    ZipCode = "zip code 3"
                },
                Floor = 3,
                RentPrice = 3000,
                HomeSquareMeterSize = 91,
                RoomQuantity = 2,
                HasElevator = true
            }
        };

        var apartmentRepositoryMock = new Mock<IApartmentRepository>();
        apartmentRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(apartments);

        var sut = new ApartmentService(apartmentRepositoryMock.Object, Mock.Of<ILandlordRepository>(),
            Mock.Of<IAddressService>());

        var result = await sut.GetAllApartmentsBasicInfosAsync();

        result.Should().NotBeNull();
        result.Count().Should().Be(3);
        result.Any(x => x.Street == "Street 3").Should().BeTrue();
        result.Any(x => x.Street == "Street 2").Should().BeTrue();
        result.Any(x => x.Street == "Street 1").Should().BeTrue();
    }

    [Fact]
    public async Task
        AddNewApartmentToExistingLandLordAsync_ShouldThrowEntityNotFoundException_WhenLandLordForProvidedIdNotExisting()
    {

        var sut = new ApartmentService(Mock.Of<IApartmentRepository>(), Mock.Of<ILandlordRepository>(),
            Mock.Of<IAddressService>());

        Func<Task> action = async () => await sut.AddNewApartmentToExistingLandLordAsync(new ApartmentCreationRequestDto(
            50, 30, 20, 2, true, "city1", "street1", "zipcode 1", "45c/3", "58", "country 1", 17));

        await action.Should().ThrowAsync<EntityNotFoundException>();
    }
}