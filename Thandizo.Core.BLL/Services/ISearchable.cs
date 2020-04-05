using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface ISearchable
    {
        Task<OutputResponse> Search(string searchText);
    }
}
