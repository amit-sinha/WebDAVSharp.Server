﻿using System.Collections.Generic;
using System.Net;
using System.Web;
using WebDAVSharp.Server.Adapters;
using WebDAVSharp.Server.Stores;

namespace WebDAVSharp.Server.MethodHandlers
{
    /// <summary>
    /// This interface must be implemented by a class that will respond
    /// to requests from a client by handling specific HTTP methods.
    /// </summary>
    public interface IWebDavMethodHandler
    {
        /// <summary>
        /// Gets the collection of the names of the HTTP methods handled by this instance.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        IEnumerable<string> Names
        {
            get;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="prefixes">The <see cref="WebDavServer" /> through which the request came in from the client.</param>
        /// <param name="context">The 
        /// <see cref="IHttpListenerContext" /> object containing both the request and response
        /// objects to use.</param>
        /// <param name="store">The <see cref="IWebDavStore" /> that the <see cref="WebDavServer" /> is hosting.</param>
        void ProcessRequest(IHttpListenerContext context, IWebDavStore store, IList<string> prefixes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="store"></param>
        /// <param name="prefixes"></param>
        void ProcessRequest(HttpRequest request, HttpResponse response,  IWebDavStore store, IList<string> prefixes);
    }
}