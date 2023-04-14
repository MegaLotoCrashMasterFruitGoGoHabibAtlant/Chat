using Chatiks.Core.Data.EF;
using Chatiks.Core.Domain;
using Chatiks.Core.DomainApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Core.DomainApi;

public class ImagesStore: IImagesStore
{
    private readonly CoreContext _coreContext;
    
    public ImagesStore(CoreContext coreContext)
    {
        _coreContext = coreContext;
    }
    
    public Task<Image> GetImageAsync(long id)
    {
        return _coreContext.Images.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Image> GetOrCreateImageAsync(string base64String)
    {
        var image = new Image(base64String);
        
        var existingImage = _coreContext.Images
            .AsNoTracking()
            .FirstOrDefault(x => x.Base64String == image.Base64String);

        if (existingImage != null)
        {
            return existingImage;
        }
        
        _coreContext.Images.Add(image);
        
        await _coreContext.SaveChangesAsync();
        
        return image;
    }

    public async Task<ICollection<Image>> GetImagesAsync(ICollection<long> ids)
    {
         return await _coreContext.Images
             .AsNoTracking()
             .Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async Task<ICollection<Image>> GetOrCreateImagesAsync(ICollection<string> base64Strings)
    {
        var images = base64Strings.Select(x => new Image(x)).ToList();
        
        var existingImages = _coreContext.Images
            .AsNoTracking()
            .Where(x => base64Strings.Contains(x.Base64String)).ToList();
        
        var newImages = images.Where(x => existingImages.All(y => y.Base64String != x.Base64String)).ToList();
        
        _coreContext.Images.AddRange(newImages);
        
        await _coreContext.SaveChangesAsync();
        
        return existingImages.Concat(newImages).ToList();
    }
}