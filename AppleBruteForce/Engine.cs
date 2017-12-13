using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;


namespace AppleBruteForce
{
    public class Engine
    {
        private object GetControlById(RemoteWebDriver driver, string id)
        {
            var xpath = string.Format(@".//*[@id=""{id}""]");
            return driver.FindElementsByXPath(xpath).FirstOrDefault();
        }

        public string Execute()
        {

            ChromeDriver driver = new ChromeDriver();
            try
            {
                var appleId = "foobar@gmail.com";
                var pwd = "foobar";

                driver.Url = "https://appleid.apple.com/#!&page=signin";
                driver.SwitchTo().Frame(0);
                
                System.Threading.Thread.Sleep(1000);

                var idControl = driver.FindElementsByXPath(@".//*[@id=""appleId""]").FirstOrDefault();
                idControl.SendKeys(appleId);

                var pwdControl = driver.FindElementsByXPath(@".//*[@id=""pwd""]").FirstOrDefault();
                pwdControl.SendKeys(pwd);

                var buttonXpath = @".//*[@id=""sign-in""]";
                var button = driver.FindElementsByXPath(buttonXpath).FirstOrDefault();
                button.Click();
                
                System.Threading.Thread.Sleep(1000);

                var ntdpXpath = @".//*[@id=""no-trstd-device-pop""]";
                var ntdpLInk = driver.FindElementsByXPath(ntdpXpath).FirstOrDefault();
                ntdpLInk.Click();

                System.Threading.Thread.Sleep(1000);
                
                var needHelpXpath = @".//*[@id=""need-help-link""]";
                //var needHelpXpath = @".//*[@text=""Need Help?""]";
                var needHelp = driver.FindElementsByXPath(needHelpXpath).FirstOrDefault();
                needHelp.Click();

                var allXpath = @".//*";
                var allControls = driver.FindElementsByXPath(allXpath);
                var tags = allControls.Select(c => c.TagName).ToArray();
                var texts = allControls.Select(c => c.Text).ToArray();

                System.Threading.Thread.Sleep(1000);

                var recoveryXpath = @".//*[@id=""acc-recovery""]";
                var recovery = driver.FindElementsByXPath(recoveryXpath).FirstOrDefault();
                recovery.Click();

                var tabs = driver.WindowHandles.ToArray();

                driver.SwitchTo().Window(tabs[1]);

                var phoneNumberXpath = @".//*[@id=""phoneNumber""]";
                var phoneNumber = driver.FindElementsByXPath(phoneNumberXpath).FirstOrDefault();
                phoneNumber.SendKeys("07979536654");
                
                
                //< button type = "button" id = "action" class="right-nav button last" can-click="{navigation.buttons.action.callback}" role="button">Continue</button>

    }
            finally
            {
                driver.Close();
            }
            return "xxx";
        }

        public void WriteOutControls(IEnumerable<IWebElement> elements, string elementId)
        {
            var ids =
            elements.ToArray()
                .Where(el => B(el, elementId))
                .Select(el => el.GetAttribute(elementId));

            foreach (var id in ids)
            {
                Debug.WriteLine(id);
            }
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
