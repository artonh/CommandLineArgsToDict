using Miracle.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Configuration;

namespace CommandLineArgsToDict
{
    public class Config
    {
        public static Dictionary<string, string> args = new Dictionary<string, string>();

        public static string Key1 => ReadKey("Key1");
        public static string Key2 => ReadKey("Key2", false);

        public static void Init(string[] argList)
        {
            try
            {
                if (argList != null && argList.Any())
                {
                    var parsedArguments = argList.ParseCommandLine<SetupArgs>();
                    if (parsedArguments != null)
                    {
                        parsedArguments.Key1 = ValidateConfigs(parsedArguments.Key1, "value1,value2"); //simple validation when we know the pattern!

                        args = parsedArguments.GetType()
                                 .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(parsedArguments, null));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private static string ReadKey(string key, bool isRequired = true)
        {
            if (args.ContainsKey(key))
            {
                return args[key];
            }
            else
            {
                string value = string.Empty;

                if (!isRequired)
                {
                    if (ConfigurationManager.AppSettings.AllKeys.Any(x => x.Equals(key)))
                    {
                        value = ConfigurationManager.AppSettings.Get(key);
                        args.Add(key, value);
                        return value;
                    }

                    return value;
                }

                value = ConfigurationManager.AppSettings.Get(key);
                if (!string.IsNullOrEmpty(value))
                {
                    args.Add(key, value);
                }
                return value;
            }
        }

        /// <summary>
        /// if the current value it's empty, that is the result!
        /// </summary>
        public static string ValidateConfigs(string currentValue, string allOptions)
        {
            if (string.IsNullOrEmpty(currentValue))
                return string.Empty;

            List<string> result = new List<string>();
            foreach (var item in currentValue.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)))
            {
                if (!allOptions.ToLower().Contains(item.ToLower()))
                    Console.WriteLine($"ValidateConfigs error: not a valid value:{item} based on the all options: {allOptions}!");
                else
                    result.Add(item);
            }

            return string.Join(",", result);
        }
    }


    [ArgumentDescription("Set up #CLI App# through the command line arguments")]
    public class SetupArgs
    {
        [ArgumentName("Key1", "k1")]
        [ArgumentRequired]
        //[ArgumentDescription("Key1 desc")]
        public string Key1 { get; set; }

        [ArgumentName("Key2", "k2")]
        public string Key2 { get; set; }


        //for testing purposes I took two properties named as Keys!
    }

}
