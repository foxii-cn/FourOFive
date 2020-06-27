using FourOFive.Models.DataBaseModels;

namespace FourOFive.Utilities
{
    public interface IAuthorityUtility
    {
        public bool IsLevel(User user, AuthorityLevel level);
        public AuthorityLevel GetLevel(User user);
        public int GetInitialUserAuthorityValue();
    }
}
