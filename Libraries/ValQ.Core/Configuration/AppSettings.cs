using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValQ.Core.Configuration
{
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets distributed cache configuration parameters
        /// </summary>
        public DistributedCacheConfig DistributedCacheConfig { get; set; }

        /// <summary>
        /// Gets or sets cache configuration parameters
        /// </summary>
        public CacheConfig CacheConfig { get; set; } = new CacheConfig();

        /// <summary>
        /// Gets or sets JWT configuration
        /// </summary>
        public JWTConfig JWTConfig { get; set; }

        /// <summary>
        /// Gets or sets password configuration parameters
        /// </summary>
        public PasswordConfig PasswordConfig { get; set; }

        /// <summary>
        /// Gets or sets game configuration
        /// </summary>
        public GameConfig GameConfig { get; set; } = new GameConfig();
    }
}
