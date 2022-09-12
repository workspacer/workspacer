using System;
using System.IO;

namespace workspacer
{
    public static class FileHelper
    {
        public static string GetUserWorkspacerPath()
        {
            #if DEBUG
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".workspacer_debug");
            #else
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".workspacer");
            #endif
        }

        public static void EnsureUserWorkspacerPathExists()
        {
            var path = GetUserWorkspacerPath();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
