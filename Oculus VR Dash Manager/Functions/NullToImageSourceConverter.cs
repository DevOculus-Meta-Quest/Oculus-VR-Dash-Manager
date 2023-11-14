using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace OVR_Dash_Manager.Functions
{
    public class NullToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                // Return an empty image or a default image
                return new BitmapImage(); // Or provide a URI to a default image
            }

            return new BitmapImage(new Uri((string)value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
