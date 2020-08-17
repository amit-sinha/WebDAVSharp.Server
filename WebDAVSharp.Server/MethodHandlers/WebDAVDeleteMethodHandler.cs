using System.Collections;
using System.Collections.Generic;
using System.Web;
using WebDAVSharp.Server.Adapters;
using WebDAVSharp.Server.Stores;

namespace WebDAVSharp.Server.MethodHandlers
{
    /// <summary>
    /// This class implements the <c>DELETE</c> HTTP method for WebDAV#.
    /// </summary>
    internal class WebDavDeleteMethodHandler : WebDavMethodHandlerBase, IWebDavMethodHandler
    {
        /// <summary>
        /// Gets the collection of the names of the HTTP methods handled by this instance.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public IEnumerable<string> Names
        {
            get
            {
                return new[]
                {
                    "DELETE"
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="store"></param>
        /// <param name="prefixes"></param>
        public void ProcessRequest(HttpRequest request, HttpResponse response, IWebDavStore store, IList<string> prefixes)
        {
            // Get the parent collection of the item
            IWebDavStoreCollection collection = GetParentCollection(prefixes, store, request.Url);

            // Get the item from the collection
            IWebDavStoreItem item = GetItemFromCollection(collection, request.Url);

            // Deletes the item
            collection.Delete(item);
            response.SendSimpleResponse();
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="prefixes">The <see cref="IList" /> through which the request came in from the client.</param>
        /// <param name="context">The
        /// <see cref="IHttpListenerContext" /> object containing both the request and response
        /// objects to use.</param>
        /// <param name="store">The <see cref="IWebDavStore" /> that the <see cref="WebDavServer" /> is hosting.</param>
        public void ProcessRequest(IHttpListenerContext context, IWebDavStore store, IList<string> prefixes)
        {
            // Get the parent collection of the item
            IWebDavStoreCollection collection = GetParentCollection(prefixes, store, context.Request.Url);

            // Get the item from the collection
            IWebDavStoreItem item = GetItemFromCollection(collection, context.Request.Url);

            // Deletes the item
            collection.Delete(item);
            context.Response.SendSimpleResponse();
        }
    }
}