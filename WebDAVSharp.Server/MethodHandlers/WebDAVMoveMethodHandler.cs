using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using WebDAVSharp.Server.Adapters;
using WebDAVSharp.Server.Exceptions;
using WebDAVSharp.Server.Stores;

namespace WebDAVSharp.Server.MethodHandlers
{
    /// <summary>
    /// This class implements the <c>MOVE</c> HTTP method for WebDAV#.
    /// </summary>
    internal class WebDavMoveMethodHandler : WebDavMethodHandlerBase, IWebDavMethodHandler
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
                    "MOVE"
                };
            }
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="prefixes">The <see cref="WebDavServer" /> through which the request came in from the client.</param>
        /// <param name="request">The </param>>
        /// <param name="response">The 
        /// <see cref="IHttpListenerContext" /> object containing both the request and response
        /// objects to use.</param>
        /// <param name="store">The <see cref="IWebDavStore" /> that the <see cref="WebDavServer" /> is hosting.</param>
        public void ProcessRequest(HttpRequest request, HttpResponse response, IWebDavStore store, IList<string> prefixes)
        {
            var source = request.Url.GetItem(prefixes, store);

            MoveItem(prefixes, request.Headers, response, store, source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="store"></param>
        /// <param name="prefixes"></param>
        public void ProcessRequest(HttpRequest request, IHttpListenerResponse response, IWebDavStore store, IList<string> prefixes)
        {
            var source = request.Url.GetItem(prefixes, store);

            MoveItem(prefixes, request.Headers, response, store, source);
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
            var source = context.Request.Url.GetItem(prefixes, store);

            MoveItem(prefixes, context.Request.Headers, context.Response, store, source);
        }

        /// <summary>
        /// Moves the
        /// </summary>
        /// <param name="prefixes">The <see cref="WebDavServer" /> through which the request came in from the client.</param>
        /// <param name="headers">The 
        /// <see cref="IHttpListenerContext" /> object containing both the request and response
        /// objects to use.</param>
        /// <param name="response"></param>
        /// <param name="store">The <see cref="IWebDavStore" /> that the <see cref="WebDavServer" /> is hosting.</param>
        /// <param name="sourceWebDavStoreItem">The <see cref="IWebDavStoreItem" /> that will be moved</param>
        /// <exception cref="WebDAVSharp.Server.Exceptions.WebDavForbiddenException">If the source path is the same as the destination path</exception>
        /// <exception cref="WebDAVSharp.Server.Exceptions.WebDavPreconditionFailedException">If one of the preconditions failed</exception>
        private void MoveItem(IList<string> prefixes, NameValueCollection headers, IHttpListenerResponse response, IWebDavStore store,
            IWebDavStoreItem sourceWebDavStoreItem)
        {
            Uri destinationUri = GetDestinationHeader(headers);
            IWebDavStoreCollection destinationParentCollection = GetParentCollection(prefixes, store, destinationUri);

            bool isNew = true;

            string destinationName = Uri.UnescapeDataString(destinationUri.Segments.Last().TrimEnd('/', '\\'));
            IWebDavStoreItem destination = destinationParentCollection.GetItemByName(destinationName);
            if (destination != null)
            {
                if (sourceWebDavStoreItem.ItemPath == destination.ItemPath)
                    throw new WebDavForbiddenException();
                // if the overwrite header is F, statuscode = precondition failed 
                if (!GetOverwriteHeader(headers))
                    throw new WebDavPreconditionFailedException();
                // else delete destination and set isNew to false
                destinationParentCollection.Delete(destination);
                isNew = false;
            }

            destinationParentCollection.MoveItemHere(sourceWebDavStoreItem, destinationName);

            // send correct response
            response.SendSimpleResponse(isNew ? HttpStatusCode.Created : HttpStatusCode.NoContent);
        }

        private void MoveItem(IList<string> prefixes, NameValueCollection headers, HttpResponse response, IWebDavStore store,
            IWebDavStoreItem sourceWebDavStoreItem)
        {
            Uri destinationUri = GetDestinationHeader(headers);
            IWebDavStoreCollection destinationParentCollection = GetParentCollection(prefixes, store, destinationUri);

            bool isNew = true;

            string destinationName = Uri.UnescapeDataString(destinationUri.Segments.Last().TrimEnd('/', '\\'));
            IWebDavStoreItem destination = destinationParentCollection.GetItemByName(destinationName);
            if (destination != null)
            {
                if (sourceWebDavStoreItem.ItemPath == destination.ItemPath)
                    throw new WebDavForbiddenException();
                // if the overwrite header is F, statuscode = precondition failed 
                if (!GetOverwriteHeader(headers))
                    throw new WebDavPreconditionFailedException();
                // else delete destination and set isNew to false
                destinationParentCollection.Delete(destination);
                isNew = false;
            }

            destinationParentCollection.MoveItemHere(sourceWebDavStoreItem, destinationName);

            // send correct response
            response.SendSimpleResponse(isNew ? HttpStatusCode.Created : HttpStatusCode.NoContent);
        }
    }
}