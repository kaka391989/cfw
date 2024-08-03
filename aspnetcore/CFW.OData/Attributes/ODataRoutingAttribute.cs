
namespace CFW.OData.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ODataRoutingAttribute : Attribute
    {
        public string Name { get; set; } = string.Empty;

        public string Action { set; get; } = string.Empty;
    }
}
