using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BackendWeb
{
    public class Loadbeat
    {
        private string file;
        public Loadbeat(string file)
        {
            this.file = file;
        }
        public IEnumerable<int> GetBeatTimestamps()
        {
            var lines = File.ReadAllLines( Path.Combine(HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data"),file));
            var i = 0;
           foreach(var line in lines.Select(s => double.Parse(s.Split('\t')[0])))
           {
               if (i%2==1)
                   yield return (int)(line*1000);

               i++;
           }
        }
    }
}