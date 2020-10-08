using System.Collections;
using System.Collections.Generic;

namespace aws.configuration
{
    public class Credentials
    {
        public string aws_access_key_id { get; set; }
        public string aws_secret_key { get; set; }
    }

    public class Domain
    {
        public string domain_name { get; set; }
        public string certificate_arn { get; set; }
        public string hosted_zone_id { get; set; }
    }

    public class Vpc_Configs
    {
        public string vpc_id { get; set; }
        public List<string> security_group_ids { get; set; }
        public List<string> subnet_ids { get; set; }
    }

    public class Environment
    {
        public string name { get; set; }
        public string region { get; set; }
        public string profile { get; set; }
        public Domain domain { get; set; }
        public string stack_name { get; set; }
        public string function_name { get; set; }
        public List<string> stages { get; set; }
        public Vpc_Configs vpc_configs { get; set; }
    }

    public class Profile
    {
        public string profile_name { get; set; }
        public Credentials credentials { get; set; }
        public List<Environment> environments { get; set; }
    }

    public class Configs
    {
        public List<Profile> profiles { get; set; }
    }

}




