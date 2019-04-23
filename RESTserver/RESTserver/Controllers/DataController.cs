using RESTserver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections;

namespace RESTserver.Controllers
{
    public class DataController : ApiController
    {
        // GET: api/Data
        public ArrayList Get()
        {
            DataPersistence items = new DataPersistence();
            
            return items.GetCollection();
        }

        // GET: api/Data/5
        public Items Get(int id)
        {
            DataPersistence item = new DataPersistence();
            

            return item.GetItem(id); 
        }

        // POST: api/Data
        public HttpResponseMessage Post([FromBody]Items value)
        {
            DataPersistence xmlBase = new DataPersistence();
            
            xmlBase.PostItem(value);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, String.Format("Data/"+value.Id.ToString()));

            return response;
        }

        // PUT: api/Data/5
        public HttpResponseMessage Put(int id, [FromBody]Items value)
        {
            bool elementExist = false;
            DataPersistence xmlBase = new DataPersistence();
            elementExist = xmlBase.UpdateItem(id, value);
            HttpResponseMessage response = new HttpResponseMessage();
            if (elementExist)
            {
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return response;

        }

        // DELETE: api/Data/5
        public HttpResponseMessage Delete(int id)
        {
            bool recorExisted;
            DataPersistence xmlBase = new DataPersistence();
            recorExisted = xmlBase.DeleteItem(id);

            HttpResponseMessage response = new HttpResponseMessage();

            if (recorExisted)
            {
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return response;
        }
    }
}
