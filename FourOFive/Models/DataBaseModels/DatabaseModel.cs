using FreeSql.DataAnnotations;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FourOFive.Models.DataBaseModels
{
    public abstract class DatabaseModel
    {
        [Column(ServerTime = DateTimeKind.Local, CanUpdate = false)]
        public DateTime CreateTime { get; set; }
        [Column(ServerTime = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(200);
            PropertyInfo[] propertyInfos = GetType().GetProperties();
            builder.Append("{");
            propertyInfos.ToList().ForEach(p =>
            {
                object value = p.GetValue(this);
                if (value != null)
                {
                    builder.Append(p.Name);
                    builder.Append(":");
                    builder.Append(value);
                    builder.Append(", ");
                }
            });
            builder.Remove(builder.Length - 2, 2);
            builder.Append('}');
            return builder.ToString();
        }
    }
}
