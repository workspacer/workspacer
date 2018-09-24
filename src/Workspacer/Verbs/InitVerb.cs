using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workspacer.ConfigLoader;

namespace Workspacer.Verbs
{
    [Verb("init", HelpText = "create a default configuration file in your user folder called Workspacer.config")]
    public class InitVerbOptions
    {

    }

    public class InitVerb : VerbBase
    {
        public override int Execute()
        {
            if (File.Exists(ConfigHelper.GetConfigPath()))
            {
                DisplayMessage("Workspacer.config already exists, cannot create");
                return 1;
            }

            File.WriteAllText(ConfigHelper.GetConfigPath(), ConfigHelper.GetConfigTemplate());

            DisplayMessage("Workspacer.config created in user folder");

            return 0;
        }
    }
}
