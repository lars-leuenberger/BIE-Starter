using System.Net;
using System.Net.Http.Json;
using BIE.Core;
using BIE.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bie.IntegrationTests;

[TestClass]
public class ApiTests
{
    [TestMethod]
    public async Task WorkIsCreatedAndCanBeFetched()
    {
        using var app = new BieFactory();
        using var client = app.CreateClient();
        var response = await client.PostAsJsonAsync("/api/cases", new Entry("  Kira  ", "Kepler-186f", "Arbeit", 90, false, false, false, true));
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var saved = await response.Content.ReadFromJsonAsync<Case>();
        Assert.IsNotNull(saved);
        Assert.AreEqual("Kira", saved.Applicant.Name);
        Assert.AreEqual("approved", saved.Decision.Status);
        Assert.IsTrue(saved.Id > 0);
        Assert.AreEqual(saved, await client.GetFromJsonAsync<Case>(response.Headers.Location));
    }

    [DataTestMethod]
    [DataRow("", "Orion", "Arbeit", 90)]
    [DataRow("  ", "Orion", "Arbeit", 90)]
    [DataRow("Test", "Orion", "Arbeit", 0)]
    [DataRow("Test", "Orion", "Arbeit", 366)]
    [DataRow("Test", "Mars", "Arbeit", 90)]
    [DataRow("Test", "Orion", "Besuch", 90)]
    public async Task InvalidEntryIsNotSaved(string name, string planet, string purpose, int days)
    {
        using var app = new BieFactory();
        using var client = app.CreateClient();
        var response = await client.PostAsJsonAsync("/api/cases", new Entry(name, planet, purpose, days, false, false, false, true));
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        var all = await client.GetFromJsonAsync<List<Case>>("/api/cases");
        Assert.IsNotNull(all);
        Assert.AreEqual(0, all.Count);
    }

    [DataTestMethod]
    [DataRow(true, false, "rejected")]
    [DataRow(false, true, "quarantine")]
    public async Task ValidEntryIsSavedForEveryDecision(bool hole, bool fungus, string expected)
    {
        using var app = new BieFactory();
        using var client = app.CreateClient();
        var response = await client.PostAsJsonAsync("/api/cases", new Entry("Test", "Orion", "Arbeit", 90, hole, fungus, false, true));
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var all = await client.GetFromJsonAsync<List<Case>>("/api/cases");
        Assert.IsNotNull(all);
        Assert.AreEqual(1, all.Count);
        Assert.AreEqual(expected, all[0].Decision.Status);
    }

    [TestMethod]
    public void RegistrySurvivesRestart()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".db");
        try
        {
            var saved = new Registry(path).Add(new("Borin", "Orion", "Arbeit", 180, false, false, false, true));
            Assert.AreEqual(saved, new Registry(path).All().Single());
        }
        finally
        {
            File.Delete(path);
        }
    }
}
