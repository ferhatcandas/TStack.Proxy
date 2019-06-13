using System;
using System.Collections.Generic;
using System.Text;
using TStack.Proxy.Aspects;

namespace TStack.Proxy
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public abstract class MethodInterceptor : Attribute, IAspect
    {
    }
}
