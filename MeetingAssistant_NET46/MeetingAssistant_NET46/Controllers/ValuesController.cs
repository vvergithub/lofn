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
using System.Threading.Tasks;

namespace MeetingAssistant_NET46.Controllers
{
    public class CreateMeetingViewModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public List<Guid> ParticipantList { get; set; }

        public CreateMeetingViewModel()
        {
            ParticipantList = new List<Guid>();
        }
    }


    public class ValuesController : ApiController
    {
        private List<string> _parts = new List<string>();

        private Dictionary<string, KeyValuePair<string, string>> _locations = new Dictionary<string, KeyValuePair<string, string>>()
                                                                                  {
            { "Reykjavík", new KeyValuePair<string, string>("64.14741", "-21.93399") },
            { "Florida", new KeyValuePair<string, string>("28.5188", "-81.44426") },
            { "Monterrey", new KeyValuePair<string, string>("25.6828", "-100.3116") },
            { "Stockholm", new KeyValuePair<string, string>("59.33233", "18.06293") },
            { "Moscow", new KeyValuePair<string, string>("55.75697", "37.61502") },
            { "Redmond", new KeyValuePair<string, string>("47.67858", "-122.1316") },
            { "Oslo", new KeyValuePair<string, string>("59.91229", "10.75") },
            { "White house", new KeyValuePair<string, string>("38.89773", "-77.03653") }
        };

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        



        // POST api/values
        public Guid Post([FromBody]CreateMeetingViewModel data) // I know....we should use viewmodels... =O)
        {
            // TODO: Validate for the love of G*d...
            try
            {
                var meeting = new Meeting();
                meeting.Name = data.Name;
                meeting.Location = data.Location;

                meeting.StartDate = DateTime.UtcNow;

                var loc = LocationsController.GetLocation(meeting.Location);
                meeting.Latitude = loc.Key;
                meeting.Longitude = loc.Value;

                using (var db = new AzureDb("Server=lofndb.database.windows.net;Database=lofn2;User Id=lofn;Password=Passw0rd; "))
                {
                    db.Meetings.Add(meeting);

                    bool isOrganizer = true;
                    foreach (var p in data.ParticipantList)
                    {
                        var participant = new Participant { EmployeeId = p, MeetingId = meeting.Id, Organizer = isOrganizer };
                        db.Participants.Add(participant);
                        isOrganizer = false;
                    }

                    db.SaveChanges();
                    // TODO: Handle transaction and failures.
                }

                return meeting.Id;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        /*
        [OperationContract]
        [WebInvoke(Method = "POST",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        UriTemplate = "Upload")]
        public string Upload(Stream stream)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=lofnspeechstorage;AccountKey=zYHSsXwLEuKukhpKjAdWOTSMNCIo4FkiSxV/+GHmJXfDPoZmGe8lqRy42/lDrvXpGO26HHyvdG01uUmHr8AdFg==;";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("files");
            CloudBlob blob = container.GetBlobReference("myblob");

            Stream s = new MemoryStream();
            stream.CopyTo(s);
            s.Seek(0, SeekOrigin.Begin);
            // blob.UploadFromStream(s);

            return "uploaded";
        }
        */

        [HttpPost]
        public void Upload(string dummy)
        {
            try
            {
                using (var stream = Request.Content.ReadAsStreamAsync().Result)
                {
                    string blobName = Guid.NewGuid().ToString("N") + ".wav";

                    /*
                string filename = Guid.NewGuid().ToString("N") + ".wav";
                string blobName = filename;

                string tempPath = HostingEnvironment.MapPath("~/App_Data/" + filename);
                var content = Request.Content.ReadAsByteArrayAsync().Result;
                */

                    // File.WriteAllBytes(tempPath, content);

                    // var ms = new MemoryStream();
                    // this.Request.Content.CopyToAsync(ms);
                    RunSpeechToTextFromStream(stream);
                    // UploadToAzureBlobStorage(blobName, stream);

                }

                /*
                CloudFileClient client = storageAccount.CreateCloudFileClient();

                //Get a reference to the file share we created previously.
                CloudFileShare share = client.GetShareReference("files");

                var listShares = client.ListShares();

                //Ensure that the share exists.
                if (share.Exists())
                {
                    //Get a reference to the root directory for the share.
                    CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                    //Get a reference to the sampledir directory we created previously.
                    CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("recordings");

                    //Ensure that the directory exists.
                    if (sampleDir.Exists())
                    {
                        var test = rootDir.ListFilesAndDirectories();
                        var Credentials = new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("lofnspeechstorage", "7bb47d20e78846c3aa0340cc2e148a85"); // ("accountName", "keyValue");

                        string stringUri = storageAccount.FileStorageUri.PrimaryUri + share.Name + "/recordings/";

                        Uri theUri = new Uri(stringUri);
                        CloudFile cloudFile = new CloudFile(theUri, Credentials);
                        System.IO.FileInfo fi = new System.IO.FileInfo(tempPath);
                        long fileSize = fi.Length;
                        cloudFile.Create(fileSize);
                    }
                }*/

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        void UploadToAzureBlobStorage(string blobName, Stream stream)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=lofnspeechstorage;AccountKey=zYHSsXwLEuKukhpKjAdWOTSMNCIo4FkiSxV/+GHmJXfDPoZmGe8lqRy42/lDrvXpGO26HHyvdG01uUmHr8AdFg==;";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference("files");

            // CloudBlob blob = container.GetBlobReference(blobName);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
            // blob.UploadFromByteArray(content, 0, content.Length);

            blob.UploadFromStreamAsync(stream);
        }

        void RunSpeechToTextFromStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            DataRecognitionClient dataClient = SpeechRecognitionServiceFactory.CreateDataClientWithIntent(
                "en-US",
                "7bb47d20e78846c3aa0340cc2e148a85",
                "LofnLuisBot", // "yourLuisAppID",
                "" // "yourLuisSubsrciptionID"
                );
            dataClient.AuthenticationUri = ""; // this.AuthenticationUri;

            // Event handlers for speech recognition results
            dataClient.OnResponseReceived += DataClient_OnResponseReceived;
            dataClient.OnPartialResponseReceived += DataClient_OnPartialResponseReceived;
            dataClient.OnConversationError += (sender, args) =>
             {

             };

            // Event handler for intent result
            dataClient.OnIntent += (sender, args) =>
            {

            };

            // Note for wave files, we can just send data from the file right to the server.
            // In the case you are not an audio file in wave format, and instead you have just
            // raw data (for example audio coming over bluetooth), then before sending up any 
            // audio data, you must first send up an SpeechAudioFormat descriptor to describe 
            // the layout and format of your raw audio data via DataRecognitionClient's sendAudioFormat() method.
            int bytesRead = 0;
            byte[] buffer = new byte[1024];

            try
            {
                do
                {
                    // Get more Audio data to send into byte buffer.
                    bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // Send of audio data to service. 
                    dataClient.SendAudio(buffer, bytesRead);
                }
                while (bytesRead > 0);
            }
            finally
            {
                // We are done sending audio.  Final recognition results will arrive in OnResponseReceived event call.
                dataClient.EndAudio();
            }
        }

        private void DataClient_OnPartialResponseReceived(object sender, PartialSpeechResponseEventArgs e)
        {
            _parts.Add(e.PartialResult);
        }

        private void DataClient_OnResponseReceived(object sender, SpeechResponseEventArgs e)
        {
            var firstResult = e.PhraseResponse.Results.First();
            _parts.Add(firstResult.LexicalForm);
        }
    }
}
