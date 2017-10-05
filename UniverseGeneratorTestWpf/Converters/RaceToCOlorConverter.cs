using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UniverseGeneratorTestWpf.Converters
{
    public class RaceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Race race = (Race)value;
            if (race == Race.Argon)
            {
                return new SolidColorBrush(Colors.Blue);
            }
            else if (race == Race.Paranid)
            {
                return new SolidColorBrush(Colors.DarkRed);
            }
            else if (race == Race.Teladi)
            {
                return new SolidColorBrush(Colors.DarkGreen);
            }
            else if (race == Race.Boron)
            {
                return new SolidColorBrush(Colors.Navy);
            }
            else if (race == Race.Split)
            {
                return new SolidColorBrush(Colors.Gold);
            }
            else if (race == Race.Pirate)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else if (race == Race.None)
            {
                return new SolidColorBrush(Colors.Violet);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
