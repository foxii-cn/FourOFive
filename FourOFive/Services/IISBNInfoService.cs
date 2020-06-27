using FourOFive.Models.DataPackages;
using System;

namespace FourOFive.Services
{
    public interface IISBNInfoService
    {
        public IObservable<ISBNInfoGetDataPackage> GetISBNInfo(IObservable<ISBNInfoGetDataPackage> getStream);
    }
}
