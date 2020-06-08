using FreeSql.DataAnnotations;
using LibraryManagementSystem.Utilities;
using System;
using System.Linq;
using System.Reflection;

namespace LibraryManagementSystem.Models
{
    public abstract class DatabaseModel
    {
        [Column(IsPrimary = true)]
        public Guid Id { get; set; }
        [Column(ServerTime = DateTimeKind.Utc, CanUpdate = false)]
        public DateTime CreateTime { get; set; }
        [Column(ServerTime = DateTimeKind.Utc)]
        public DateTime UpdateTime { get; set; }
        public override string ToString()
        {
            try
            {
                return JsonUtility.SerializeObject(this);
            }
            catch (Exception)
            {
                return base.ToString();
            }
        }
        public static T Copy<T>(T element) where T : DatabaseModel, new()
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
