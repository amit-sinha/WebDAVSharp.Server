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
    /// This class implements the <c>COPY</c> HTTP method for WebDAV#.
    /// </summary>
    internal class WebDavCopyMethodHandler : WebDavMethodHandlerBase, IWebDavMethodHandler
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
                    "COPY"
                };
            }
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="prefixes">The <see cref="HttpListenerPrefixCollection" /> through which the request came in from the client.</param>
        /// <param name="context">The 
        /// <see cref="HttpContext" /> object containing both the request and response
        /// objects to use.</param>
        /// <param name="store">The <see cref="IWebDavStore" /> that the <see cref="WebDavServer" /> is hosting.</param>
        /// <exception cref="WebDAVSharp.Server.Exceptions.WebDavMethodNotAllowedException"></exception>
        public void ProcessRequest(IHttpListenerContext context, IWebDavStore store, IList<string> prefixes)
        {            
            IWebDavStoreItem source = context.Request.Url.GetItem(prefixes, store);
            if (source is IWebDavStoreDocument || source is IWebDavStoreCollection)
                CopyItem(context.Request, context.Response, store, source, prefixes);
            else
                throw new WebDavMethodNotAllowedException();
        }

        public void ProcessRequest(HttpRequest request, HttpResponse response, IWebDavStore store, IList<string> prefixes)
        {
            IWebDavStoreItem source = request.Url.GetItem(prefixes, store);
            if (source is IWebDavStoreDocument || source is IWebDavStoreCollection)
                CopyItem(prefixes, request, response, store, source);
            else
                throw new WebDavMethodNotAllowedException();
        }

        /// <summary>
        /// Copies the item.
        /// </summary>
        /// <param name="prefixes">The server.</param>
        /// <param name="request">The context.</param>
        /// <param name="response">The context.</param>
        /// <param name="store">The store.</param>
        /// <param name="source">The source.</param>
        /// <exception cref="WebDAVSharp.Server.Exceptions.WebDavForbiddenException"></exception>
        /// <exception cref="WebDAVSharp.Server.Exceptions.WebDavPreconditionFailedException"></exception>
        private static void CopyItem(IList<string> prefixes, HttpRequest request, HttpResponse response, IWebDavStore store,
            IWebDavStoreItem source)
        {
            Uri destinationUri = GetDestinationHeader(request.Headers);
            IWebDavStoreCollection destinationParentCollection = GetParentCollection(prefixes, store, destinationUri);

            bool copyContent = (GetDepthHeader(request.Headers) != 0);
            bool isNew = true;

            string destinationName = Uri.UnescapeDataString(destinationUri.Segments.Last().TrimEnd('/', '\\'));
            IWebDavStoreItem destination = destinationParentCollection.GetItemByName(destinationName);
            
            if (destination != null)
            {
                if (source.ItemPath == destination.ItemPath)
                    throw new WebDavForbiddenException();
                if (!GetOverwriteHeader(request.Headers))
                    throw new WebDavPreconditionFailedException();
                if (destination is IWebDavStoreCollection)
                    destinationParentCollection.Delete(destination);
                isNew = false;
            }

            destinationParentCollection.CopyItemHere(source, destinationName, copyContent);

            response.SendSimpleResponse(isNew ? HttpStatusCode.Created : HttpStatusCode.NoContent);
        }

        private static void CopyItem(IHttpListenerRequest request, IHttpListenerResponse response, IWebDavStore store, IWebDavStoreItem source, IList<string> prefixes)
        {
            Uri destinationUri = GetDestinationHeader(request.Headers);
            IWebDavStoreCollection destinationParentCollection = GetParentCollection(prefixes, store, destinationUri);

            bool copyContent = (GetDepthHeader(request.Headers) != 0);
            bool isNew = true;

            string destinationName = Uri.UnescapeDataString(destinationUri.Segments.Last().TrimEnd('/', '\\'));
            IWebDavStoreItem destination = destinationParentCollection.GetItemByName(destinationName);

            if (destination != null)
            {
                if (source.ItemPath == destination.ItemPath)
                    throw new WebDavForbiddenException();
                if (!GetOverwriteHeader(request.Headers))
                    throw new WebDavPreconditionFailedException();
                if (destination is IWebDavStoreCollection)
                    destinationParentCollection.Delete(destination);
                isNew = false;
            }

            destinationParentCollection.CopyItemHere(source, destinationName, copyContent);

            response.SendSimpleResponse(isNew ? HttpStatusCode.Created : HttpStatusCode.NoContent);
        }
    }
}