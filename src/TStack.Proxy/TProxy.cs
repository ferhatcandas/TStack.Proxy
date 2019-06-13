using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using TStack.Proxy.Aspects;

namespace TStack.Proxy
{
    public class TProxy<TI, T> : TProxy<T>
        where TI : class
        where T : class, TI
    {
        public static TI Create(T decorated)
        {
            object proxy = Create<TI, TProxy<T>>();
            ((TProxy<T>)proxy).SetParameters(decorated);
            return (TI)proxy;
        }
    }
    public class TProxy<T> : DispatchProxy
    {
        private T _decorated;
        private MethodInfo _methodInfo;
        private object[] _args;
        private Exception _exception;
        private object result;
        private Stopwatch _stopwatch;
        private Thread _thread;
        private ExecutionArgs _executionArgs;
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            _methodInfo = targetMethod;
            _args = args;
            var aspects = GetAspects(targetMethod);
            try
            {
                // before aspects
                ExecuteBeforeAspect(aspects);
                // before aspects

                result = _methodInfo.Invoke(_decorated, _args);

                // after aspects
                ExecuteAfterAspect(aspects);
                // after aspects

                // success aspects
                ExecuteSuccessAspect(aspects);
                // success aspects
            }
            catch (Exception ex)
            {
                //exception aspect
                _exception = ex;
                ExecuteExceptionAspect(aspects);
                //exception aspect
            }
            finally
            {
                //exit aspect
                ExecuteExitAspect(aspects);
                //exit aspect
            }

            return result;
        }



        internal void SetParameters(T decorated)
        {
            if (decorated == null)
                throw new ArgumentNullException(nameof(decorated));
            _decorated = decorated;
        }
        private IList<IAspect> GetAspects(MethodInfo methodInfo)
        {
            var realType = typeof(T);
            var mInfo = realType.GetMethod(methodInfo.Name);
            return (IList<IAspect>)mInfo.GetCustomAttributes(typeof(IAspect), true);
        }
        private void ExecuteExitAspect(IList<IAspect> aspects)
        {
            _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnExit, _exception: _exception, _result: result);
            if (aspects != null)
                foreach (var aspect in aspects)
                {
                    if (aspect is IExit)
                    {
                        _executionArgs.SetLogName(aspect.GetType().Name);
                        ((IExit)aspect).OnExit(_executionArgs);
                    }
                }
        }
        private void ExecuteSuccessAspect(IList<IAspect> aspects)
        {
            _thread = GetCurrentThread();
            _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnSuccess, result, null, _stopwatch.Elapsed, _thread);
            if (aspects != null)
                foreach (var aspect in aspects)
                {
                    if (aspect is ISuccess)
                    {
                        _executionArgs.SetLogName(aspect.GetType().Name);
                        ((ISuccess)aspect).OnSuccess(_executionArgs);
                    }
                }
        }
        private void ExecuteExceptionAspect(IList<IAspect> aspects)
        {
            _stopwatch.Stop();
            _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnException, _exception: _exception);
            if (aspects != null)
                foreach (var aspect in aspects)
                {
                    if (aspect is IException)
                    {
                        _executionArgs.SetLogName(aspect.GetType().Name);
                        ((IException)aspect).OnException(_executionArgs);
                    }
                }
        }
        private void ExecuteBeforeAspect(IList<IAspect> aspects)
        {

            _thread = GetCurrentThread();
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnBefore, null, _thread: _thread);
            if (aspects != null)
                foreach (var aspect in aspects)
                {
                    if (aspect is IBefore)
                    {
                        _executionArgs.SetLogName(aspect.GetType().Name);
                        ((IBefore)aspect).OnBefore(_executionArgs);
                    }
                }
        }
        private void ExecuteAfterAspect(IList<IAspect> aspects)
        {
            _stopwatch.Stop();
            _thread = GetCurrentThread();
            _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnAfter, result, null, _stopwatch.Elapsed, _thread);
            if (aspects != null)
                foreach (var aspect in aspects)
                {
                    if (aspect is IAfter)
                    {
                        _executionArgs.SetLogName(aspect.GetType().Name);
                        ((IAfter)aspect).OnAfter(_executionArgs);
                    }
                }
        }
        private Thread GetCurrentThread()
        {
            return Thread.CurrentThread;
        }
    }
}
