namespace workspacer.FSharpConfig
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class Extensions
    {

        public static Assembly LoadFile(this AppDomain instance, string dllFile)
        {
            var buffer = File.ReadAllBytes(dllFile);
            return instance.Load(buffer);
        }

        public static AppDomain AddFileResolver(this AppDomain instance, IEnumerable<string> dllFiles)
        {
            var references = dllFiles.ToDictionary(x => AssemblyName.GetAssemblyName(x).FullName);

            Assembly Resolver(object sender, ResolveEventArgs args)
            {
                AppDomain domain = (AppDomain) sender!;

                if (references.TryGetValue(args.Name, out var fileName))
                {
                    return domain.LoadFile(fileName);
                }

                return null;
            }

            instance.AssemblyResolve += Resolver;
            return instance;
        }
        
        public static AppDomain AddNoVersionResolver(this AppDomain instance)
        {
            Assembly Resolver(object sender, ResolveEventArgs args)
            {
                AppDomain domain = (AppDomain) sender!;

                var aname = new AssemblyName(args.Name).Name;

                var test = instance.GetAssemblies().FirstOrDefault(x => x.GetName().Name == aname);

                return test;
            }

            instance.AssemblyResolve += Resolver;
            return instance;
        }

        public static T GetDelegate<T>(this Assembly a, string t, string name)
            where T : Delegate
        {
            foreach (var tp in a.GetTypes())
            {
                Console.WriteLine(t);
            }
            
            var method = a.GetType(t)!.GetMethod(name);
            return method is null ? null : Delegate.CreateDelegate(typeof(T), method) as T;
        }
    }
}
