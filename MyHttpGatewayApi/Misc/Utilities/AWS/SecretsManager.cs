using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.IO;

namespace MyHttpGatewayApi.Utilities.AmazonWebServices
{
    public static class SecretsManager
    {
        private struct SecretsManager_Keys
        {
            public string AWS_Key { get => "aws/services/aws-service-credentials"; }
            public string Outlook365_Key { get => "outlook/accounts/outlook365-credentials"; }
            public string Database_Key { get => "database/api/api-database-credentials"; }
        }

        public class Credentials
        {
            public string username { get; set; }
            public string password { get; set; }

        }
        public class Database
        {
            public class Database_Credentials: Credentials
            {
                public List<string> permissions { get; set; }
            }

            public List<Database_Credentials> credentials = new List<Database_Credentials>();
        }

        public class AWS
        {
            public class AWS_Credentials
            {
                public string access_key { get; set; }
                public string access_key_id { get; set; }
                public string service { get; set; }
                public List<string> permissions { get; set; }
            }

            public List<AWS_Credentials> credentials = new List<AWS_Credentials>();
        }

        public class Outlook
        {
            public class OutlookCredentials : Credentials
            {
                public string name { get; set; }
                public string address {get;set ;}
                
            }

            public class Root
            {
                public List<OutlookCredentials> send_credentials = new List<OutlookCredentials>();
            }

            public Root credentials = new Root();
        }

        public static Database database_credentials = new Database();
        public static Outlook outlook_credentials = new Outlook();
        public static AWS amazon_web_services_credentials = new AWS();
        private static readonly SecretsManager_Keys Keys;

        static SecretsManager()
        {
            /* initialize the secrets */

            database_credentials = Newtonsoft.Json.JsonConvert.DeserializeObject<Database>(GetSecret(Keys.Database_Key));
            outlook_credentials = Newtonsoft.Json.JsonConvert.DeserializeObject<Outlook>(GetSecret(Keys.Outlook365_Key));
            amazon_web_services_credentials = Newtonsoft.Json.JsonConvert.DeserializeObject<AWS>(GetSecret(Keys.AWS_Key));

        }
       
        public static string GetSecret(string secretName)
        {
            string region = "us-east-2";
            string secret = "";

            MemoryStream memoryStream = new MemoryStream();

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest();
            request.SecretId = secretName;
            request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

            GetSecretValueResponse response = null;

            // In this sample we only handle the specific exceptions for the 'GetSecretValue' API.
            // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            // We rethrow the exception by default.

            try
            {
                response = client.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException e)
            {
                // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                // Deal with the exception here, and/or rethrow at your discretion.
                //throw;
            }
            catch (InternalServiceErrorException e)
            {
                // An error occurred on the server side.
                // Deal with the exception here, and/or rethrow at your discretion.
                //throw;
            }
            catch (InvalidParameterException e)
            {
                // You provided an invalid value for a parameter.
                // Deal with the exception here, and/or rethrow at your discretion
                //throw;
            }
            catch (InvalidRequestException e)
            {
                // You provided a parameter value that is not valid for the current state of the resource.
                // Deal with the exception here, and/or rethrow at your discretion.
                //throw;
            }
            catch (ResourceNotFoundException e)
            {
                // We can't find the resource that you asked for.
                // Deal with the exception here, and/or rethrow at your discretion.
                //throw;
            }
            catch (System.AggregateException ae)
            {
                // More than one of the above exceptions were triggered.
                // Deal with the exception here, and/or rethrow at your discretion.
                //throw;
            }

            // Decrypts secret using the associated KMS CMK.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                secret = response.SecretString;
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            // Your code goes here.
            return secret;
        }
    }
}
