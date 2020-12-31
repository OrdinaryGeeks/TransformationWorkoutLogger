using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Square.Picasso;
using Android.Widget;

using Android.Webkit;

using System.Text.RegularExpressions;


namespace TransformationWorkoutLogger
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    class WorkoutVideo: AppCompatActivity
    {


        int intDisplayWidth;
        int intDisplayHeight;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
           // SetContentView(Resource.Layout.activity_main);

            Window.RequestFeature(Android.Views.WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.workoutVideo);
            var metrics = Resources.DisplayMetrics;
            //fix video screen height and width
            intDisplayWidth = (FnConvertPixelsToDp(metrics.WidthPixels) + 200);
            intDisplayHeight = (FnConvertPixelsToDp(metrics.HeightPixels)) / (2);
           // FnPlayInWebView();

            FnPlayInWebView("https://www.youtube.com/watch?v=Ne73RDg_AzA", Resource.Id.webView1);
            FnPlayInWebView("https://youtu.be/ByPdklB8Q0g", Resource.Id.webView2);
        }

        void FnPlayInWebView(string strUrl, int resourceID)
        {

          //  string strUrl = "https://www.youtube.com/watch?v=Ne73RDg_AzA";

            string id = FnGetVideoID(strUrl);

            if (!string.IsNullOrEmpty(id))
            {
                strUrl = string.Format("http://youtube.com/embed/{0}", id);
            }
            else
            {
                Toast.MakeText(this, "Video url is not in correct format", ToastLength.Long).Show();
                return;
            }

            string html = @"<html><body><iframe width=""videoWidth"" height=""videoHeight"" src=""strUrl""></iframe></body></html>";
            var myWebView = (WebView)FindViewById(resourceID);
            var settings = myWebView.Settings;
            settings.JavaScriptEnabled = true;
            settings.UseWideViewPort = true;
            settings.LoadWithOverviewMode = true;
            settings.JavaScriptCanOpenWindowsAutomatically = true;
            settings.DomStorageEnabled = true;
            settings.SetRenderPriority(WebSettings.RenderPriority.High);
            settings.BuiltInZoomControls = false;

            settings.JavaScriptCanOpenWindowsAutomatically = true;
            myWebView.SetWebChromeClient(new WebChromeClient());
            settings.AllowFileAccess = true;
            settings.SetPluginState(WebSettings.PluginState.On);
            string strYouTubeURL = html.Replace("videoWidth", intDisplayWidth.ToString()).Replace("videoHeight", intDisplayHeight.ToString()).Replace("strUrl", strUrl);

            myWebView.LoadDataWithBaseURL(null, strYouTubeURL, "text/html", "UTF-8", null);

        }
        int FnConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }
        static string FnGetVideoID(string strVideoURL)
        {
            const string regExpPattern = @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)";
            //for Vimeo: vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)
            var regEx = new Regex(regExpPattern);
            var match = regEx.Match(strVideoURL);
            return match.Success ? match.Groups[1].Value : null;
        }


        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    // textMessage.SetText(Resource.String.title_home);
                    return true;
                case Resource.Id.navigation_dashboard:
                    //  textMessage.SetText(Resource.String.title_dashboard);
                    return true;
                case Resource.Id.navigation_notifications:
                    // textMessage.SetText(Resource.String.title_notifications);
                    return true;
            }
            return false;
        }

    }
}