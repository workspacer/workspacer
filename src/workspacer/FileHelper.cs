using System;
using System.IO;

namespace workspacer
{
    public static class FileHelper
    {
        public static string GetConfigDirectory()
        {
            // Change  EnvironmentVariableTarget.User to another Enum to use System variable
            var userFolder = Environment.GetEnvironmentVariable("WORKSPACER_CONFIG",EnvironmentVariableTarget.User);
            // Using a UserEnviromentVariable to set the workspacer config folder
            if (userFolder == null){
              userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            // Use a migrated configuraton folder first.
            var newConfigDirectory = Path.Combine(userFolder, ".config", "workspacer");
            if (Directory.Exists(newConfigDirectory))
                return newConfigDirectory;

            // Fall back to old configuration folder.
            var oldConfigDirectory = Path.Combine(userFolder, ".workspacer");
            if (Directory.Exists(oldConfigDirectory))
                return oldConfigDirectory;

            // Default to new configuration folder.
            return newConfigDirectory;
        }

        public static void EnsureConfigDirectoryExists()
        {
            var path = GetConfigDirectory();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
