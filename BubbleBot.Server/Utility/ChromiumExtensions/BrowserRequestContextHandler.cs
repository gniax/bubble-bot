using System.Collections.Generic;
using CefSharp;

namespace BubbleBot.Server.Utility.ChromiumExtensions
{
    public class BrowserRequestContextHandler : IRequestContextHandler
    {
        private readonly string Host;
        private readonly string Service;

        public BrowserRequestContextHandler(string host, string service)
        {
            Host = host;
            Service = service;
        }

        IResourceRequestHandler IRequestContextHandler.GetResourceRequestHandler(IBrowser browser, IFrame frame,
            IRequest request, bool isNavigation, bool isDownload, string requestInitiator,
            ref bool disableDefaultHandling)
        {
            return GetResourceRequestHandler(browser, frame, request, isNavigation, isDownload, requestInitiator,
                ref disableDefaultHandling);
        }

        bool IRequestContextHandler.OnBeforePluginLoad(string mimeType, string url, bool isMainFrame,
            string topOriginUrl, WebPluginInfo pluginInfo, ref PluginPolicy pluginPolicy)
        {
            return OnBeforePluginLoad(mimeType, url, isMainFrame, topOriginUrl, pluginInfo, ref pluginPolicy);
        }

        void IRequestContextHandler.OnRequestContextInitialized(IRequestContext requestContext)
        {
            OnRequestContextInitialized(requestContext);
        }

        private IResourceRequestHandler GetResourceRequestHandler(IBrowser browser, IFrame frame, IRequest request,
            bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            IResourceRequestHandler result = default;
            return result;
        }

        private bool OnBeforePluginLoad(string mimeType, string url, bool isMainFrame, string topOriginUrl,
            WebPluginInfo pluginInfo, ref PluginPolicy pluginPolicy)
        {
            return false;
        }

        private void OnRequestContextInitialized(IRequestContext requestContext)
        {
            var v = new Dictionary<string, object>();
            v["mode"] = "fixed_servers";
            v["server"] = "http://" + Host + ':' + Service;
            requestContext.SetPreference("proxy", v, out var error);
        }
    }
}