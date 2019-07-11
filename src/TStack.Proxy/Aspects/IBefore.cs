using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using TStack.Proxy.Models;

namespace TStack.Proxy.Aspects
{
    public interface IBefore :IAspect
    {
        void OnBefore(ExecutionArgs executionArgs);
    }
}
