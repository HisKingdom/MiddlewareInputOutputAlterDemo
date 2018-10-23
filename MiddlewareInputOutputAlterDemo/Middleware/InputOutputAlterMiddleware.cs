using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public class InputOutputAlterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public InputOutputAlterMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<InputOutputAlterMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            //判断是POST提交过来的
            if (method.Equals("POST"))
            {
                var requestMessage = context.Request.Form["RequestMessage"];
                _logger.LogInformation("requestMessage:" + requestMessage);

                var alterValue =$"{requestMessage}被我修改啦！";
                var dic = new Dictionary<string, StringValues>
                {
                    { "value", new StringValues(alterValue) }
                };
                //修改提交过来的值
                context.Request.Form = new FormCollection(dic);
                using (var ms = new MemoryStream())
                {
                    var orgBodyStream = context.Response.Body;
                    context.Response.Body = ms;
                    context.Response.ContentType = "multipart/form-data";
                    await _next(context);

                    using (var sr = new StreamReader(ms))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        //得到Action的返回值
                        var responseJsonResult = sr.ReadToEnd();
                        ms.Seek(0, SeekOrigin.Begin);
                        //如下代码若不注释则会显示Action的返回值 这里做了注释 则清空Action传过来的值  
                        //  await ms.CopyToAsync(orgBodyStream);
                        var alterResult = $"没事返回值【{responseJsonResult}】被我改过来啦！";

                        context.Response.Body = orgBodyStream;
                        //显示修改后的数据 
                        await context.Response.WriteAsync(alterResult, Encoding.UTF8);


                    }
                }

            }
            else
            {
                await _next(context);
            }

        }
    }
}
