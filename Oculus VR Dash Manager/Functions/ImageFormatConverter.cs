using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using ImageMagick;


namespace OVR_Dash_Manager.Functions
{
    public static class ImageConverter
    {
        public static void ConvertDdsToPng(string inputPath, string outputPath)
        {
            using (var image = Pfim.Pfimage.FromFile(inputPath))
            {
                PixelFormat format;

                switch (image.Format)
                {
                    case Pfim.ImageFormat.Rgba32:
                        format = PixelFormat.Format32bppArgb;
                        format = PixelFormat.Format32bppArgb;
                        break;
                    default:
                        throw new NotImplementedException($"The format {image.Format} is not supported");
                }

                var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                try
                {
                    var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                    var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                    bitmap.Save(outputPath, ImageFormat.Png);
                }
                finally
                {
                    handle.Free();
                }
            }
        }

        public static void ConvertPngToDds(string pngFilePath, string ddsFilePath)
        {
            using (MagickImage image = new MagickImage(pngFilePath))
            {
                // You can set various options for DDS format, such as compression
                image.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt5");

                // Save the image as DDS
                image.Write(ddsFilePath);
            }
        }
    }
}
