
using CFW.Core.Models;

namespace CFW.OData.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ODataPolicyAttribute : Attribute
    {
        public TenantType TenantType { get; set; }

        public GrantEnum[] Grants { get; set; } = Array.Empty<GrantEnum>();
    }
}
