using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using Cafocha.Context.Logging;

namespace Cafocha.Context.Config
{
    public class CafochaConfig : DbConfiguration
    {
        public CafochaConfig()
        {
            DbInterception.Add((new CafochaInterceptorLogging()));
        }
    }
}
