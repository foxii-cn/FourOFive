using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;

namespace FourOFive.Utilities.Implements
{
    public class AuthorityUtility : IAuthorityUtility
    {
        private readonly ILogManager logger;
        private readonly IConfigurationManager config;

        private readonly int administrator = 100;
        private readonly int initial = 5;

        public AuthorityUtility(ILogManagerFactory loggerFactory, IConfigurationManagerFactory configFactory)
        {
            logger = loggerFactory.CreateManager<AuthorityUtility>();
            config = configFactory.CreateManager("Authority");

            administrator = config.Get("AdministratorAuthority", administrator);
            initial = config.Get("InitialAuthorityValue", initial);
        }
        public int GetInitialUserAuthorityValue()
        {
            return initial;
        }

        public AuthorityLevel GetLevel(User user)
        {
            if (user == null)
            {
                return AuthorityLevel.Visitor;
            }

            if (user.Authority < administrator)
            {
                return AuthorityLevel.Member;
            }
            else
            {
                return AuthorityLevel.Administrator;
            }
        }

        public bool IsLevel(User user, AuthorityLevel level)
        {
            return GetLevel(user) == level;
        }
    }
}
