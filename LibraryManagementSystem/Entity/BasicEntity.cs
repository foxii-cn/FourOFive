using FreeSql.DataAnnotations;
using LibraryManagementSystem.Tool;
using System;
using System.Linq;
using System.Reflection;

namespace LibraryManagementSystem.Entity
{
    public abstract class BasicEntity
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
                return JsonTool.SerializeObject(this);
            }
            catch (Exception)
            {
                return base.ToString();
            }
        }
        public static T Copy<T>(T element) where T : BasicEntity, new()
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
