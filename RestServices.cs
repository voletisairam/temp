using CSW.Common.Others;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;


namespace CSW.Common.Services
{
    class RestServices
    {
        //ClientID, ClientSecret Credentials
        private static readonly string clientid = "78eeccca04a7487b825bd232a75cca6c";
        private static readonly string clientSecret = "94EF5acae04F48AA95AC98fD6298c3A0";
        public static string endpoint = @"https://modl-api.nylaarp.newyorklife.com";

        private IWebDriver driver;
        private Dictionary<string, string> data;

        public RestServices(IWebDriver webDriver, Dictionary<string, string> testdata)
        {
            driver = webDriver; 
            data = testdata;
            PageFactory.InitElements(webDriver, this);
        }

        //Submit Rest Call using RestSharp
        public void SubmitRestCall(string args = "")
        {
            string GUID = Environment.MachineName + DateTime.Now.ToString("yyyyMMddHHmmss");

            //Create Client Object with URL in Constructor
            var client = new RestClient(endpoint)
            {
                Authenticator = new HttpBasicAuthenticator("motest12", "abcd1234")
            };

            //Set the Request based on the input method
            var request = Request();

            //Get the response by executing the request
            Thread.Sleep(2000);
            RestResponse response = (RestResponse)client.Execute(request);

            if (response.StatusCode.ToString() != "OK")
            {

            }

            //return response.Content.ToString();
            if (GetRestOutput(response.Content, args))
                data[KeyRepository.RiderFlag] = "True";
            else
                data[KeyRepository.RiderFlag] = "False";

        }

        public RestRequest Request()
        {          

            String resource = @"https://modl-api.nylaarp.newyorklife.com/0.1-m/digital/web/offerservice/offers?companyCode=AARP&contractNumber=" + data[KeyRepository.PolicyNumber];

            //Security Credentials
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            //Post Request
            var request = new RestRequest(resource, Method.GET);

            ///// Add Headers and Parameters //////

            //For Digital Apps Only
            request.AddHeader("UserAuthRequest-VendorApp-AppName", "CSW");
            request.AddHeader("UserAuthRequest-UserLoginName", Environment.UserName);

            request.AddHeader("clientid", clientid);
            request.AddHeader("clientSecret", clientSecret);
            request.AddHeader("Connection", "Keep-Alive");
            request.AddHeader("TransRefGUID", "sdfsdfsd");  //Need to be unique
            //request.AddHeader("contractNumber", "");

            request.AddHeader("Content-Type", "application/json");

            return request;
        }

        public bool GetRestOutput(string response, string args)
        {
            ////Parse the whole JSON File
            data[KeyRepository.TempValue] = "";
            bool riderExists = true;
            IList<JToken> offers;
            IList<JToken> riderOfferInfo;
            IList<IList<JToken>> riderDetails = new List<IList<JToken>>();
            JObject firstresponse;
            JObject secondresponse;

            try
            {
                firstresponse = JObject.Parse(response);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return riderExists = false;
            }
            try
            {
                offers = firstresponse["offers"].Children().ToList(); //Here don't see ToList()
            }
            catch (System.NullReferenceException)
            {
                return riderExists = false;
            }


            //Parse the Offers Section
            try
            {
                secondresponse = JObject.Parse(offers[0].ToString());
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return riderExists = false;
            }

            try
            {
                riderOfferInfo = (secondresponse["riderOfferInfo"].Children().ToList())[4].Children().Children().ToList();
            }
            catch (System.NullReferenceException)
            {
                return riderExists = false;
            }


            if (riderExists && args == "")
            {
                data[KeyRepository.TempValue] = secondresponse["imageSrcPath"].ToString();
                data[KeyRepository.RiderUserName] = secondresponse["creativeSpotName"].ToString();

                CSWData.RiderInfo.Clear();
                foreach (var v in riderOfferInfo)
                {
                    riderDetails.Add(v.Children().ToList());
                }
                for (int i = 0; i < riderDetails.Count; i++)
                {
                    CSWData.RiderInfo.Add(new List<string>());
                    for (int j = 0; j < 2; j++)
                    {
                        string c = riderDetails[i][j].ToString().Split(':')[1].Replace("\"", "").Trim();
                        CSWData.RiderInfo[i].Add(c);
                    }
                }
            }
            return riderExists;
        }
    }
}
