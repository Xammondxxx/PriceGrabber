using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.WebService
{

    public class UrlData
    {
        public string AbsoluteUrl { get; set; }
        public string MetricUrl { get; set; }
        public string Url { get; set; }
        public string Query { get; set; }
        public string PageTitle { get; set; }
        public bool PullToRefreshOff { get; set; }
        public bool FullScreen { get; set; }
        //public bool RefreshOnReturn { get; set; }
        public Dictionary<string, string> Params { get; set; }
        public string ViewId { get; set; }

        public void AddParam(string key, string val)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val)) return;
            if (Params == null) Params = new Dictionary<string, string>();
            if (Params.ContainsKey(key)) Params[key] = val;
            else Params.Add(key, val);

            Query = string.Empty;
            var mq = string.Empty;
            foreach (var p in Params)
            {
                Query = Query + (string.IsNullOrEmpty(Query) ? "" : "&") + p.Key + "=" + p.Value;
                if (p.Key.ToLower() == "entryid") mq = mq + p.Key + "=" + p.Value;
            }
            AbsoluteUrl = Url + (string.IsNullOrEmpty(Query) ? "" : "?") + Query;
            MetricUrl = Url + (string.IsNullOrEmpty(mq) ? "" : "?") + mq;
        }
    }

    public static class UrlParser
    {
        const string PAGE_LAYOUT_KEY = "PageLayout";
        const string PAGE_LAYOUT_SHORT_KEY = "PL";
        const string PAGE_TITLE_KEY = "PageTitle";
        const string PAGE_TITLE_SHORT_KEY = "PT";
        const string PULL_TO_REFRESH_KEY = "PullToRefresh";
        const string PULL_TO_REFRESH_SHORT_KEY = "PTR";
        const string ORIENTATION_KEY = "Orientation";
        const string ORIENTATION_SHORT_KEY = "O";
        const string FULL_SCREEN_KEY = "HideToolbar";
        const string FULL_SCREEN_SHORT_KEY = "HT";
        //const string REFRESH_ON_RETURN_KEY = "RefreshOnReturn";

        public static UrlData Parse(string url)
        {
            var res = new UrlData();
            if (string.IsNullOrEmpty(url)) return res;

            if (!url.Contains("http") && (url.Contains("form/view") || url.Contains("form/rs")))
                url = PGService.ApiServerUrl + url;
            if (url.Contains("&ErrorLog="))
            {
                res.AbsoluteUrl = url;
                res.Url = url;
                return res;
            }


            var q = url.IndexOf('?');
            if (q > 0)
            {
                res.Params = new Dictionary<string, string>();
                var pars = url.Substring(q + 1).Split('&');
                foreach (var par in pars)
                {
                    var idx = par.IndexOf('=');
                    //var v = par.Split('=');
                    if (idx > 0)
                    {
                        var n = par.Substring(0, idx);
                        var v = "";
                        if (par.Length > (idx + 1)) v = par.Substring(idx + 1);
                        if (!res.Params.ContainsKey(n))
                            res.Params.Add(n, v);
                    }
                    /*if (v.Length == 2)
                        res.Params.Add(v[0], v[1]);*/
                }
                res.Url = url.Substring(0, q);
                res.Query = string.Empty;
                foreach (var p in res.Params)
                    res.Query = res.Query + (string.IsNullOrEmpty(res.Query) ? "" : "&") + p.Key + "=" + p.Value;
                res.AbsoluteUrl = res.Url + (string.IsNullOrEmpty(res.Query) ? "" : "?") + res.Query;
            }
            else
            {
                res.Url = url;
                res.AbsoluteUrl = res.Url;
            }


            return res;
        }
    }
}
