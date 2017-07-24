using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace DanskeTask
{
	public class HttpHelper
	{
		private const int ConnectionLimit = 100;

		private Encoding _encoding = Encoding.Default;

		private string _useragent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36";

		private string _accept = "text/html, application/xhtml+xml, application/xml, */*";

		private int _timeout = 30*1000;

		private string _contenttype = "application/x-www-form-urlencoded";

		private Dictionary<string,string> _headers = new Dictionary<string, string>();
		
		public HttpHelper()
		{
			_headers.Clear();
			ServicePointManager.DefaultConnectionLimit = ConnectionLimit;
		}
		
		private String GetStringFromResponse(WebResponse response)
		{
			var html = "";
			try
			{
				var stream = response.GetResponseStream();
				var sr = new StreamReader(stream, _encoding);
				html = sr.ReadToEnd();
					
				sr.Close();
				stream.Close();
			}
			catch(Exception e)
			{
				Trace.WriteLine("GetStringFromResponse Error: " + e.Message);
			}
			
			return html;
		}

		private bool CheckCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
		    return true;
		}

		public string HttpGet(string url)
		{
			return HttpGet(url, url);
		}

		public string HttpGet(string url, string refer)
		{
			string html;
			try
			{
				ServicePointManager.ServerCertificateValidationCallback = CheckCertificate;
				var request = (HttpWebRequest)WebRequest.Create(url);
				request.UserAgent = _useragent;
				request.Timeout = _timeout;
				request.ContentType = _contenttype;
				request.Accept = _accept;
				request.Method = "GET";
				request.Referer = refer;
				request.KeepAlive = true;
				request.AllowAutoRedirect = true;
				request.UnsafeAuthenticatedConnectionSharing = true;
				request.CookieContainer = new CookieContainer();

				request.Proxy = null;

				foreach(var hd in _headers)
				{
					request.Headers[hd.Key] = hd.Value;
				}
				
				var response = (HttpWebResponse)request.GetResponse();
				html = GetStringFromResponse(response);
				if(request.CookieContainer != null)
				{
					response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
				}
				
				response.Close();
				return html;
			}
			catch(Exception e)
			{
				Trace.WriteLine("HttpGet Error: " + e.Message);
				return string.Empty;
			}			
		}
	}
}
