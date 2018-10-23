using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class InputOutputAlterMiddlewareExtensions
    {
        public static IApplicationBuilder UseInputOutputAlter(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InputOutputAlterMiddleware>();
        }

    }
}
