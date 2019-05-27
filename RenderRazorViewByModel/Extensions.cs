using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RenderRazorViewByModel
{
    public static class Extensions
    {
        public static async Task<string> GetViewForModel<T>(this IRazorViewToStringRenderer renderer, T model)
        {
            string path = Path.Combine(Path.GetFullPath("."), $"{typeof(Extensions).Namespace}.Views.dll");
            var assembly = Assembly.LoadFrom(path);

            var identifier = assembly
                .ExportedTypes
                .Single(q => q.IsSubclassOf(typeof(RazorPage<T>)))
                .GetCustomAttribute<RazorSourceChecksumAttribute>()
                .Identifier;

            var result = await renderer.RenderViewToStringAsync(identifier, model);

            return result;
        }
    }
}
