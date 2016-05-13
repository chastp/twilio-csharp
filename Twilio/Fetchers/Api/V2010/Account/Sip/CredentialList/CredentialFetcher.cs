using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Exceptions;
using Twilio.Fetchers;
using Twilio.Http;
using Twilio.Resources.Api.V2010.Account.Sip.CredentialList;

namespace Twilio.Fetchers.Api.V2010.Account.Sip.CredentialList {

    public class CredentialFetcher : Fetcher<CredentialResource> {
        private string accountSid;
        private string credentialListSid;
        private string sid;
    
        /**
         * Construct a new CredentialFetcher
         * 
         * @param accountSid The account_sid
         * @param credentialListSid The credential_list_sid
         * @param sid The sid
         */
        public CredentialFetcher(string accountSid, string credentialListSid, string sid) {
            this.accountSid = accountSid;
            this.credentialListSid = credentialListSid;
            this.sid = sid;
        }
    
        /**
         * Make the request to the Twilio API to perform the fetch
         * 
         * @param client ITwilioRestClient with which to make the request
         * @return Fetched CredentialResource
         */
        public override async Task<CredentialResource> ExecuteAsync(ITwilioRestClient client) {
            Request request = new Request(
                System.Net.Http.HttpMethod.Get,
                Domains.API,
                "/2010-04-01/Accounts/" + this.accountSid + "/SIP/CredentialLists/" + this.credentialListSid + "/Credentials/" + this.sid + ".json"
            );
            
            Response response = await client.Request(request);
            
            if (response == null) {
                throw new ApiConnectionException("CredentialResource fetch failed: Unable to connect to server");
            } else if (response.GetStatusCode() != HttpStatus.HTTP_STATUS_CODE_OK) {
                RestException restException = RestException.FromJson(response.GetContent());
                if (restException == null)
                    throw new ApiException("Server Error, no content");
                throw new ApiException(
                    restException.GetMessage(),
                    restException.GetCode(),
                    restException.GetMoreInfo(),
                    restException.GetStatus(),
                    null
                );
            }
            
            return CredentialResource.FromJson(response.GetContent());
        }
    }
}