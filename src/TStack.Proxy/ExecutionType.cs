using System;
using System.Collections.Generic;
using System.Text;

namespace TStack.Proxy
{
    public enum ExecutionType
    {
        OnBefore,
        OnAfter,
        OnSuccess,
        OnException,
        OnExit
    }
}
