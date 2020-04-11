using CefSharp;
using CefSharp.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Net;

namespace BubbleBot.Core.Accounts
{
	public class BrowserRequestContextHandler : IRequestContextHandler
	{
		private string Host;
		private string Service;

		public BrowserRequestContextHandler(string host, string service)
		{
			Host = host;
			Service = service;
		}

		private IResourceRequestHandler GetResourceRequestHandler(IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
		{
			IResourceRequestHandler result = default(IResourceRequestHandler);
			return result;
		}

		IResourceRequestHandler IRequestContextHandler.GetResourceRequestHandler(IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
		{
			return GetResourceRequestHandler(browser, frame, request, isNavigation, isDownload, requestInitiator, ref disableDefaultHandling);
		}

		private bool OnBeforePluginLoad(string mimeType, string url, bool isMainFrame, string topOriginUrl, WebPluginInfo pluginInfo, ref PluginPolicy pluginPolicy)
		{
			return false;
		}

		bool IRequestContextHandler.OnBeforePluginLoad(string mimeType, string url, bool isMainFrame, string topOriginUrl, WebPluginInfo pluginInfo, ref PluginPolicy pluginPolicy)
		{
			return OnBeforePluginLoad(mimeType, url, isMainFrame, topOriginUrl, pluginInfo, ref pluginPolicy);
		}

		private void OnRequestContextInitialized(IRequestContext requestContext)
		{
			var v = new Dictionary<string, object>();
			v["mode"] = "fixed_servers";
			v["server"] = "http://" + Host + ':' + Service;
			requestContext.SetPreference("proxy", v, out string error);
		}

		void IRequestContextHandler.OnRequestContextInitialized(IRequestContext requestContext)
		{
			this.OnRequestContextInitialized(requestContext);
		}
	}
}
