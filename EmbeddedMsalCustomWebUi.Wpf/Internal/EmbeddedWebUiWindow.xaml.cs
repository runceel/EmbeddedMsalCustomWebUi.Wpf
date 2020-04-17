using Microsoft.Identity.Client.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmbeddedMsalCustomWebUi.Wpf.Internal
{
    public partial class EmbeddedWebUiWindow : Window
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

        private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (!e.Uri.ToString().StartsWith(_redirectUri.ToString()))
            {
                // not redirect uri case
                return;
            }

            var query = HttpUtility.ParseQueryString(e.Uri.Query);
            if (query.AllKeys.Any(x => x == "code"))
            {
                _taskCompletionSource.SetResult(e.Uri);
            }
            else
            {
                _taskCompletionSource.SetException(
                    new MsalExtensionException(
                        $"An error occurred, error: {query.Get("error")}, error_description: {query.Get("error_description")}"));
            }

            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _token = _cancellationToken.Register(() => _taskCompletionSource.SetCanceled());
            webBrowser.Navigate(_authorizationUri);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _taskCompletionSource.TrySetCanceled();
            _token.Dispose();
        }
    }
}
