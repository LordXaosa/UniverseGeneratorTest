using Common;
using Common.Models;
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
            RaceEnum race = (RaceEnum)value;
            if (race == RaceEnum.Argon)
            {
                return new SolidColorBrush(Colors.Blue);
            }
            else if (race == RaceEnum.Paranid)
            {
                return new SolidColorBrush(Colors.LightGreen);
            }
            else if (race == RaceEnum.Teladi)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            else if (race == RaceEnum.Boron)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else if (race == RaceEnum.Split)
            {
                return new SolidColorBrush(Colors.Violet);
            }
            else if (race == RaceEnum.Pirate)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else if (race == RaceEnum.None)
            {
                return new SolidColorBrush(Colors.Gray);
            }
            return new SolidColorBrush(Colors.DarkRed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
