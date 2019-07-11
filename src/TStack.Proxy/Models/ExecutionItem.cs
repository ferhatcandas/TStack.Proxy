using System;
using System.Collections.Generic;
using System.Text;
using TStack.Proxy.Aspects;

namespace TStack.Proxy.Models
{
    internal class ExecutionItem
    {
        public ExecutionArgs ExecutionArgs { get; set; }
        public IAspect Aspect { get; set; }
    }
}
