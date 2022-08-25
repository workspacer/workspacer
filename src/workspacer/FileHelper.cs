using System;
using System.IO;

namespace workspacer
{
    public static class FileHelper
    {
        public static string GetUserWorkspacerPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".workspacer");
        }

        public static void EnsureUserWorkspacerPathExists()
        {
            var path = GetUserWorkspacerPath();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
