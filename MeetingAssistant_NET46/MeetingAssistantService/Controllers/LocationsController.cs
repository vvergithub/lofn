using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

using Microsoft.CognitiveServices.SpeechRecognition;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;

using RestSharp;

namespace MeetingAssistant_NET46.Controllers
{
    public class LocationsController : ApiController
    {

        // GET api/locations
        public IEnumerable<string> Get()
        {
            using (var db = new AzureDb("Server=lofndb.database.windows.net;Database=lofn2;User Id=lofn;Password=Passw0rd; "))
            {
                return db.Meetings.Select(i => i.Location).Distinct().ToList();
            }
        }


        public static KeyValuePair<string, string> GetLocation(string location)
        {
            var client = new RestClient(@"http://dev.virtualearth.net");
            try
            {
                LocationResult result = client.Get<LocationResult>(
                    new RestRequest(
                        "/REST/v1/Locations?query=" + location
                        + "&includeNeighborhood=0&include=ciso2&maxResults=1&key=Am-WZOWEstdqEt5JsCZVxGB6fq_BqydoGPCQPgLqPYjswVPh7jN3tW1p0LXypfuw")).Data;

                var coordinates = result.ResourceSets.First().Resources.First().Point.Coordinates.ToArray();
                return new KeyValuePair<string, string>("" + coordinates[0], "" + coordinates[1]);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<string, string>("55.75697", "37.61502");
            }
        }
    }

    public class Point
    {
        public List<double> Coordinates { get; set; }
    }

    public class Resource
    {
        public Point Point { get; set; }
    }

    public class ResourceSet
    {
        public List<Resource> Resources { get; set; }
    }

    public class LocationResult
    {
        public List<ResourceSet> ResourceSets { get; set; }
    }
}
