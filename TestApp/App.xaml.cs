using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static new App Current => (App)Application.Current;
        public IPublicClientApplication PublicClientApplication { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<App>()
                .Build();

            var b2cSection = config.GetSection(nameof(AzureAdB2C));
            if (b2cSection == null)
            {
                MessageBox.Show(@"Please set Azure AD B2C info to user secrets like below:
{
  ""AzureAdB2C"": {
    ""ClientId"": ""your client id"",
    ""RedirectUri"": ""your redirect uri"",
    ""TenantId"": ""your tenant id"",
    ""B2CAuthority"": ""https://{your_tenant_name}.b2clogin.com/tfp/{your_tenant_name}.onmicrosoft.com/{sign_in_flow_name}""
  }
}");
                Shutdown();
                return;
            }

            var b2cSettings = b2cSection.Get<AzureAdB2C>();
            PublicClientApplication = PublicClientApplicationBuilder.Create(b2cSettings.ClientId)
                .WithRedirectUri(b2cSettings.RedirectUri)
                .WithTenantId(b2cSettings.TenantId)
                .WithB2CAuthority(b2cSettings.B2CAuthority)
                .Build();
        }
    }
}
