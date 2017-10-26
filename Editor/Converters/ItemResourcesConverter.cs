using Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Editor.Converters
{
    public class ItemResourcesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<ItemModel, decimal> resources = (Dictionary<ItemModel, decimal>)value;
            StringBuilder sb = new StringBuilder();
            foreach (var item in resources)
            {
                sb.AppendLine($"{item.Key.Name}({item.Key.Id}):{item.Value}");
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
