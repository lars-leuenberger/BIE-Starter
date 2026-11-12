using System.Net;
using System.Net.Http.Json;
using BIE.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bie.IntegrationTests;

[TestClass]
public class LearnerIntegrationTests
{
    [TestMethod]
    public async Task TouristAt30DaysIsSavedAsApproved()
    {
        using var app = new BieFactory();
        using var client = app.CreateClient();
        var entry = new Entry("Sam Test", "Orion", "Tourismus", 30, false, false, false, true);

        var response = await client.PostAsJsonAsync("/api/cases", entry);
        var created = await response.Content.ReadFromJsonAsync<Case>();
        var records = await client.GetFromJsonAsync<List<Case>>("/api/cases");

        // R7: gültiger Fall -> 201 Created, Location zeigt auf den neuen Fall
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(created);
        Assert.IsNotNull(response.Headers.Location);
        Assert.IsTrue(response.Headers.Location.ToString().EndsWith($"/api/cases/{created.Id}"));

        // R3: genau 30 Tage Tourismus sind erlaubt -> approved
        Assert.AreEqual("approved", created.Decision.Status);

        // R7/R8: der Fall ist gespeichert und mit den gesendeten Werten abrufbar
        Assert.IsNotNull(records);
        Assert.AreEqual(1, records.Count);
        var saved = records[0];
        Assert.AreEqual(created.Id, saved.Id);
        Assert.AreEqual("Sam Test", saved.Applicant.Name);
        Assert.AreEqual("Orion", saved.Applicant.Planet);
        Assert.AreEqual("Tourismus", saved.Applicant.Purpose);
        Assert.AreEqual(30, saved.Applicant.Days);
        Assert.AreEqual("approved", saved.Decision.Status);
    }
}
