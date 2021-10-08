using System.Drawing;
using System.IO;

namespace Engine.Foundation
{
    public interface IImageWrapper
    {
        Image FromStream(Stream stream);
    }
}
