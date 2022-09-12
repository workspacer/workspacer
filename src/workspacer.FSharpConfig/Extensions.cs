namespace workspacer.FSharpConfig
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class Extensions
    {
        public static Assembly LoadFile(this AppDomain appDomain, string filename) {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int) fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            return appDomain.Load(buffer);
        }

        public static AppDomain FileResolver(this AppDomain resoleAppDomain, IEnumerable<string> dllFiles)
        {
            var references = dllFiles.ToDictionary(x => AssemblyName.GetAssemblyName(x).FullName);

            Assembly Resolver(object sender, ResolveEventArgs args)
            {
                AppDomain domain = (AppDomain) sender!;

                var file = references[args.Name];
                return LoadFile(domain, file);
            }

            resoleAppDomain.AssemblyResolve += Resolver;
            return resoleAppDomain;
        }

        public static T GetDelegate<T>(this Type t, string name)
            where T : Delegate
        {
            var method = t.GetMethod(name);
            return (T)Delegate.CreateDelegate(typeof(T), method);
        }

        public static T GetDelegate<T>(this Assembly a, string t, string name)
            where T : Delegate
        {
            return a.GetType(t)!.GetDelegate<T>(name);
        }
    }
}
