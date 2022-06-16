using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace EmbeddedMsalCustomWebUi.Wpf.Internal
{
    internal partial class EmbeddedWebUiWindow : Window
    {
        private readonly Uri _authorizationUri;
        private readonly Uri _redirectUri;
        private readonly TaskCompletionSource<Uri> _taskCompletionSource;
        private readonly CancellationToken _cancellationToken;
        private CancellationTokenRegistration _token;

        public EmbeddedWebUiWindow(
            Uri authorizationUri,
            Uri redirectUri,
            TaskCompletionSource<Uri> taskCompletionSource,
            CancellationToken cancellationToken)
        {
            InitializeComponent();
            _authorizationUri = authorizationUri;
            _redirectUri = redirectUri;
            _taskCompletionSource = taskCompletionSource;
            _cancellationToken = cancellationToken;
        }

        private void webView2_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (!e.Uri.StartsWith(_redirectUri.ToString()))
            {
                // not redirect uri case
                return;
            }

            // parse query string
            var uri = new Uri(e.Uri);
            var query = HttpUtility.ParseQueryString(uri.Query);
            if (query.AllKeys.Any(x => x == "code"))
            {
                // It has a code parameter.
                _taskCompletionSource.SetResult(uri);
            }
            else
            {
                // error.
                _taskCompletionSource.SetException(
                    new MsalException(
                        $"An error occurred, error: {query.Get("error")}, error_description: {query.Get("error_description")}"));
            }

            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _token = _cancellationToken.Register(() => _taskCompletionSource.SetCanceled());
            // navigating to an uri that is entry point to authorization flow.
            webView2.Source = _authorizationUri;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _taskCompletionSource.TrySetCanceled();
            _token.Dispose();
        }
    }
}
