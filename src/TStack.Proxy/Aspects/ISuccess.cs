using System;
using System.Collections.Generic;
using System.Text;
using TStack.Proxy.Models;

namespace TStack.Proxy.Aspects
{
    public interface ISuccess : IAspect
    {
        void OnSuccess(ExecutionArgs executionArgs);
    }
}
