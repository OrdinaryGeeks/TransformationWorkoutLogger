
using Android;
using Android.App;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using Xamarin.Essentials;

namespace TransformationWorkoutLogger
{
    [Activity(Label = "@string/app_name")]
    //Implement ILocationListener interface to get location updates
    public class DistanceActivity : AppCompatActivity, ILocationListener
    {
        const long ONE_MINUTE = 60 * 1000;
        const long FIVE_MINUTES = 5 * ONE_MINUTE;
        static readonly string KEY_REQUESTING_LOCATION_UPDATES = "requesting_location_updates";

        static readonly int RC_LAST_LOCATION_PERMISSION_CHECK = 1000;
        static readonly int RC_LOCATION_UPDATES_PERMISSION_CHECK = 1100;

        float totalTimeElapsed;

        Button getLastLocationButton;
        bool isRequestingLocationUpdates;
        TextView latitude;
        internal TextView latitude2;
        LocationManager locationManager;
        TextView longitude;
        internal TextView longitude2;
        TextView provider;
        internal TextView provider2;
        internal Button requestLocationUpdatesButton;
        View rootLayout;
        private TextView distanceRun;
        public double distanceRunDouble;
        public float TimeElapsed;
        public DateTime StartTime;
        public void OnLocationChanged(Android.Locations.Location location)
        {
            latitude.Text = latitude2.Text;
            longitude.Text = longitude2.Text;

            double oldThreshold = .1;


            latitude2.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
            longitude2.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
            provider2.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
            distanceRunDouble += distance(double.Parse(latitude.Text.ToString()), double.Parse(longitude.ToString()), double.Parse(latitude2.ToString()), double.Parse(longitude2.ToString()), 'M');
            distanceRun.Text = distanceRunDouble.ToString();

            if (distanceRunDouble > oldThreshold)
            {

                oldThreshold += .1;
                Threshold();
            
            }
        }

        public async void Threshold()
        {

            TimeSpan ElapsedTime = DateTime.Now - StartTime;
            await TextToSpeech.SpeakAsync(distanceRunDouble.ToString() + " IN  " + ElapsedTime.Minutes.ToString());

        }
        public void OnProviderDisabled(string provider)
        {
            isRequestingLocationUpdates = false;
            requestLocationUpdatesButton.SetText(Resource.String.request_location_button_text);
            latitude2.Text = string.Empty;
            longitude2.Text = string.Empty;
            provider2.Text = string.Empty;
        }

        public void OnProviderEnabled(string provider)
        {
            // Nothing to do in this example.
            Log.Debug("LocationExample", "The provider " + provider + " is enabled.");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            if (status == Availability.OutOfService)
            {
                StopRequestingLocationUpdates();
                isRequestingLocationUpdates = false;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK || requestCode == RC_LOCATION_UPDATES_PERMISSION_CHECK)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK)
                    {
                        GetLastLocationFromDevice();
                    }
                    else
                    {
                        isRequestingLocationUpdates = true;
                        StartRequestingLocationUpdates();
                    }
                }
                else
                {
                    Snackbar.Make(rootLayout, Resource.String.permission_not_granted_termininating_app, Snackbar.LengthIndefinite)
                            .SetAction(Resource.String.ok, delegate { FinishAndRemoveTask(); })
                            .Show();
                    return;
                }
            }
            else
            {
                Log.Debug("LocationSample", "Don't know how to handle requestCode " + requestCode);
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            locationManager = (LocationManager)GetSystemService(LocationService);

            if (bundle != null)
            {
                isRequestingLocationUpdates = bundle.KeySet().Contains(KEY_REQUESTING_LOCATION_UPDATES) &&
                                              bundle.GetBoolean(KEY_REQUESTING_LOCATION_UPDATES);
            }
            else
            {
                isRequestingLocationUpdates = false;
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.distance);
            rootLayout = FindViewById(Resource.Id.root_layout);
            distanceRun = FindViewById<TextView>(Resource.Id.distanceRun);
            getLastLocationButton = FindViewById<Button>(Resource.Id.get_last_location_button);
            latitude = FindViewById<TextView>(Resource.Id.latitude);
            longitude = FindViewById<TextView>(Resource.Id.longitude);
            provider = FindViewById<TextView>(Resource.Id.provider);

            requestLocationUpdatesButton = FindViewById<Button>(Resource.Id.request_location_updates_button);
            latitude2 = FindViewById<TextView>(Resource.Id.latitude2);
            longitude2 = FindViewById<TextView>(Resource.Id.longitude2);
            provider2 = FindViewById<TextView>(Resource.Id.provider2);

            if (locationManager.AllProviders.Contains(LocationManager.NetworkProvider)
                && locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
            {
                getLastLocationButton.Click += GetLastLocationButtonOnClick;
                requestLocationUpdatesButton.Click += RequestLocationUpdatesButtonOnClick;
            }
            else
            {
                Snackbar.Make(rootLayout, Resource.String.missing_gps_location_provider, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok, delegate { FinishAndRemoveTask(); })
                        .Show();
            }
        }

        void RequestLocationUpdatesButtonOnClick(object sender, EventArgs eventArgs)
        {

            StartTime = DateTime.Now;
            if (isRequestingLocationUpdates)
            {
                isRequestingLocationUpdates = false;
                StopRequestingLocationUpdates();
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
                {
                    StartRequestingLocationUpdates();
                    isRequestingLocationUpdates = true;
                }
                else
                {
                    RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
                }
            }
        }

        void GetLastLocationButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                GetLastLocationFromDevice();
            }
            else
            {
                RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
            }
        }

        void GetLastLocationFromDevice() 
        {
            getLastLocationButton.SetText(Resource.String.getting_last_location);

            var criteria = new Criteria { PowerRequirement = Power.Medium };

            var bestProvider = locationManager.GetBestProvider(criteria, true);
            var location = locationManager.GetLastKnownLocation(bestProvider);

            if (location != null)
            {
                latitude.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
                longitude.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
                provider.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
                getLastLocationButton.SetText(Resource.String.get_last_location_button_text);
            }
            else
            {
                latitude.SetText(Resource.String.location_unavailable);
                longitude.SetText(Resource.String.location_unavailable);
                provider.Text = Resources.GetString(Resource.String.provider_string, bestProvider);
                getLastLocationButton.SetText(Resource.String.get_last_location_button_text);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            locationManager = GetSystemService(LocationService) as LocationManager;
        }
        protected override void OnPause()
        {
            locationManager.RemoveUpdates(this);
            base.OnPause();
        }

        void RequestLocationPermission(int requestCode)
        {
            isRequestingLocationUpdates = false;
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                Snackbar.Make(rootLayout, Resource.String.permission_location_rationale, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok,
                                   delegate
                                   {
                                       ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, requestCode);
                                   })
                        .Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, requestCode);
            }
        }

        void StartRequestingLocationUpdates()
        {
            long time = 3000;
            requestLocationUpdatesButton.SetText(Resource.String.request_location_in_progress_button_text);
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, time, 2f, this);
        }

        void StopRequestingLocationUpdates()
        {
            latitude2.Text = string.Empty;
            longitude2.Text = string.Empty;
            provider2.Text = string.Empty;

            requestLocationUpdatesButton.SetText(Resource.String.request_location_button_text);
            locationManager.RemoveUpdates(this);
        }
        //https://www.geodatasource.com/developers/c-sharp
        private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

    }
}

