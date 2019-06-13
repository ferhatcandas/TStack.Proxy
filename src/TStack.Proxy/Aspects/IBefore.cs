using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace TStack.Proxy.Aspects
{
    public interface IBefore :IAspect
    {
        void OnBefore(ExecutionArgs executionArgs);
    }
}
