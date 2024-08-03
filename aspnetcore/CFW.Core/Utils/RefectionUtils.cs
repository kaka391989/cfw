using System.ComponentModel;
using System.Reflection;

namespace CFW.Core.Utils
{
    public static class RefectionUtils
    {
        public static Dictionary<string, string> GetDisplayNames<T>()
        {
            var result = new Dictionary<string, string>();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var displayNameAttribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                                   .FirstOrDefault() as DisplayNameAttribute;

                var displayName = displayNameAttribute != null ? displayNameAttribute.DisplayName : property.Name;

                result.Add(property.Name, displayName);
            }

            return result;
        }
    }
}
