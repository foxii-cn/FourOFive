using FreeSql.DataAnnotations;
using LibraryManagementSystem.Utilities;
using System;

namespace LibraryManagementSystem.Models.DataBaseModels
{
    public abstract class DatabaseModel
    {
        [Column(ServerTime = DateTimeKind.Local, CanUpdate = false)]
        public DateTime CreateTime { get; set; }
        [Column(ServerTime = DateTimeKind.Local)]
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
    }
}
