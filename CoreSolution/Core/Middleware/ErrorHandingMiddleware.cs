using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Middleware
{
    public class ErrorHandingMiddleware
    {
        RequestDelegate next;

        public ErrorHandingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, DTO.MSG ms)
        {
            await next(context);

            switch (ms.Code)
            {
                case (int)GValues.GValues.MSGCodes.ERROR:
                    break;
            }
        }
    }
}
