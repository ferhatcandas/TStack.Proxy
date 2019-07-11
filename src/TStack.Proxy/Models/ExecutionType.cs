using System;
using System.Collections.Generic;
using System.Text;

namespace TStack.Proxy.Models
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
