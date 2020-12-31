
using Android;
using Android.App;
using Android.App.Assist;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Speech;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.Security;
using Org.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Xamarin.Essentials;

using static Android.App.Application;

namespace TransformationWorkoutLogger
{
    [Activity(Label = "@string/app_name")]

    [IntentFilter(new[] { Intent.ActionAssist }, Categories = new[] { Intent.CategoryDefault })]

    //Implement ILocationListener interface to get location updates
    [IntentFilter(new[] { MediaStore.ActionImageCapture }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryVoice })]
    [IntentFilter(new[] { MediaStore.ActionImageCaptureSecure }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryVoice })]
    public class MainActivity : AppCompatActivity, ILocationListener
    {

        // public static Uri ALARM_URI = Uri.parse("android-app://com.myclockapp/set_alarm_page");
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
        public float timeTarget;
        double oldThreshold;
        public TextView threshold;
        public TimeSpan warningCooldown;
        DateTime cooldownEnd;

        string enteredSpeechToText;


        public void OnProvideAssistData(Activity activity, Bundle data) {


        }
        protected override void OnResume()
        {
            base.OnResume();
            if (!IsVoiceInteraction)
                return;

            var prompt = new VoiceInteractor.Prompt("A taxi is about 5 minutes away do you want to be picked up?");
            var request = new ConfirmTaxiRequest(prompt);
            VoiceInteractor.SubmitRequest(request);
        }
        public void OnLocationChanged(Android.Locations.Location location)
        {
            try
            {
                latitude.Text = latitude2.Text;
                longitude.Text = longitude2.Text;



                latitude2.Text = location.Latitude.ToString();// Resources.GetString(Resource.String.latitude_string, location.Latitude);
                longitude2.Text = location.Longitude.ToString();// Resources.GetString(Resource.String.longitude_string, location.Longitude);
                                                                //  provider2.Text = latitude2.Text + " " + latitude.Text + longitude2.Text + longitude.Text;
                if (latitude.Text != "")
                {
                    double lat1 = double.Parse(latitude.Text.ToString());
                    double lon1 = double.Parse(longitude.Text.ToString());
                    double lat2 = double.Parse(latitude2.Text.ToString());
                    double lon2 = double.Parse(longitude2.Text.ToString());
                    distanceRunDouble += distance(lat1, lon1, lat2, lon2, 'M');
                    distanceRun.Text = distanceRunDouble.ToString();
                }

                TimeSpan ElapsedTime = DateTime.Now - StartTime;

                double distanceWeShouldHaveRun = (ElapsedTime.TotalSeconds / 60.0f) / 8.0f;

                if (cooldownEnd < DateTime.Now)
                    if (distanceRunDouble < distanceWeShouldHaveRun)
                    {
                        SpeedUp(1 / 8 * ElapsedTime.TotalSeconds / 60, distanceRunDouble);
                        warningCooldown = TimeSpan.FromSeconds(5);
                        cooldownEnd = DateTime.Now + TimeSpan.FromSeconds(15);
                    }

                if (distanceRunDouble > oldThreshold)
                {

                    oldThreshold += .025;
                    threshold.Text = oldThreshold.ToString();
                    Threshold();

                }
            }
            catch (Exception e)
            {
                Snackbar.Make(latitude, e.Message, 5);
            }
        }
        public async void SpeedUp(double elapsedTime, double distanceRun)
        {

            Random rand = new Random();
            int motivation = rand.Next(4);

            if (motivation == 1)
            {
                await TextToSpeech.SpeakAsync("Speed up. You are " + String.Format("{0:000.000}", (elapsedTime - distanceRun)) + " behind the pace");

            }
            if (motivation == 2)
            {
                await TextToSpeech.SpeakAsync("If it was easy everyone would be doing it");
            }
            if (motivation == 3)
            {
                await TextToSpeech.SpeakAsync("You got this. Now push to catch up");
            }

        }
        public async void Threshold()
        {

            TimeSpan ElapsedTime = DateTime.Now - StartTime;

            await TextToSpeech.SpeakAsync(String.Format("{0:000.000}", distanceRunDouble) + " IN  " + String.Format("{0:000.00}", (ElapsedTime.TotalSeconds / 60.0f)) + " minutes");

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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK || requestCode == RC_LOCATION_UPDATES_PERMISSION_CHECK)
            {
                if (grantResults.Length == 1 && grantResults[0] == Android.Content.PM.Permission.Granted)
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

        public override void OnProvideAssistContent(AssistContent outContent)
        {
            base.OnProvideAssistContent(outContent);

            // Provide some JSON 
            string structuredJson = new JSONObject()
                .Put("@type", "MusicRecording")
                .Put("@id", "https://example.com/music/recording")
                .Put("name", "Album Title")
                .ToString();

            outContent.StructuredData = structuredJson;
        }

        public override void OnProvideAssistData(Bundle data)
        {
            base.OnProvideAssistData(data);
        }

        private static int SPEECH_REQUEST_CODE = 0;

        // Create an intent that can start the Speech Recognizer activity
        private void displaySpeechRecognizer()
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel,
                    RecognizerIntent.LanguageModelFreeForm);
            // Start the activity, the intent will be populated with the speech text
            StartActivityForResult(intent, SPEECH_REQUEST_CODE);
        }


        public bool isStringDigit(string check)
        {

            int periodCount = 0;
            for (int i = 0; i < check.Length; i++)
            {
                if ((!char.IsDigit(check[i])))
                {
                    if (check[i] == '.')
                    {
                        periodCount++;
                        if (periodCount >= 2)
                            return false;

                    }
                    else
                        return false;
                }

            }

            return true;
        }

        public bool checkStringForStatement(string command, string text)
        {


            int foundStart = text.IndexOf(command);
            int foundEnd = text.IndexOf("end");

            if (foundEnd > foundStart)
            {

                text.Substring(foundStart, foundEnd - foundStart);


                TextToSpeech.SpeakAsync(text.Substring(foundStart, foundEnd - foundStart));
            }

            //   int i = 0; 
            //  if(text[startIndex] == )



            return false;
        }

        public string checkStringForTimesInsteadOfReps(string text)
        {


            return text.Replace(":", " ");


        }
        protected override void OnActivityResult(int requestCode, Result resultCode,
        Intent data)
        {
            if (requestCode == SPEECH_REQUEST_CODE && resultCode == Result.Ok)
            {
                IList<String> results = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                enteredSpeechToText = results[0];


                TextView oldString = (TextView)FindViewById(Resource.Id.oldString);
                TextView newString = (TextView)FindViewById(Resource.Id.newString);

                oldString.Text = enteredSpeechToText;
                //workout start bench press stop
                //weights start 140 150 160 stop
                //reps start 10 40 20 stop
                string[] speech = enteredSpeechToText.Split(" ");

                List<string> workouts = new List<string>();
                List<List<decimal>> weights = new List<List<decimal>>();
                List<List<int>> reps = new List<List<int>>();

                string changedString = enteredSpeechToText.Replace("wraps", "reps").Replace("in", "end").Replace("sets", "set").Replace("at","add");
                string repsNotTimesString = checkStringForTimesInsteadOfReps(changedString);

                newString.Text = repsNotTimesString;
                checkStringForStatement("add set to workout", repsNotTimesString);
                /*  int workoutIndex = 0;
                  int weightsIndex = 0;
                  int repsIndex = 0;
                  string workoutName = "";
                  for (int i= 0; i< speech.Length; i++)
                  {

                      //check for add reps to last workout

                  //    if(speech[i] == "add)

                      if(speech[i] == "workout")
                      {
                          int j = 2;

                          if (speech[i + 1] == "start")
                          {

                              while (speech[i + j] != "stop")
                              {
                                  workoutName += speech[i + j] + " ";
                                  j++;
                              }
                          }
                          i += j;
                          workouts.Add(workoutName);
                      }

                      if(speech[i] == "weights")
                      {
                          int j = 2;
                          if (speech[i + 1] == "start")
                          {
                              repsIndex = 0;
                              while (speech[i + j] != "stop")
                              {
                                  if (isStringDigit(speech[i + j])) 
                                  weights[weightsIndex].Add(int.Parse(speech[i + j]));

                                  j++;
                              }
                              weightsIndex++;
                          }
                          i += j;
                      }
                      if (speech[i] == "reps")
                      {
                          int j = 2;
                          if (speech[i + 1] == "start")
                          {

                              while (speech[i + j] != "stop")
                              {
                                  if (isStringDigit(speech[i + j])) 
                                  reps[repsIndex].Add(int.Parse(speech[i + j]));

                                  j++;
                              }
                              repsIndex++;
                          }
                          i += j;
                      }

                  }

                  string speak = "";

                  speak += "Workout names are ";
                  foreach (string name in workouts)
                      speak += name + " ";

                  speak += "Weights are ";
                  foreach(List<decimal> weightList in weights)
                  foreach (decimal weight in weightList)
                      speak += weight + " ";

                  speak += "Reps are ";
                  foreach (List<int> repList in reps)
                      foreach (int weight in repList)
                          speak += weight + " ";


                //  SpeechOptions options = new SpeechOptions;
                 // options.
                  TextToSpeech.SpeakAsync(speak);
                  TextToSpeech.SpeakAsync(enteredSpeechToText);



                  */
                // Do something with spokenText
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            enteredSpeechToText = "";

            // Application.RegisterOnProvideAssistDataListener(new AndroidListener());
            Intent intent = Intent;

            /*         // DateTime time = ... // whatever time
          //AlarmManager manager = (AlarmManager)Android.App.Application.Context.GetSystemService(Android.App.Application.Context.AlarmService);

                      Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
                      calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();

                    //  calendar.Set(time.Year, time.Month - 1, time.Day, time.Hour, time.Minute, 0);
                    //  manager.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis,
                    //  AlarmManager.IntervalDay, pendingIntent);
                      var pending = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
                      var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
                      alarmManager.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 5 * 1000, pending);
                      oldThreshold = .25;
                      cooldownEnd = DateTime.Now;*/
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
            SetContentView(Resource.Layout.runningisfun);
            rootLayout = FindViewById(Resource.Id.root_layout);
            distanceRun = FindViewById<TextView>(Resource.Id.distanceRun);
            threshold = FindViewById<TextView>(Resource.Id.threshold);
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

            displaySpeechRecognizer();

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
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted)
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
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted)
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

            var criteria = new Criteria { PowerRequirement = Power.High };

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

        [Export("startSpeechRecognizer")]
        public void restartSpeechRecog()
        {

            displaySpeechRecognizer();

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
            long time = 300;
            requestLocationUpdatesButton.SetText(Resource.String.request_location_in_progress_button_text);
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, time, .02f, this);
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

       
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

    }
    class ConfirmTaxiRequest : VoiceInteractor.ConfirmationRequest
    {
        public ConfirmTaxiRequest(VoiceInteractor.Prompt prompt)
          : base(prompt, null)
        {
        }

        public override void OnConfirmationResult(bool confirmed, Bundle result)
        {
            base.OnConfirmationResult(confirmed, result);
            if (confirmed)
            {
                //Finalize taxi confiramation
                Toast.MakeText(Activity, "Your taxi has been confirmed.", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(Activity, "No taxi ordered.", ToastLength.Long).Show();
            }

            Activity.Finish();
        }

        public override void OnCancel()
        {
            base.OnCancel();
            Activity.Finish();
        }
    }
}

