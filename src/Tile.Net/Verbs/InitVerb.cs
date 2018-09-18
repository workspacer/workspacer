using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tile.Net.ConfigLoader;

namespace Tile.Net.Verbs
{
    [Verb("init", HelpText = "create a default configuration file in your user folder called Tile.Net.config")]
    public class InitVerbOptions
    {

    }

    public class InitVerb : VerbBase
    {
        public override int Execute()
        {
            if (File.Exists(ConfigHelper.GetConfigPath()))
            {
                DisplayMessage("Tile.Net.config already exists, cannot create");
                return 1;
            }

            File.WriteAllText(ConfigHelper.GetConfigPath(), ConfigHelper.GetConfigTemplate());

            DisplayMessage("Tile.Net.config created in user folder");

            return 0;
        }
    }
}
