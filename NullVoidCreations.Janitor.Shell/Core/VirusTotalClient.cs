using System;
using System.Diagnostics;
using System.IO;
using RestSharp;
using RestSharp.Deserializers;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class VirusTotalResponce
    {
        public enum ResponceCode
        {
            NotPresent = -1,
            Error = 0,
            Queued = 1
        }

        #region properties

        [DeserializeAs(Name = "response_code")]
        public ResponceCode ResponseCode { get; set; }

        [DeserializeAs(Name = "verbose_msg")]
        public string Message { get; set; }

        [DeserializeAs(Name = "scan_id")]
        public string ScanId { get; set; }

        [DeserializeAs(Name = "permalink")]
        public string PermanentLink { get; set; }

        [DeserializeAs(Name = "resource")]
        public string Resource { get; set; }

        [DeserializeAs(Name = "sha256")]
        public string Sha256 { get; set; }

        #endregion

        public override string ToString()
        {
            return ScanId;
        }
    }

    class VirusTotalScanReport
    {

        public enum ResponceCode
        {
            Queued = -1,
            Error = -1,
            NotPresent = 0,
            Present = 1
        }

        #region properties

        [DeserializeAs(Name = "response_code")]
        public ResponceCode ResponseCode { get; set; }

        [DeserializeAs(Name = "verbose_msg")]
        public string Message { get; set; }

        [DeserializeAs(Name = "scan_id")]
        public string ScanId { get; set; }

        [DeserializeAs(Name = "permalink")]
        public string PermanentLink { get; set; }

        [DeserializeAs(Name = "resource")]
        public string Resource { get; set; }

        [DeserializeAs(Name = "md5")]
        public string Md5 { get; set; }

        [DeserializeAs(Name = "sha1")]
        public string Sha1 { get; set; }

        [DeserializeAs(Name = "sha256")]
        public string Sha256 { get; set; }

        [DeserializeAs(Name = "scan_date")]
        public DateTime ScanDate { get; set; }

        [DeserializeAs(Name = "positives")]
        public int Positives { get; set; }

        [DeserializeAs(Name = "total")]
        public int Total { get; set; }

        [DeserializeAs(Name = "scans")]
        public Dictionary<string, VirusTotalScanEngine> Result { get; set; }

        #endregion

        public override string ToString()
        {
            return ScanId;
        }
    }

    class VirusTotalScanEngine
    {
        #region properties

        [DeserializeAs(Name = "detected")]
        public bool IsDetected { get; set; }

        [DeserializeAs(Name = "version")]
        public string Version { get; set; }

        [DeserializeAs(Name = "result")]
        public string VirusName { get; set; }

        [DeserializeAs(Name = "update")]
        public string Update { get; set; }

        #endregion
    }

    class VirusTotalClient
    {
        RestClient _client;
        string _apiKey;
        static VirusTotalClient _instance;

        private VirusTotalClient()
        {
            _client = new RestClient();
            _client.BaseUrl = new Uri("https://www.virustotal.com/vtapi/v2/");

            if (Debugger.IsAttached)
                ApiKey = "bbeab7b2a618333584b64fc5a8a4d5b066bd545094bbf820b5f279a31fe52f0a";
        }

        #region properties

        public static VirusTotalClient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VirusTotalClient();

                return _instance;
            }
        }

        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("ApiKey", "API key not specified.");
                if (value.Length != 64)
                    throw new ArgumentException("API key must be 64 characters long.", "ApiKey");
                if (value == _apiKey)
                    return;

                _apiKey = value;
            }
        }

        public bool UseSecureConnection
        {
            get 
            {
                if (_client.BaseUrl.Scheme.Equals(Uri.UriSchemeHttp))
                    return false;
                else if (_client.BaseUrl.Scheme.Equals(Uri.UriSchemeHttps))
                    return true;
                else
                    throw new InvalidOperationException(string.Format("Invalid base URL '{0}'. Virus total only supports HTTP and HTTPS connections.", _client.BaseUrl));
            }
            set
            {
                if (value)
                    _client.BaseUrl = new Uri("https://www.virustotal.com/vtapi/v2/");
                else
                    _client.BaseUrl = new Uri("http://www.virustotal.com/vtapi/v2/");
            }
        }

        #endregion

        void Validate()
        {
            if (string.IsNullOrEmpty(ApiKey))
                throw new ArgumentNullException("ApiKey", "API key not specified.");
        }

        VirusTotalScanReport GetScanReport(params string[] resources)
        {
            Validate();

            var resourceIds = new StringBuilder();
            foreach (var resource in resources)
                resourceIds.Append(resource);

            var request = new RestRequest();
            request.Resource = "file/report";
            request.Method = Method.POST;
            request.AddParameter("apikey", ApiKey);
            request.AddParameter("resource", resourceIds.ToString(0, resourceIds.Length));

            var responce = _client.Execute<VirusTotalScanReport>(request);
            if (responce.ErrorException != null)
                throw responce.ErrorException;

            return responce.Data;
        }

        public VirusTotalScanReport Scan(string fileName)
        {
            Validate();

            var file = new FileInfo(fileName);
            if (!file.Exists)
                throw new FileNotFoundException("File to scan not found.", file.FullName);

            var report = GetScanReport(FileSystemHelper.Instance.GetMD5(fileName));
            if (report.ResponseCode == VirusTotalScanReport.ResponceCode.Present)
                return report;

            var request = new RestRequest();
            request.Resource = "file/scan";
            request.Method = Method.POST;
            request.AddParameter("apikey", ApiKey);
            request.AddFile("file", file.FullName);

            var responce = _client.Execute<VirusTotalResponce>(request);
            if (responce.ErrorException != null)
                throw responce.ErrorException;

            if (responce.Data.ResponseCode == VirusTotalResponce.ResponceCode.Error)
                return null;

            report.ResponseCode = VirusTotalScanReport.ResponceCode.Queued;
            while (report.ResponseCode == VirusTotalScanReport.ResponceCode.Queued)
            {
                Thread.Sleep(20000);
                report = GetScanReport(responce.Data.ScanId);
            }

            return report;
        }
    }
}
