using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium;
using CSW.PageObjects.External_Applications;

namespace CSW.Drivers
{
    class RidersDriver
    {
        private IWebDriver driver;
        private Dictionary<string, string> data;

        public RidersDriver(IWebDriver webDriver, Dictionary<string, string> data)
        {
            this.driver = webDriver;
            this.data = data;
            PageFactory.InitElements(webDriver, this);
        }

        /// <summary>
        /// Method to login to rider screen
        /// </summary>
        /// <param name="args"></param>
        public void VerifyRiderOffers(string args)
        {
            RiderPage riders = new RiderPage(driver, data);

            //Verify offers displayed
            riders.VerifyOffers();

            //Select offer
            riders.SelectOffer();

        }
    }
}
