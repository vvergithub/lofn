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
    public class EmployeesController : ApiController
    {

        // GET api/employees
        public IEnumerable<Employee> Get()
        {
            using (var db = new AzureDb("Server=lofndb.database.windows.net;Database=lofn2;User Id=lofn;Password=Passw0rd; "))
            {
                return db.Employees.ToList();
            }
        }

        // GET api/employees/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/employees
        public Guid Post([FromBody]Employee data) // I know....we should use viewmodels... =O)
        {
            throw new NotImplementedException();
        }

        // PUT api/employees/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/employees/5
        public void Delete(int id)
        {
        }
    }
}
