using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.WebService
{
    public static class PGConst
    {
        public const string API_SERVER_URL_PROD = "https://app-designer.itcs.hp.com";
        public const string API_SERVER_URL_STAGING = "https://app-designer-stg.itcs.hp.com";

        public const string API_NEW_AUTH = "/mobile/api/v1/sessions/login/provider";
        public const string API_REFRESH_AUTH_TOKEN = "/mobile/api/v1/sessions/refresh-token";
        public const string API_CMD_SSO_DATA = "/mobile/api/v1/sessions/sso-data";

    }
}
