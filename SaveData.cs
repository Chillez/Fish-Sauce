using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Locations;
using Java.IO;
using Android.Util;


namespace FirstGPSApp
{
    class SaveData
    {

        private static string TAG2 = "Test: ";

        public static string writeData(DateTime time, Location _location)
        {
            Log.Debug(TAG2, "Writing");

            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filePath = System.IO.Path.Combine(sdCardPath, "GPSFile.txt");
            if (System.IO.File.Exists(filePath))
            {

                Log.Debug(TAG2, "filePath: " + filePath.ToString());
                using (FileWriter writer = new FileWriter(filePath, true))
                {
                    writer.Write(time + ", " + _location.Latitude + ", " + _location.Longitude + "/n");
                }
            }

            FileWriter fWriter;
            Java.IO.File sdCardFile = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + " \filename.txt");
            try
            {
                fWriter = new FileWriter(sdCardFile, true);
                fWriter.Write(time.ToString() + ", " + string.Format("{0:f6}",_location.Latitude) + ", " + string.Format("{0:f6}",_location.Longitude) + "/n");
                fWriter.Flush();
                fWriter.Close();
            }
            catch (Exception e)
            {
                e.GetBaseException();
            }


            return time.ToString() + ", " + string.Format("{0:f6}", _location.Latitude) + ", " + string.Format("{0:f6}", _location.Longitude) + "/n";

            //string path = Android.OS.Environment.ExternalStorageDirectory.Path;
            //string fileName = Path.Combine(path, "myfile.txt");

            //if (!System.IO.File.Exists(fileName))
            //{
            //    using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            //    using (StreamWriter sw = new StreamWriter(fs))
            //    {
            //        sw.WriteLine(time + ", " + _location.Latitude + ", " + _location.Longitude + "/n");
            //    }
            //}


        }

    }
}