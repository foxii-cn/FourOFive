using FourOFive.Models.DataBaseModels;
using System;

namespace FourOFive.Models.DataPackages
{
    public class ChosenBookDataPackage
    {
        public ChosenBookDataPackage() { }
        public ChosenBookDataPackage(Book dataSource)
        {
            Id = dataSource.Id;
            Title = dataSource.Title;
        }
        public Guid Id { get;  }
        public string Title { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Id.Equals(((ChosenBookDataPackage)obj).Id);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(ChosenBookDataPackage dp1, ChosenBookDataPackage dp2)
        {
            return dp1.Id == dp2.Id;
        }
        public static bool operator !=(ChosenBookDataPackage dp1, ChosenBookDataPackage dp2)
        {
            return dp1.Id != dp2.Id;
        }
    }
}
