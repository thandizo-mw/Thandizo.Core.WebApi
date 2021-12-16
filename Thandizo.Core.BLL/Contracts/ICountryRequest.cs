using Thandizo.DataModels.Core;

namespace Thandizo.Core.BLL.Contracts
{
    public interface ICountryRequest
    {
        CountryDTO Country { get; }
        string CountryCode { get;}
    }
}
