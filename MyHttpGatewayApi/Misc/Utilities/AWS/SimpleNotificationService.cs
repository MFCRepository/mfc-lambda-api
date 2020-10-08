using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHttpGatewayApi.Utilities.AmazonWebServices
{
    public static class SimpleNotificationService
    {
        public static string Send(string _message,
            string _phone)
        {
            /* get rid of anything in the phone number that is not numeric and format it to 11 digits */
            _phone = new String(_phone.Where(c => char.IsDigit(c)).ToArray());

            _phone = (_phone.Length == 10 ? string.Format("1{0}", _phone) : _phone);

            if (_phone.Length != 11) return "Invalid phone number";

            try
            {
                var _credentials = MyHttpGatewayApi.Utilities.AmazonWebServices.SecretsManager.amazon_web_services_credentials.credentials.Find(x => x.service == "SNS");

                var _aws = new Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient(
                        _credentials.access_key_id,
                        _credentials.access_key,
                        Amazon.RegionEndpoint.USEast1
                    );

                _aws.PublishAsync(new Amazon.SimpleNotificationService.Model.PublishRequest() { Message = _message, PhoneNumber = _phone }).Wait();
                
                return "Message sent";

            } catch (Exception _ex)
            {
                return _ex.InnerException.ToString();

            }

        }

    }

}
