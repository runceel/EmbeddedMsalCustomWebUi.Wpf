using EmbeddedMsalCustomWebUi.Wpf;
using Microsoft.Identity.Client.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MenuItemSignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var r = await App.Current
                    .PublicClientApplication
                    .AcquireTokenInteractive(App.Current.Scopes)
                    .WithCustomWebUi(new EmbeddedBrowserWebUi(this))
                    .ExecuteAsync();

                var jwt = new JwtSecurityToken(r.AccessToken);
                var name = jwt.Claims.First(x => x.Type == "name");
                textBlockOutput.Text = $"Hello, {name.Value}.";
            }
            catch (Exception ex)
            {
                textBlockOutput.Text = ex.Message;
                Debug.WriteLine(ex);
            }
        }
    }
}
