using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TStack.Proxy.Aspects
{
    public interface IException : IAspect
    {
        void OnException(ExecutionArgs executionArgs);
    }
}
