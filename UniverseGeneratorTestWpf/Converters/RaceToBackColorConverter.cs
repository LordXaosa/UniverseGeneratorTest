using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Common.Models;

namespace UniverseGeneratorTestWpf.Converters
{
    public class RaceToBackColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RaceEnum race = (RaceEnum)value;
            if (race == RaceEnum.Argon)
            {
                return new SolidColorBrush(Colors.White);
            }
            else if (race == RaceEnum.Paranid)
            {
                return new SolidColorBrush(Colors.Black);
            }
            else if (race == RaceEnum.Teladi)
            {
                return new SolidColorBrush(Colors.Black);
            }
            else if (race == RaceEnum.Boron)
            {
                return new SolidColorBrush(Colors.White);
            }
            else if (race == RaceEnum.Split)
            {
                return new SolidColorBrush(Colors.White);
            }
            else if (race == RaceEnum.Pirate)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else if (race == RaceEnum.None)
            {
                return new SolidColorBrush(Colors.Black);
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}