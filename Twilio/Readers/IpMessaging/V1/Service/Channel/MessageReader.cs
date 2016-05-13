using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Exceptions;
using Twilio.Http;
using Twilio.Readers;
using Twilio.Resources;
using Twilio.Resources.IpMessaging.V1.Service.Channel;

namespace Twilio.Readers.IpMessaging.V1.Service.Channel {

    public class MessageReader : Reader<MessageResource> {
        private string serviceSid;
        private string channelSid;
    
        /**
         * Construct a new MessageReader
         * 
         * @param serviceSid The service_sid
         * @param channelSid The channel_sid
         */
        public MessageReader(string serviceSid, string channelSid) {
            this.serviceSid = serviceSid;
            this.channelSid = channelSid;
        }
    
        /**
         * Make the request to the Twilio API to perform the read
         * 
         * @param client ITwilioRestClient with which to make the request
         * @return MessageResource ResourceSet
         */
        public override async Task<ResourceSet<MessageResource>> ExecuteAsync(ITwilioRestClient client) {
            Request request = new Request(
                System.Net.Http.HttpMethod.Get,
                Domains.IPMESSAGING,
                "/v1/Services/" + this.serviceSid + "/Channels/" + this.channelSid + "/Messages"
            );
            
            AddQueryParams(request);
            
            Page<MessageResource> page = await PageForRequest(client, request);
            
            return new ResourceSet<MessageResource>(this, client, page);
        }
    
        /**
         * Retrieve the next page from the Twilio API
         * 
         * @param nextPageUri URI from which to retrieve the next page
         * @param client ITwilioRestClient with which to make the request
         * @return Next Page
         */
        public override Page<MessageResource> NextPage(string nextPageUri, ITwilioRestClient client) {
            Request request = new Request(
                System.Net.Http.HttpMethod.Get,
                nextPageUri
            );
            
            var task = PageForRequest(client, request);
            task.Wait();
            
            return task.Result;
        }
    
        /**
         * Generate a Page of MessageResource Resources for a given request
         * 
         * @param client ITwilioRestClient with which to make the request
         * @param request Request to generate a page for
         * @return Page for the Request
         */
        protected async Task<Page<MessageResource>> PageForRequest(ITwilioRestClient client, Request request) {
            Response response = await client.Request(request);
            
            if (response == null) {
                throw new ApiConnectionException("MessageResource read failed: Unable to connect to server");
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
            
            Page<MessageResource> result = new Page<MessageResource>();
            result.deserialize("messages", response.GetContent());
            
            return result;
        }
    
        /**
         * Add the requested query string arguments to the Request
         * 
         * @param request Request to add query string arguments to
         */
        private void AddQueryParams(Request request) {
            request.AddQueryParam("PageSize", GetPageSize().ToString());
        }
    }
}