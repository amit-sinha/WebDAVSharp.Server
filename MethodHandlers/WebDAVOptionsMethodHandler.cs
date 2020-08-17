using System.Collections.Generic;
using System.Web;
using WebDAVSharp.Server.Adapters;
using WebDAVSharp.Server.Stores;

namespace WebDAVSharp.Server.MethodHandlers
{
    /// <summary>
    /// This class implements the <c>OPTIONS</c> HTTP method for WebDAV#.
    /// </summary>
    internal class WebDavOptionsMethodHandler : WebDavMethodHandlerBase, IWebDavMethodHandler
    {
        /// <summary>
        /// Gets the collection of the names of the HTTP methods handled by this instance.
        /// </summary>
        public IEnumerable<string> Names
        {
            get
            {
                return new[]
                {
                    "OPTIONS"
                };
            }
        }

        public void ProcessRequest(HttpRequest request, HttpResponse response, IWebDavStore store, IList<string> prefixes)
        {
            List<string> verbsAllowed = new List<string> { "OPTIONS", "TRACE", "GET", "HEAD", "POST", "COPY", "PROPFIND", "LOCK", "UNLOCK" };

            List<string> verbsPublic = new List<string> { "OPTIONS", "GET", "HEAD", "PROPFIND", "PROPPATCH", "MKCOL", "PUT", "DELETE", "COPY", "MOVE", "LOCK", "UNLOCK" };

            foreach (string verb in verbsAllowed)
                response.AppendHeader("Allow", verb);

            foreach (string verb in verbsPublic)
                response.AppendHeader("Public", verb);

            // Sends 200 OK
            response.SendSimpleResponse();
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="prefixes">The <see cref="WebDavServer" /> through which the request came in from the client.</param>
        /// <param name="context">The 
        /// <see cref="IHttpListenerContext" /> object containing both the request and response
        /// objects to use.</param>
        /// <param name="store">The <see cref="IWebDavStore" /> that the <see cref="WebDavServer" /> is hosting.</param>
        public void ProcessRequest(IHttpListenerContext context, IWebDavStore store, IList<string> prefixes)
        {
            List<string> verbsAllowed = new List<string> { "OPTIONS", "TRACE", "GET", "HEAD", "POST", "COPY", "PROPFIND", "LOCK", "UNLOCK" };

            List<string> verbsPublic = new List<string> { "OPTIONS", "GET", "HEAD", "PROPFIND", "PROPPATCH", "MKCOL", "PUT", "DELETE", "COPY", "MOVE", "LOCK", "UNLOCK" };

            foreach (string verb in verbsAllowed)
                context.Response.AppendHeader("Allow", verb);

            foreach (string verb in verbsPublic)
                context.Response.AppendHeader("Public", verb);

            // Sends 200 OK
            context.Response.SendSimpleResponse();
        }
    }
}