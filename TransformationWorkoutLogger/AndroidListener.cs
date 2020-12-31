using Android.App;
using Android.OS;
using static Android.App.Application;

namespace TransformationWorkoutLogger
{
    public class AndroidListener : Java.Lang.Object, IOnProvideAssistDataListener
    {
        public void OnProvideAssistData(Activity activity, Bundle data) { }
    }
}