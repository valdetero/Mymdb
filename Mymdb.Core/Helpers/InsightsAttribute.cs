using System;
using System.Reflection;
using System.Collections.Generic;
//using MethodDecoratorInterfaces;
using Xamarin;

namespace Mymdb.Core.Helpers
{
    [AttributeUsage(
            AttributeTargets.Method
            | AttributeTargets.Constructor
            | AttributeTargets.Assembly
            | AttributeTargets.Module)]
    public class InsightsAttribute : Attribute//, IMethodDecorator
    {
        private string _methodName;

        public void Init(object instance, MethodBase method, object[] args)
        {
            _methodName = method.DeclaringType.FullName + "." + method.Name;
        }

        public void OnEntry()
        {
            var message = string.Format("OnEntry: {0}", _methodName);
            Insights.Track(message);
        }

        public void OnExit()
        {
            var message = string.Format("OnExit: {0}", _methodName);
            Insights.Track(message);
        }

        public void OnException(Exception exception)
        {
            Insights.Report(exception);
        }
    }
}
