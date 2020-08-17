using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using WebDAVSharp.Server.Adapters;
using WebDAVSharp.Server.Exceptions;
using WebDAVSharp.Server.Stores;

namespace WebDAVSharp.Server.MethodHandlers
{
    /// <summary>
    /// This class implements the <c>GET</c> HTTP method for WebDAV#.
    /// </summary>
    internal sealed class WebDavGetMethodHandler : WebDavMethodHandlerBase, IWebDavMethodHandler
    {
        /// <summary>
        /// Gets the collection of the names of the verbs handled by this instance.
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
                    "GET"
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
            IWebDavStoreCollection collection = GetParentCollection(prefixes, store, request.Url);
            IWebDavStoreItem item = GetItemFromCollection(collection, request.Url);
            if (!(item is IWebDavStoreDocument doc))
                throw new WebDavNotFoundException();

            long docSize = doc.Size;
            if (docSize == 0)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                //context.Response.ContentLength64 = 0;
            }

            using (Stream stream = doc.OpenReadStream())
            {
                response.StatusCode = (int)HttpStatusCode.OK;

                //if (docSize > 0)
                //    context.Response.ContentLength64 = docSize;

                byte[] buffer = new byte[4096];
                int inBuffer;
                while ((inBuffer = stream.Read(buffer, 0, buffer.Length)) > 0)
                    response.OutputStream.Write(buffer, 0, inBuffer);
            }
            response.Close();
        }
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="prefixes"></param>
        /// <param name="context">The
        /// <see cref="IHttpListenerContext" /> object containing both the request and response
        /// objects to use.</param>
        /// <param name="store">The <see cref="IWebDavStore" /> that the <see cref="WebDavServer" /> is hosting.</param>
        /// <exception cref="WebDAVSharp.Server.Exceptions.WebDavNotFoundException"></exception>
        /// <exception cref="WebDavNotFoundException"><para>
        ///   <paramref name="context" /> specifies a request for a store item that does not exist.</para>
        /// <para>- or -</para>
        /// <para>
        ///   <paramref name="context" /> specifies a request for a store item that is not a document.</para></exception>
        /// <exception cref="WebDavConflictException"><paramref name="context" /> specifies a request for a store item using a collection path that does not exist.</exception>
        public void ProcessRequest(IHttpListenerContext context, IWebDavStore store, IList<string> prefixes)
        {
            IWebDavStoreCollection collection = GetParentCollection(prefixes, store, context.Request.Url);
            IWebDavStoreItem item = GetItemFromCollection(collection, context.Request.Url);
            if (!(item is IWebDavStoreDocument doc))
                throw new WebDavNotFoundException();

            long docSize = doc.Size;
            if (docSize == 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                //context.Response.ContentLength64 = 0;
            }

            using (Stream stream = doc.OpenReadStream())
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                //if (docSize > 0)
                //    context.Response.ContentLength64 = docSize;

                byte[] buffer = new byte[4096];
                int inBuffer;
                while ((inBuffer = stream.Read(buffer, 0, buffer.Length)) > 0)
                    context.Response.OutputStream.Write(buffer, 0, inBuffer);
            }
            context.Response.Close();
        }
    }
}