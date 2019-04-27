using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace lec0Project.Models
{
    public class MyLogActionFilter: ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            log("OnActionExecuting", filterContext.RouteData);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            log("OnActionExecuted", filterContext.RouteData);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            log("OnResultExecuting", filterContext.RouteData);
        }


        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            log("OnResultExecuted", filterContext.RouteData);
        }

        public void log(string methodName, RouteData routeData)
        {
            // save url (controller, action)

            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];

            var message = $"{methodName} Controller Name: {controllerName} Action Name: {actionName}";


            Debug.WriteLine(message, "Action Filter Log");
        }
    }
}