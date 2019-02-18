using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public static class FileHelper
    {
        public static string GetUserWorkspacerPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "workspacer");
        }

        public static void EnsureUserWorkspacerPathExists()
        {
            var path = GetUserWorkspacerPath();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
