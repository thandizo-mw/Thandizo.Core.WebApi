using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface ICountryService
    {
        Task<OutputResponse> Add(CountryDTO country);
        Task<OutputResponse> Delete(string countryCode);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(string countryCode);
        Task<OutputResponse> Update(CountryDTO country);
    }
}