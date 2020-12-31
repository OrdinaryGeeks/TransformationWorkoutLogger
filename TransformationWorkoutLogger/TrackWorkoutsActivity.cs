using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Interop;

namespace TransformationWorkoutLogger
{
   



    [Activity(Label = "TrackWorkoutsActivity", Theme = "@style/AppTheme", MainLauncher = true)]
    public class TrackWorkoutsActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.trackworkouts);

           
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
        }



        [Export("ButtonClick")]

        public void ButtonClick(View v)
        {


            Button button = (Button)v;

            if (button.Text == "Track Distance Running")
            {
                Intent intent = new Intent(this, typeof(MainActivity));

                StartActivity(intent);


            }
            if(button.Text == "Track Weightlifting")
            {
                Intent intent = new Intent(this, typeof(MainActivity));

                StartActivity(intent);


            }
        }

        


     
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                   // textMessage.SetText(Resource.String.title_home);
                    return true;
                case Resource.Id.navigation_dashboard:
                   // textMessage.SetText(Resource.String.title_dashboard);
                    return true;
                case Resource.Id.navigation_notifications:
                   // textMessage.SetText(Resource.String.title_notifications);
                    return true;
            }
            return false;
        }
    }
}