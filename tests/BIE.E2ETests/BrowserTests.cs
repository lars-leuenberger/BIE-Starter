using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace BIE.E2ETests;

[TestClass]
public class BrowserTests : BrowserTestBase
{
    [TestMethod]
    public void TouristAt31DaysIsRejected()
    {
        Fill("Tourist", "Tourismus", "31");
        Driver.FindElement(By.Id("submit")).Click();
        StringAssert.Contains(Decision("rejected").Text, "ABGELEHNT");
    }

    [TestMethod]
    public void AllSixProfilesHavePortraitsAndTransferSecurityFields()
    {
        string[] names = ["Zorg Blubber III", "Nox Taschenuniversum", "Spora Flausch", "Lexa Vielgesicht", "Kira Lichtleiter", "Borin Schaltkreis"];
        for (int i = 0; i < names.Length; i++)
        {
            Assert.AreEqual(1, Driver.FindElements(By.CssSelector(".sample-card")).Count(c => c.Displayed));
            var card = Driver.FindElement(By.CssSelector(".sample-card:not([hidden])"));
            StringAssert.Contains(card.Text, names[i]);
            Wait.Until(d => true.Equals(((IJavaScriptExecutor)d).ExecuteScript("const i=document.getElementById('sample-portrait');return i.complete && i.naturalWidth>0;")));
            Assert.AreEqual("Porträt von " + names[i], Driver.FindElement(By.Id("sample-portrait")).GetDomAttribute("alt"));
            card.FindElement(By.TagName("button")).Click();
            Assert.AreEqual(names[i], Driver.FindElement(By.Id("name")).GetDomProperty("value"));
            Assert.AreEqual(i == 1, Driver.FindElement(By.Id("blackHole")).Selected);
            Assert.AreEqual(i == 2, Driver.FindElement(By.Id("fungus")).Selected);
            Assert.AreEqual(i == 3, Driver.FindElement(By.Id("shapeshifter")).Selected);
            Assert.IsTrue(Driver.FindElement(By.Id("validIdentity")).Selected);
            if (i >= 4)
                Assert.AreEqual("Arbeit", Driver.FindElement(By.Id("purpose")).GetDomProperty("value"));
            Driver.FindElement(By.Id("sample-next")).Click();
        }

        Driver.FindElement(By.Id("sample-prev")).Click();
        StringAssert.Contains(Driver.FindElement(By.Id("sample-position")).Text, "6 / 6");
        Driver.FindElement(By.CssSelector(".sample-card:not([hidden]) button")).Click();
        Driver.FindElement(By.Id("submit")).Click();
        StringAssert.Contains(Decision("approved").Text, "GENEHMIGT");
    }
}
