using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Models
{
    public class TrackingApiRequest: ApiKeyRequest
    {
        private MethodPeoperties methodProperties;
        public TrackingApiRequest(string documentNumber, string phone = null)
        {
            this.ModelName = "TrackingDocument";
            this.MethodName = "getStatusDocuments";
            this.methodProperties = new MethodPeoperties();
            this.methodProperties.Documents.Add(new TrackingDocument
            {
                DocumentNumber = documentNumber,
                Phone = phone
            });
            this.MethodProperties = this.methodProperties;
        }

        public void AddDocument(string documentNumber, string phone = null)
        {
            this.methodProperties.Documents.Add(new Models.TrackingApiRequest.TrackingDocument
            {
                DocumentNumber = documentNumber,
                Phone = phone
            });
        }

        class MethodPeoperties: MethodPropertiesBase
        {
            public List<TrackingDocument> Documents { get; set; } = new List<TrackingDocument>();
        }

        class TrackingDocument
        {
            public string DocumentNumber { get; set; }
            public string Phone { get; set; }
        }
    }
}
