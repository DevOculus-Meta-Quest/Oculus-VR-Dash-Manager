using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace OVR_Dash_Manager.Functions
{
    public class NullToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                // Return an empty image or a default image
                return new BitmapImage(); // Or provide a URI to a default image
            }

            var imagePath = (string)value;
            try
            {
                if (!File.Exists(imagePath))
                {
                    // Log the error or handle it as needed
                    Debug.WriteLine($"File not found: {imagePath}");
                    return new BitmapImage(); // Or provide a URI to a default image
                }

                Uri imageUri = new Uri(new Uri("file:///"), imagePath);
                return new BitmapImage(imageUri);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading image: {ex.Message}");
                return new BitmapImage(); // Or provide a URI to a default image
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}