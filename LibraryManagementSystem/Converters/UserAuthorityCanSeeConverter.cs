using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace LibraryManagementSystem.Converters
{
    public class UserAuthorityCanSeeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Count() < 2)
                throw new NotImplementedException();
            User account = values[0] as User;
            string para = parameter as string;
            if (!(values[1] is AuthorityService authorityService) || string.IsNullOrEmpty(para))
                throw new NotImplementedException();
            return (para.ToLower()) switch
            {
                "istourist" => account == null ? Visibility.Visible : Visibility.Collapsed,
                "isnottourist" => account != null && account.Id != null ? Visibility.Visible : Visibility.Collapsed,
                "isAdministrator" => account != null && authorityService.IsAdministrator(account.Id, account.Name, account.Authority) ? Visibility.Visible : Visibility.Collapsed,
                _ => throw new NotImplementedException()
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
