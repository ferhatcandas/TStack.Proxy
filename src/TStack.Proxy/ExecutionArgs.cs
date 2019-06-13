using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace TStack.Proxy
{
    public class ExecutionArgs
    {
        public ExecutionArgs(MethodInfo _methodInfo, object[] args, ExecutionType _executionType, object _result = null, Exception _exception = null, TimeSpan? _total = null, Thread _thread = null)
        {
            MethodInfo = _methodInfo;
            Arguments = args;
            Exception = _exception;
            Total = _total;
            ActiveThread = _thread;
            Result = _result;
            ExecutionLevel = _executionType;
        }
        public MethodInfo MethodInfo { get; private set; }
        public object[] Arguments { get; private set; }
        public Exception Exception { get; private set; }
        public TimeSpan? Total { get; private set; }
        public Thread ActiveThread { get; private set; }
        public string NameOfLogger { get; private set; }
        public object Result { get; private set; }
        public ExecutionType ExecutionLevel { get; private set; }
        public List<ExecutionArgs> Childs { get; set; }
        public bool HasChilds()
        {
            if (Childs != null && Childs.Count != 0)
                return true;
            return false;
        }
        internal void SetLogName(string name)
        {
            NameOfLogger = name;
        }
        private void Fill(ExecutionArgs executionArgs, TimeSpan _total, object _result)
        {
            executionArgs.Total = _total;
            executionArgs.Result = _result;
        }
        public void Fill(ExecutionArgs executionArgs)
        {
            if (HasChilds())
            {
                var last = Childs.LastOrDefault();
                if (last.Total == null)
                {
                    Fill(last, (TimeSpan)executionArgs.Total, executionArgs.Result);
                }
                else
                {
                    Fill(this, (TimeSpan)executionArgs.Total, executionArgs.Result);
                }
            }
            else
            {
                Fill(this, (TimeSpan)executionArgs.Total, executionArgs.Result);
            }
        }
    }
}
