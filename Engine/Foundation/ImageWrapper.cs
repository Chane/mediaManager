using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;

namespace Engine.Foundation
{
    [ExcludeFromCodeCoverage]
    public class ImageWrapper : IImageWrapper
    {
        public Image FromStream(Stream stream)
        {
            return Image.FromStream(stream, false, false);
        }
    }
}
