using Chatiks.Core.Domain;

namespace Chatiks.Adapters;

public class ChatImageAdapter
{
    private readonly Image _image;

    public ChatImageAdapter(Image image)
    {
        _image = image;
    }

    public string Base64Text => _image.Base64String;
}
