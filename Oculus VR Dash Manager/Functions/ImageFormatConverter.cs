using ImageMagick;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace OVR_Dash_Manager.Functions
{
    public static class ImageConverter
    {
        public static void ConvertDdsToPng(string inputPath, string outputPath)
        {
            try
            {
                using (var image = Pfim.Pfimage.FromFile(inputPath))
                {
                    PixelFormat format;

                    switch (image.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
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
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, $"Error converting DDS to PNG. InputPath: {inputPath}, OutputPath: {outputPath}");
            }
        }

        public static void ConvertPngToDds(string pngFilePath, string ddsFilePath)
        {
            try
            {
                using (MagickImage image = new MagickImage(pngFilePath))
                {
                    // You can set various options for DDS format, such as compression
                    image.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt5");

                    // Save the image as DDS
                    image.Write(ddsFilePath);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, $"Error converting PNG to DDS. PNGFilePath: {pngFilePath}, DDSFilePath: {ddsFilePath}");
            }
        }
    }
}