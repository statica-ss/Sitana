using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Diagnostics
{
    public abstract class CrashService
    {
        public abstract string AppName { get; }

        public virtual Task Start()
        {
            return new Task(() => { });
        }

        public virtual Task<ExceptionData> SendOne(ExceptionData exceptionData)
        {
            return new Task<ExceptionData>(() => exceptionData);
        }
    }
}
