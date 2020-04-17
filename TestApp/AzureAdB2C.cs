using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
    public class AzureAdB2C
    {
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string RedirectUri { get; set; }
        public string B2CAuthority { get; set; }
    }
}
