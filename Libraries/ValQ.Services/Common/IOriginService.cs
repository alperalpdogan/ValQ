using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;

namespace ValQ.Services.Common
{
    public interface IOriginService
    {
        Task<Origin> GetOriginByIdAsync(int id);

        Task<List<Origin>> GetAllOriginsAsync();
    }
}
