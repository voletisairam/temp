using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium;
using CSW.Common.Services;
using CSW.Common.Others;
using NYLDDotNetFramework;

namespace CSW.PageObjects.External_Applications
{
    class RiderPage
    {
        private IWebDriver driver;
        private Dictionary<string, string> data;

        public RiderPage(IWebDriver webDriver, Dictionary<string, string> data)
        {
            this.driver = webDriver;
            this.data = data;
            PageFactory.InitElements(webDriver, this);
        }

        [FindsBy(How = How.XPath, Using = "//input[@name='SelectedOffer']/../div")]
        public IList<IWebElement> RiderOffers { get; set; }

        /// <summary>
        /// Method to veriy offers displayed
        /// </summary>
        /// <param name="args"></param>
        public void VerifyOffers(String args = "")
        {
            List<Int32> riderCoverageAmount = new List<Int32>();
            CSW.Common.Services.RestServices restService = new CSW.Common.Services.RestServices(driver, data);
            restService.SubmitRestCall();

            Int32 currentCoverageAmount = Convert.ToInt32(data[KeyRepository.CoverageAmount]);

            //Report mismatgch in rider offer counts
            if (RiderOffers.Count != CSWData.RiderInfo.Count())
                NYLDirectFramework.reportStepResult("Rider offers count do not match", "Rider offer counts displayed in UI does not match with serice result", "Fail");

            //Collect the rider offers
            foreach (List<string> offer in CSWData.RiderInfo)
            {
                Int32 chk = Convert.ToInt32(offer[1]);
                riderCoverageAmount.Add(currentCoverageAmount + Convert.ToInt32(offer[1]));
                  
            }

            //verify Offers



        }

        /// <summary>
        /// Method to select the required offer
        /// </summary>
        /// <param name="args"></param>
        public void SelectOffer(string args = "")
        {

        }

    }
}
