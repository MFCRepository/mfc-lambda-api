using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyHttpGatewayApi.Misc.Globals
{
    public static class Application
    {
        public static Credentials Credentials
        {
            get
            {
                string _credentialsPath = string.Format("{0}/Resources/string-resources/.gitignore/credentials.json", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

                using (StreamReader _sr = new StreamReader(_credentialsPath))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Credentials>(_sr.ReadToEnd());
                }

            }

        }

    }

    public static class Environment_Variables
    {
        public static string Environment
        {
            get;set;
        }

    }
}

namespace MyHttpGatewayApi.Misc
{
    
    public class Aws
    {
        public string aws_access_key_id { get; set; }
        public string aws_secret_key { get; set; }
        public string aws_sns_access_key { get; set; }
        public string aws_sns_access_key_id { get; set; }
    }

    public class Outlook365
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Database
    {
        public List<string> privileges { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Credentials
    {
        public Aws aws { get; set; }
        public Outlook365 outlook_365 { get; set; }
        public List<Database> database { get; set; }
    }
}
