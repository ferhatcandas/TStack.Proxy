using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TStack.Proxy.Aspects;
using TStack.Proxy.Models;

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
        private ExecutionArgs _executionArgs;
        private List<ExecutionItem> _executionItems = new List<ExecutionItem>();
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            _methodInfo = targetMethod;
            _args = args;
            var aspects = GetAspects(targetMethod);
            try
            {
                // before aspects
                ExecuteBeforeAspect(aspects.Where(x => x is IBefore).ToList());
                // before aspects

                result = _methodInfo.Invoke(_decorated, _args);

                // after aspects
                ExecuteAfterAspect(aspects.Where(x => x is IAfter).ToList());
                // after aspects

                // success aspects
                ExecuteSuccessAspect(aspects.Where(x => x is ISuccess).ToList());
                // success aspects
            }
            catch (Exception ex)
            {
                //exception aspect
                _exception = ex;
                ExecuteExceptionAspect(aspects.Where(x => x is IException).ToList());
                //exception aspect
            }
            finally
            {
                //exit aspect
                ExecuteExitAspect(aspects.Where(x => x is IExit).ToList());
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
            return ((IList<IAspect>)mInfo.GetCustomAttributes(typeof(IAspect), true)).Distinct().ToList();
        }
        private void ExecuteBeforeAspect(IList<IAspect> aspects)
        {
            if (aspects != null)
                foreach (IBefore aspect in aspects)
                {
                    _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnBefore, aspect.GetType().Name, null);
                    _executionArgs.setWatch();
                    _executionItems.AddToExecutionItem(_executionArgs, aspect);
                    aspect.OnBefore(_executionArgs);
                }
        }
        private void ExecuteAfterAspect(IList<IAspect> aspects)
        {
            if (aspects != null)
                foreach (IAfter aspect in aspects)
                {
                    _executionArgs = _executionItems.GetExistsExecutionArgs(aspect);
                    if (_executionArgs == null)
                    {
                        _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnAfter, aspect.GetType().Name, result, null);
                        _executionItems.AddToExecutionItem(_executionArgs, aspect);
                    }
                    else
                    {
                        _executionArgs.stopWatch();
                        _executionArgs.setExecutionLevel(ExecutionType.OnAfter);
                        _executionArgs.setResult(result);
                    }
                    aspect.OnAfter(_executionArgs);
                }
        }
        private void ExecuteExceptionAspect(IList<IAspect> aspects)
        {
            if (aspects != null)
                foreach (IException aspect in aspects)
                {
                    _executionArgs = _executionItems.GetExistsExecutionArgs(aspect);
                    if (_executionArgs == null)
                    {
                        _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnException, aspect.GetType().Name, _exception: _exception);
                        _executionItems.AddToExecutionItem(_executionArgs, aspect);
                    }
                    else
                    {
                        _executionArgs.stopWatch();
                        _executionArgs.setExecutionLevel(ExecutionType.OnException);
                        _executionArgs.setException(_exception);
                    }
                    aspect.OnException(_executionArgs);
                }
        }
        private void ExecuteSuccessAspect(IList<IAspect> aspects)
        {
            if (aspects != null)
                foreach (ISuccess aspect in aspects)
                {
                    _executionArgs = _executionItems.GetExistsExecutionArgs(aspect);
                    if (_executionArgs == null)
                    {
                        _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnSuccess, aspect.GetType().Name, result, null);
                        _executionItems.AddToExecutionItem(_executionArgs, aspect);
                    }
                    else
                    {
                        _executionArgs.stopWatch();
                        _executionArgs.setExecutionLevel(ExecutionType.OnSuccess);
                        _executionArgs.setResult(result);
                    }
                    aspect.OnSuccess(_executionArgs);
                }
        }
        private void ExecuteExitAspect(IList<IAspect> aspects)
        {
            if (aspects != null)
                foreach (IExit aspect in aspects)
                {
                    _executionArgs = _executionItems.GetExistsExecutionArgs(aspect);
                    if (_executionArgs == null)
                    {
                        _executionArgs = new ExecutionArgs(_methodInfo, _args, ExecutionType.OnExit, aspect.GetType().Name, _exception: _exception, _result: result);
                        _executionItems.AddToExecutionItem(_executionArgs, aspect);
                    }
                    else
                    {
                        _executionArgs.stopWatch();
                        _executionArgs.setExecutionLevel(ExecutionType.OnExit);
                        _executionArgs.setResult(result);
                        _executionArgs.setException(_exception);
                    }

                    aspect.OnExit(_executionArgs);
                }
        }
    }
}
