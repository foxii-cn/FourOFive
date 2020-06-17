using FreeSql.DataAnnotations;
using LibraryManagementSystem.Models.DataBaseModels;
using System;
using System.Linq;
using System.Reflection;

namespace LibraryManagementSystem.Models.DataBaseModels.ExtensionMethods
{
    public static class ModelsExtensionMethods
    {
        public static T Copy<T>(this T element) where T : DatabaseModel, new()
        {
            if (element == null)
                return element;
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T newBorn = new T();
            propertyInfos.ToList().ForEach(p =>
            {
                Attribute attribute = p.GetCustomAttributes(typeof(ColumnAttribute)).FirstOrDefault();
                if (attribute == null || !((ColumnAttribute)attribute).IsIgnore)
                    p.SetValue(newBorn, p.GetValue(element));
            });
            return newBorn;
        }
    }
}
