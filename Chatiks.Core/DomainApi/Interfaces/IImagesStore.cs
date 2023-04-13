using System.Collections.Generic;
using System.Threading.Tasks;
using Chatiks.Core.Domain;

namespace Chatiks.Core.DomainApi.Interfaces;

public interface IImagesStore
{
    Task<Image> GetImageAsync(long id);
    
    Task<Image> GetOrCreateImageAsync(string base64String);
    
    Task<ICollection<Image>> GetImagesAsync(ICollection<long> ids);
    
    Task<ICollection<Image>> GetOrCreateImagesAsync(ICollection<string> base64Strings);
}