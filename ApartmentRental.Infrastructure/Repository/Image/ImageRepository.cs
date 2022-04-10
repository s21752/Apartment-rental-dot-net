using ApartmentRental.Infrastructure.Context;
using ApartmentRental.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ApartmentRental.Infrastructure.Repository.Image;

public class ImageRepository : IImageRepository
{
    private readonly MainContext _mainContext;

    public ImageRepository(MainContext mainContext)
    {
        _mainContext = mainContext;
    }

    public async Task<IEnumerable<Entities.Image>> GetAllAsync()
    {
        var images = await _mainContext.Image.ToListAsync();

        foreach (var image in images)
        {
            await _mainContext.Entry(image).Reference(x => x.Apartment).LoadAsync();
        }

        return images;
    }

    public async Task<Entities.Image> GetByIdAsync(int id)
    {
        var image = await _mainContext.Image.SingleOrDefaultAsync(x => x.Id == id);

        if (image != null)
        {
            await _mainContext.Entry(image).Reference(x => x.Apartment).LoadAsync();
            return image;
        }

        throw new EntityNotFoundException();
    }

    public async Task AddAsync(Entities.Image entity)
    {
        var existingImage = _mainContext.Image.SingleOrDefault(x => x.Id == entity.Id || (
            x.Apartment == entity.Apartment
            && x.Data == entity.Data
        ));

        if (existingImage != null)
        {
            throw new EntityAlreadyExistingException();
        }
        
        entity.DateOfCreation = DateTime.UtcNow;
        await _mainContext.AddAsync(entity);
        await _mainContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Entities.Image entity)
    {
        var imageToUpdate = _mainContext.Image.SingleOrDefault(x => x.Id == entity.Id);

        if (imageToUpdate == null)
        {
            throw new EntityNotFoundException();
        }

        imageToUpdate.Apartment = entity.Apartment;
        imageToUpdate.Data = entity.Data;
        
        imageToUpdate.DateOfUpdate = DateTime.UtcNow;

        await _mainContext.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var imageToDelete = await _mainContext.Image.SingleOrDefaultAsync(x => x.Id == id);

        if (imageToDelete != null)
        {
            _mainContext.Image.Remove(imageToDelete);
            await _mainContext.SaveChangesAsync();
        }

        throw new EntityNotFoundException();
    }
}