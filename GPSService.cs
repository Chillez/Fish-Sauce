using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Widget;
using Android.App;
using Android.Content;
using Android.OS;

using Android.Locations;

using System.Timers;
using Android.Util;

namespace FirstGPSApp
{
    [Service]
    public class GPSService : Service, ILocationListener
    {
        private const string _sourceAddress = "6448 Woodridge Rd, Alexandria Virginia 22312";
        private string _location = string.Empty;
        private string _address = string.Empty;
        private string _remarks = string.Empty;
        string _locationProvider;


        private DateTime time = DateTime.Now;

        public const string LOCATION_UPDATE_ACTION = "LOCATION_UPDATED";
        private Location _currentLocation;
        IBinder _binder;
        protected LocationManager _locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(LocationService);

        


        //Timer timer = new Timer(timerDelegate, s, 1000, 1000);

        private string TAG = "Test: ";

        public void startTimer()
        {
            Timer timer = new Timer();
            //timer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            timer.Interval = 5000;
            timer.Enabled = true;
            //timer.Start();
        }

        public Location getLocation()
        {
            return _currentLocation;
        }

        public void setLocation(Location location)
        {
           _currentLocation = location;
        }
        public void stopTimer(Timer timer)
        {
            timer.Stop();
        }

        public void OnTimeEvent(Object source, ElapsedEventArgs e)
        {
            Log.Debug(TAG, "Test Timer Event");
            Log.Debug("LocationTest", "Location: " + _currentLocation.ToString());
            if (_currentLocation != null)
            {
               SaveData.writeData(DateTime.Now, _currentLocation);

            }

            //Reset
            _currentLocation = null;
        }



 

        public override IBinder OnBind(Intent intent)
        {
            _binder = new GPSServiceBinder(this);
            return _binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public void StartLocationUpdates()
        {
            Criteria criteriaForGPSService = new Criteria
            {
                //A constant indicating an approximate accuracy  
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Low
            };

            var locationProvider = _locationManager.GetBestProvider(criteriaForGPSService, true);
            _locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }

        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };

        public void OnLocationChanged(Location location)
        {
            try
            {
                _currentLocation = location;

                if (_currentLocation == null)
                    _location = "Unable to determine your location.";
                else
                {
                    _location = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);

                    Geocoder geocoder = new Geocoder(this);

                    //The Geocoder class retrieves a list of address from Google over the internet  
                    IList<Address> addressList = geocoder.GetFromLocation(_currentLocation.Latitude, _currentLocation.Longitude, 10);

                    Address addressCurrent = addressList.FirstOrDefault();

                    if (addressCurrent != null)
                    {
                        StringBuilder deviceAddress = new StringBuilder();

                        for (int i = 0; i < addressCurrent.MaxAddressLineIndex; i++)
                            deviceAddress.Append(addressCurrent.GetAddressLine(i))
                                .AppendLine(",");

                        _address = deviceAddress.ToString();
                    }
                    else
                        _address = "Unable to determine the address.";

                    IList<Address> source = geocoder.GetFromLocationName(_sourceAddress, 1);
                    Address addressOrigin = source.FirstOrDefault();

                    var coord1 = new LatLng(addressOrigin.Latitude, addressOrigin.Longitude);
                    var coord2 = new LatLng(addressCurrent.Latitude, addressCurrent.Longitude);

                    var distanceInRadius = Utils.HaversineDistance(coord1, coord2, Utils.DistanceUnit.Miles);

                    

                    _remarks = string.Format("Your are {0} miles away from your original location.", distanceInRadius);

                    Intent intent = new Intent(this, typeof(MainActivity.GPSServiceReciever));
                    intent.SetAction(MainActivity.GPSServiceReciever.LOCATION_UPDATED);
                    intent.AddCategory(Intent.CategoryDefault);
                    intent.PutExtra("Location", _location);
                    intent.PutExtra("Address", _address);
                    intent.PutExtra("Remarks", _remarks);
                    SendBroadcast(intent);
                }
            }
            catch (Exception ex)
            {
                _address = "Unable to determine the address.";
            }

        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }
    }

    public class GPSServiceBinder : Binder
    {

        public GPSService Service { get { return this.LocService; } }
        protected GPSService LocService;
        public bool IsBound { get; set; }
        public GPSServiceBinder(GPSService service) { this.LocService = service; }
    }

    public class GPSServiceConnection : Java.Lang.Object, IServiceConnection
    {
        GPSServiceBinder _binder;

        public event Action Connected;
        public GPSServiceConnection(GPSServiceBinder binder)
        {
            if (binder != null)
                this._binder = binder;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            GPSServiceBinder serviceBinder = (GPSServiceBinder)service;

            if (serviceBinder != null)
            {
                this._binder = serviceBinder;
                this._binder.IsBound = true;
                serviceBinder.Service.StartLocationUpdates();
                if (Connected != null)
                    Connected.Invoke();
            }
        }
        public void OnServiceDisconnected(ComponentName name) { this._binder.IsBound = false; }
    }
}





