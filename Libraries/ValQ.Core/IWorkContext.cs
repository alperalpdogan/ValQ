using ValQ.Core.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Core
{
    /// <summary>
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets current user
        /// </summary>
        /// <returns></returns>
        Task<User> GetCurrentUserAsync();
    }
}
