using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing.LinkGeneration
{
    public class RequestContext
    {
        public HttpContext HttpContext { get; set; }

        public RouteValueDictionary AmbientValues { get; set; }
    }
}
