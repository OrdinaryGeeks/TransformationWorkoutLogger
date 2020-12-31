using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Square.Picasso;
using Android.Widget;
using System;
using Android.Content;
using Java.Interop;

namespace TransformationWorkoutLogger
{



    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
     public class HomeActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        Utils layoutUtils;
        Utils mainLayoutUtils;
        ImageView image1, image2, image3;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.home2);


            var metrics = Resources.DisplayMetrics;

            int height = metrics.HeightPixels;
            int width = metrics.WidthPixels;

            mainLayoutUtils = new Utils(width, height);
            layoutUtils = new Utils(width, height);

     //   https://www.youtube.com/watch?v=hR41k7VvnWw
            Picasso.Get().Load("https://img.youtube.com/vi/OUiZmGy8jHU/1.jpg").Into(FindViewById<ImageView>(Resource.Id.imageView1));
            Picasso.Get().Load("https://img.youtube.com/vi/hR41k7VvnWw/1.jpg").Into(FindViewById<ImageView>(Resource.Id.imageView2));
            Picasso.Get().Load("https://img.youtube.com/vi/PaAh17Hjbak/1.jpg").Into(FindViewById<ImageView>(Resource.Id.imageView3));
//            Picasso.Get().Load("https://img.youtube.com/vi/EvDdQmCNWUc/1.jpg").Into(FindViewById<ImageView>(Resource.Id.datingExplore));

  //          image1 = FindViewById<ImageView>(Resource.Id.imageView1);
    //        image2 = FindViewById<ImageView>(Resource.Id.imageView2);
      //      image3 = FindViewById<ImageView>(Resource.Id.imageView3);

            LinearLayout ll1 = FindViewById<LinearLayout>(Resource.Id.linearLayout1);

            mainLayoutUtils.setMenuLLViewDimensions(ll1);

            LinearLayout ll2 = FindViewById<LinearLayout>(Resource.Id.linearLayout2);

            mainLayoutUtils.setSelectionLLViewDimensions(ll2);

            TextView muscleMemory = FindViewById<TextView>(Resource.Id.muscleMemory);

            muscleMemory.Click += (s, arg) =>
            {
                PopupMenu mmPopUpMenu = new PopupMenu(this, muscleMemory);
                mmPopUpMenu.Inflate(Resource.Menu.musclememorymenu);
                
                mmPopUpMenu.MenuItemClick += (s1, arg1) => {


                    // Console.WriteLine("{0} selected", arg1.Item.TitleFormatted);
                    if (arg1.Item.TitleFormatted.ToString() == "kortney")
                        SetKortney();

                    if (arg1.Item.TitleFormatted.ToString() == "levmarie")
                        SetLevMarie();
                };
            
                mmPopUpMenu.DismissEvent += (s2, arg2) => {
                    Console.WriteLine("menu dismissed");
                };
                mmPopUpMenu.Show();
            };



            // textMessage = FindViewById<TextView>(Resource.Id.message);
            //    BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            //   navigation.SetOnNavigationItemSelectedListener(this);
        }


        /*
         *      exploreSecondary.Visibility = ViewStates.Gone;
            exploreGridLayout.Visibility = ViewStates.Gone;
            exploreBackArrow.Visibility = ViewStates.Gone;
            exploreHome.Visibility = ViewStates.Visible;
*/

        [Export("TextClick")]
        public void TextClick(View v)
        {



            TextView tv = (TextView)v;

            if (tv.Text == "Track Workouts")
            {
                Intent intent = new Intent(this, typeof(TrackWorkoutsActivity));

                StartActivity(intent);


                }
        }


        [Export("ImageClick")]
        public void ImageClick(View v)
        {



            // ImageView image = (ImageView)sender;

            Intent intent = new Intent(this, typeof(WorkoutVideo));


            StartActivity(intent);


        }

        public void SetLevMarie()
        {

            Picasso.Get().Load("https://img.youtube.com/vi/B7cT6KHHspg/1.jpg").Into(FindViewById<ImageView>(Resource.Id.imageView1));
            Picasso.Get().Load("https://img.youtube.com/vi/rB5IUbrvoTc/1.jpg").Into(FindViewById<ImageView>(Resource.Id.imageView2));
            Picasso.Get().Load("https://img.youtube.com/vi/mYJQ6c7HMoo/1.jpg").Into(FindViewById<ImageView>(Resource.Id.imageView3));




        }

        public void SetKortney()
        {


            Picasso.Get().Load("https://img.youtube.com/vi/OUiZmGy8jHU/1.jpg").Into(FindViewById<ImageView>(Resource.Id.imageView1));
            Picasso.Get().Load("https://img.youtube.com/vi/hR41k7VvnWw/1.jpg").Into(FindViewById<ImageView>(Resource.Id.imageView2));
            Picasso.Get().Load("https://img.youtube.com/vi/PaAh17Hjbak/1.jpg").Into(FindViewById<ImageView>(Resource.Id.imageView3));

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