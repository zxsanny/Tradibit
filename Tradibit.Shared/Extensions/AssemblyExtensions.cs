using System.Reflection;

namespace Tradibit.Shared.Extensions;

/// <summary> </summary>
public static class AssemblyExt
{
    /// <summary>
    /// Returns all assemblies which name are started with same prefix (word before .) as executing assembly
    /// e.g. if current assembly has name Loyalty.MembersApi, then it will return all assemblies started with Loyalty 
    /// </summary>
    public static Assembly[] GetAllOwnReferencedAssemblies()
    {
        var currentAsmName = Assembly.GetEntryAssembly()!.GetName().Name;
        if (!currentAsmName.Contains('.'))
            return new[] { Assembly.GetExecutingAssembly() };
        var prefix = currentAsmName![..currentAsmName.IndexOf('.')];
        return Assembly.GetEntryAssembly().GetAllReferencedAssemblies(prefix).ToArray();
    }

    /// <summary> </summary>
    public static List<Assembly> GetAllReferencedAssemblies(this Assembly currentAssembly, string filter = null)
    {
        var assemblyDict = new Dictionary<string, Assembly>();
        var assembliesToCheck = new Queue<Assembly>();

        assemblyDict.Add(currentAssembly.FullName!, currentAssembly);
        assembliesToCheck.Enqueue(currentAssembly);

        while (assembliesToCheck.Any())
        {
            var assemblyToCheck = assembliesToCheck.Dequeue();

            foreach (var refAssemblyName in assemblyToCheck.GetReferencedAssemblies())
            {
                if (assemblyDict.ContainsKey(refAssemblyName.FullName) ||
                    (!string.IsNullOrEmpty(filter) && !refAssemblyName.FullName.Contains(filter))) 
                    continue;
                    
                var assembly = Assembly.Load(refAssemblyName);
                assembliesToCheck.Enqueue(assembly);
                assemblyDict.Add(refAssemblyName.FullName, assembly);
            }
        }

        return assemblyDict.Select(x => x.Value).ToList();
    }
}