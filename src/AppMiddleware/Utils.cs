using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("acm.AppMiddleware.Tests")]
namespace Aspose.Cloud.Marketplace.App.Middleware
{
    internal static class Utils
    {
        public static string controllerKey = "controller";
        public static string actionKey = "action";
        public static string pageKey = "page";
        /// <summary>
        /// Format request parameters (query string params, form params, controller/action, razor page
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ValueTuple<string, string, Dictionary<string, string>> GetRequestParameters(HttpContext context)
        {
            string[] predefinedRouteData = { controllerKey, actionKey };
            Dictionary<string, string> parameters = null;
            if (null != context?.Request?.QueryString && context.Request.QueryString.HasValue)
            {
                parameters = context.Request.Query.ToDictionary(q => q.Key, q => string.Join(";", q.Value));
            }
            if (true == context?.Request?.HasFormContentType && context?.Request?.Form?.Count > 0)
            {
                var formParams = context.Request.Form?.ToDictionary(f => f.Key, f => string.Join(";", f.Value));
                // merge with parameters dict
                parameters = null == parameters ? formParams : parameters.Union(formParams.Where(f => !parameters.ContainsKey(f.Key))).ToDictionary(p => p.Key, p => p.Value);
            }

            var routeData = context?.GetRouteData();

            var pathParams = routeData?.Values?.Where(p => !predefinedRouteData.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value?.ToString());
            if (null != pathParams && pathParams.Count > 0)
            {
                parameters = null == parameters ? pathParams : parameters.Union(pathParams.Where(f => !parameters.ContainsKey(f.Key))).ToDictionary(p => p.Key, p => p.Value);
            }
            object controller = null;
            object action = null;
            routeData?.Values?.TryGetValue(controllerKey, out controller);
            routeData?.Values?.TryGetValue(actionKey, out action);
            if (null == action)
            {
                // try to get route data for Razor page [page] = <page path>
                routeData?.Values?.TryGetValue(pageKey, out action);
                if (null != action)
                    controller = pageKey;
            }
            return (controller?.ToString(), action?.ToString(), parameters);
        }
    }
}
