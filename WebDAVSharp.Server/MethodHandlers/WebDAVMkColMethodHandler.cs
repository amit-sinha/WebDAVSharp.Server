using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using WebDAVSharp.Server.Adapters;
using WebDAVSharp.Server.Exceptions;
using WebDAVSharp.Server.Stores;

namespace WebDAVSharp.Server.MethodHandlers
{
    /// <summary>
    /// This class implements the <c>MKCOL</c> HTTP method for WebDAV#.
    /// </summary>
    internal class WebDavMkColMethodHandler : WebDavMethodHandlerBase, IWebDavMethodHandler
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
                    "MKCOL"
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
            if (request.ContentLength > 0)
                throw new WebDavUnsupportedMediaTypeException();

            IWebDavStoreCollection collection = GetParentCollection(prefixes, store, request.Url);

            string collectionName = Uri.UnescapeDataString(
                request.Url.Segments.Last().TrimEnd('/', '\\')
                );
            if (collection.GetItemByName(collectionName) != null)
                throw new WebDavMethodNotAllowedException();

            collection.CreateCollection(collectionName);

            response.SendSimpleResponse(HttpStatusCode.Created);
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="prefixes">The <see cref="WebDavServer" /> through which the request came in from the client.</param>
        /// <param name="context">The 
        /// <see cref="IHttpListenerContext" /> object containing both the request and response
        /// objects to use.</param>
        /// <param name="store">The <see cref="IWebDavStore" /> that the <see cref="WebDavServer" /> is hosting.</param>
        /// <exception cref="WebDAVSharp.Server.Exceptions.WebDavUnsupportedMediaTypeException"></exception>
        /// <exception cref="WebDAVSharp.Server.Exceptions.WebDavMethodNotAllowedException"></exception>
        public void ProcessRequest(IHttpListenerContext context, IWebDavStore store, IList<string> prefixes)
        {
            if (context.Request.ContentLength64 > 0)
                throw new WebDavUnsupportedMediaTypeException();

            IWebDavStoreCollection collection = GetParentCollection(prefixes, store, context.Request.Url);
                
            string collectionName = Uri.UnescapeDataString(
                context.Request.Url.Segments.Last().TrimEnd('/', '\\')
                );
            if (collection.GetItemByName(collectionName) != null)
                throw new WebDavMethodNotAllowedException();

            collection.CreateCollection(collectionName);

            context.Response.SendSimpleResponse(HttpStatusCode.Created);
        }
    }
}