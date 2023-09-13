using System.Net.Http;

namespace Leads.IntegrationTests.Common
{
    public class CallWrapper<TResult>
    {
        public HttpResponseMessage Response { get; set; }
        
        public HttpRequestMessage Request { get; set; }
        
        public TResult Result { get; set; }
    }
}