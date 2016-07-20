using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System;
using System.IO;
using Android.Views;
using Android.Util;
using System.Threading;


namespace FirstGPSApp
{
    class TimerExampleState
    {
        public int counter = 0;
        public Timer tmr;
    }

    [Activity(Label = "FirstGPSApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView _locationText;
        TextView _addressText;
        TextView _remarksText;

        TextView FileText;

        GPSServiceBinder _binder;
        GPSServiceConnection _gpsServiceConnection;
        Intent _gpsServiceIntent;
        private GPSServiceReciever _receiver;

        public static MainActivity Instance;

        GPSService service;

        private Context context;

        // Create the delegate that invokes methods for the timer.


        protected override void OnCreate(Bundle bundle)
        {
            

            base.OnCreate(bundle);

            Instance = this;
            base.OnCreate(bundle);
            service = new GPSService();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _addressText = FindViewById<TextView>(Resource.Id.txtAddress);
            _locationText = FindViewById<TextView>(Resource.Id.txtLocation);
            _remarksText = FindViewById<TextView>(Resource.Id.txtRemarks);
            FileText = FindViewById<TextView>(Resource.Id.FileText);
            var button = FindViewById<Button>(Resource.Id.MyButton);
            
            Console.WriteLine("Starting app");
            RegisterService();

            TimerExampleState s = new TimerExampleState();
            TimerCallback timerDelegate = new TimerCallback(CheckStatus);
            Timer timer = new Timer(timerDelegate, s, 1000, 1000);

            // Keep a handle to the timer, so it can be disposed.
            s.tmr = timer;

            //service.startTimer();
            while (s.tmr != null)
                Thread.Sleep(0);
            Console.WriteLine("Timer example done.");



        }

        static void CheckStatus(Object state)
        {
            TimerExampleState s = (TimerExampleState)state;
            s.counter++;
            Console.WriteLine("TimerEvent");
            //SaveData.writeData(DateTime.Now, service.getLocation());
            //System.Console.WriteLine("Date: " + DateTime.Now + "Location: " + service.getLocation().ToString());
            Log.Debug("Test", "Test Timer Event");

        }


        void button_Click (object sender, EventArgs e)
        {
            FileText.Text = "GPS: Initial ";

            WriteTextFile();


            //var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //var filePath = System.IO.Path.Combine(sdCardPath, "GPSFile.txt");
            //if (!System.IO.File.Exists(filePath))
            //{
            //    using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath, true))
            //    {
            //        FileText.Text = reader.ReadLine();
            //    }

            //}
                //else
                //{
                //    FileText.Text = "No data file found.";
                //    string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                //    string fileName = Path.Combine(path, "myfile.txt");

                //    using (var streamReader = new StreamReader(fileName))
                //    {
                //        string content = streamReader.ReadLine();
                //        //System.Diagnostics.Debug.WriteLine(content);
                //        FileText.Text = content;
                //    }


                //}

            }

        private void WriteTextFile()
        {
            
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filePath = System.IO.Path.Combine(sdCardPath, "MyTextFile.txt");

            using (Java.IO.FileWriter writer = new Java.IO.FileWriter(filePath, true))
            {
                writer.Write(DateTime.Now + ", " + service.getLocation().Latitude + ", " + service.getLocation().Longitude + " / n");
                FileText.Text = SaveData.writeData(DateTime.Now, service.getLocation());
                writer.Flush();
                writer.Close();
            }



        }

        private void RegisterService()
        {
            _gpsServiceConnection = new GPSServiceConnection(_binder);
            _gpsServiceIntent = new Intent(Android.App.Application.Context, typeof(GPSService));
            BindService(_gpsServiceIntent, _gpsServiceConnection, Bind.AutoCreate);
        }
        private void RegisterBroadcastReceiver()
        {
            IntentFilter filter = new IntentFilter(GPSServiceReciever.LOCATION_UPDATED);
            filter.AddCategory(Intent.CategoryDefault);
            _receiver = new GPSServiceReciever();
            RegisterReceiver(_receiver, filter);
        }

        private void UnRegisterBroadcastReceiver()
        {
            UnregisterReceiver(_receiver);
        }


        public void UpdateUI(Intent intent)
        {
            _locationText.Text = intent.GetStringExtra("Location");
            _addressText.Text = intent.GetStringExtra("Address");
            _remarksText.Text = intent.GetStringExtra("Remarks");
        }

        protected override void OnResume()
        {
            base.OnResume();
            RegisterBroadcastReceiver();
        }

        protected override void OnPause()
        {
            base.OnPause();
            //UnRegisterBroadcastReceiver();
        }


        protected void onBackPressed()
        {
            Toast.MakeText(context, "Thanks for using application!!", ToastLength.Long).Show();
            UnRegisterBroadcastReceiver();
            Finish();

        }



        [BroadcastReceiver]
        internal class GPSServiceReciever : BroadcastReceiver
        {
            public static readonly string LOCATION_UPDATED = "LOCATION_UPDATED";
            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action.Equals(LOCATION_UPDATED))
                {
                    MainActivity.Instance.UpdateUI(intent);
                }

            }
        }


    }
}




