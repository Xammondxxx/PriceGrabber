using System;
using System.IO;
using Android.Webkit;
using Android.OS;
using PriceGrabber.DependencyServices;
using PriceGrabber.Droid.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(WebCacheHelper))]
namespace PriceGrabber.Droid.DependencyServices
{
    public class WebCacheHelper : IWebCacheHelper
    {
        public static string CACHE_PATH = System.Environment.GetFolderPath(System.Environment.SpecialFolder.InternetCache);


        public void ClearCache()
        {
            try
            {
                var dir = MainActivity.Instance.ApplicationContext.CacheDir;
                DeleteCacheDir(dir);
            }
            catch (Exception e) { }
        }

        private bool DeleteCacheDir(Java.IO.File dir)
        {
            if (dir != null && dir.IsDirectory)
            {
                String[] children = dir.List();
                for (int i = 0; i < children.Length; i++)
                {
                    bool success = DeleteCacheDir(new Java.IO.File(dir, children[i]));
                    if (!success)
                    {
                        return false;
                    }
                }
                return dir.Delete();
            }
            else if (dir != null && dir.IsFile)
            {
                return dir.Delete();
            }
            else
            {
                return false;
            }
        }

        public void ClearCookies()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.LollipopMr1)
            {
                CookieManager.Instance.RemoveAllCookies(null);
                CookieManager.Instance.Flush();
            }
            else
            {
#pragma warning disable CS0618 // Тип или член устарел
                CookieSyncManager cookieSyncMngr = CookieSyncManager.CreateInstance(MainActivity.Instance.ApplicationContext);
#pragma warning restore CS0618 
                cookieSyncMngr.StartSync();
                CookieManager cookieManager = CookieManager.Instance;
                cookieManager.RemoveAllCookie();
                cookieManager.RemoveSessionCookie();
                cookieSyncMngr.StopSync();
                cookieSyncMngr.Sync();
            }
        }

        // in KB
        public double GetCacheSize()
        {
            Java.IO.File[] files = MainActivity.Instance?.ApplicationContext?.CacheDir?.ListFiles();

            if (files == null) return 0;
            long size = 0;

            size += GetDirSize(MainActivity.Instance?.ApplicationContext?.CacheDir);
            size += GetDirSize(MainActivity.Instance?.ApplicationContext?.ExternalCacheDir);

            return Math.Round((double)size / 1024, 2);

        }

        public long GetDirSize(Java.IO.File dir)
        {
            if (dir == null) return 0;
            long size = 0;
            foreach (Java.IO.File f in dir.ListFiles())
            {
                if (f != null && f.IsDirectory)
                {
                    size += GetDirSize(f);
                }
                else if (f != null && f.IsFile)
                {
                    size += f.Length();
                }
            }
            return size;
        }
    }
}