using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Common.Logging;
using WebDAVSharp.Server.Exceptions;
using WebDAVSharp.Server.MethodHandlers;
using WebDAVSharp.Server.Stores;
using WebDAVSharp.Server.Stores.DiskStore;

namespace WebDAVSharp.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class WebDAVHandler : IHttpHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public const string HttpUser = "HTTP.User";

        private readonly IWebDavStore _store;
        private readonly Dictionary<string, IWebDavMethodHandler> _methodHandlers;
        private readonly ILog _log;

        private readonly string _localStore;
        private readonly string _url;

        /// <summary>
        /// 
        /// </summary>
        public WebDAVHandler()
        {
            _localStore = ConfigurationManager.AppSettings["LOCALSTORE"];
            if(string.IsNullOrWhiteSpace(_localStore))
            {
                _localStore = @"E:\Projects\webDav\LocalPath";
            }
            _store = new WebDavDiskStore(_localStore);

            var methodHandlers = WebDavMethodHandlers.BuiltIn;

            IWebDavMethodHandler[] webDavMethodHandlers = methodHandlers as IWebDavMethodHandler[] ?? methodHandlers.ToArray();

            if (!webDavMethodHandlers.Any())
                throw new ArgumentException("The methodHandlers collection is empty", "methodHandlers");
            if (webDavMethodHandlers.Any(methodHandler => methodHandler == null))
                throw new ArgumentException("The methodHandlers collection contains a null-reference", "methodHandlers");

            var handlersWithNames =
                from methodHandler in webDavMethodHandlers
                from name in methodHandler.Names
                select new
                {
                    name,
                    methodHandler
                };
            _methodHandlers = handlersWithNames.ToDictionary(v => v.name, v => v.methodHandler);
            _log = LogManager.GetLogger("WebDAVSharp.Server");

            _url = "";
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            // For authentication
            Thread.SetData(Thread.GetNamedDataSlot(HttpUser), context.User.Identity);

            _log.Info(context.Request.HttpMethod + ": " + context.Request.Url);
            try
            {
                try
                {
                    string method = context.Request.HttpMethod;
                    if (!_methodHandlers.TryGetValue(method, out var methodHandler))
                        throw new WebDavMethodNotAllowedException(string.Format(CultureInfo.InvariantCulture, "%s ({0})", context.Request.HttpMethod));

                    context.Response.AppendHeader("DAV", "1,2,1#extend");

                    var coll = new List<string>() {_url};
                    methodHandler.ProcessRequest(context.Request, context.Response, _store, coll);
                }
                catch (WebDavException)
                {
                    throw;
                }
                catch (UnauthorizedAccessException)
                {
                    throw new WebDavUnauthorizedException();
                }
                catch (FileNotFoundException ex)
                {
                    _log.Warn(ex.Message);
                    throw new WebDavNotFoundException(innerException: ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    _log.Warn(ex.Message);
                    throw new WebDavNotFoundException(innerException: ex);
                }
                catch (NotImplementedException ex)
                {
                    _log.Warn(ex.Message);
                    throw new WebDavNotImplementedException(innerException: ex);
                }
                catch (Exception ex)
                {
                    _log.Warn(ex.Message);
                    throw new WebDavInternalServerException(innerException: ex);
                }
            }
            catch (WebDavException ex)
            {
                _log.Warn(ex.StatusCode + " " + ex.Message);
                context.Response.StatusCode = ex.StatusCode;
                context.Response.StatusDescription = ex.StatusDescription;
                if (ex.Message != context.Response.StatusDescription)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(ex.Message);
                    context.Response.ContentEncoding = Encoding.UTF8;
                    //context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                context.Response.Close();
            }
            finally
            {
                _log.Info(context.Response.StatusCode + " " + context.Response.StatusDescription + ": " + context.Request.HttpMethod + ": " + context.Request.Url);
            }
        }
    }
}
