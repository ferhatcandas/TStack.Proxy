using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace TStack.Proxy.Models
{
    public class ExecutionArgs
    {
        public ExecutionArgs(MethodInfo _methodInfo, object[] args, ExecutionType _executionType, string _loggerName, object _result = null, Exception _exception = null)
        {
            MethodInfo = _methodInfo;
            Arguments = args;
            Exception = _exception;
            Result = _result;
            ExecutionLevel = _executionType;
            NameOfLogger = _loggerName;
        }
        public MethodInfo MethodInfo { get; private set; }
        public object[] Arguments { get; private set; }
        public Exception Exception { get; private set; }
        public TimeSpan? Total { get; private set; }
        public Stopwatch ElapsedTime { get; private set; }
        public string NameOfLogger { get; private set; }
        public object Result { get; private set; }
        public ExecutionType ExecutionLevel { get; private set; }
        internal void setWatch()
        {
            ElapsedTime = new Stopwatch();
            ElapsedTime.Start();
        }
        internal void stopWatch()
        {
            if (ElapsedTime != null)
                if (ElapsedTime.IsRunning)
                {
                    ElapsedTime.Stop();
                    Total = ElapsedTime.Elapsed;
                }
        }
        internal void setResult(object result)
        {
            if (Result == null)
                Result = result;
        }
        internal void setException(Exception exception)
        {
            if (Exception == null)
                Exception = exception;
        }
        internal void setExecutionLevel(ExecutionType executionType)
        {
            ExecutionLevel = executionType;
        }
    }
}
