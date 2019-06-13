using System;
using System.Collections.Generic;
using System.Text;

namespace TStack.Proxy.Aspects
{
    public interface ISuccess : IAspect
    {
        void OnSuccess(ExecutionArgs executionArgs);
    }
}
