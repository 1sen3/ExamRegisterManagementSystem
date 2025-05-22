using Microsoft.UI.Xaml.Data;
using Microsoft.UI;
using Windows.UI;
using System;

namespace App5.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string status = value as string;

            switch (status)
            {
                case "进行中":
                    return Microsoft.UI.Colors.Green;
                case "未开始":
                    return Microsoft.UI.Colors.DodgerBlue;
                case "已过期":
                    return Microsoft.UI.Colors.Gray;
                default:
                    return Microsoft.UI.Colors.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
