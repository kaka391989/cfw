using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.OData.Extensions;

namespace CFW.OData.Extensions
{
    public static class UtilsExtensions
    {
        public static string GetHttpMethodName(this ActionModel actionModel)
            => actionModel.GetAttribute<HttpMethodAttribute>()?.HttpMethods?.FirstOrDefault() ?? HttpMethod.Get.Method;
    }
}
