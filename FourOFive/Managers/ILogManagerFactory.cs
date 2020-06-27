using System;

namespace FourOFive.Managers
{
    public interface ILogManagerFactory : IDisposable
    {
        public ILogManager CreateManager<T>() where T : class;
    }
}
