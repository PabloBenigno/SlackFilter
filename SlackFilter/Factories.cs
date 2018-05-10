using System;
using Spin.Runtime;

namespace SlackFilter
{
    internal static class Factories
    {
        internal static ISpinExecutionContext ExecutionContextFactory(IServiceProvider serviceProvider)
        {
            return new SpinExecutionContext()
            {
                ServiceProvider = serviceProvider
            };
        }
    }
}