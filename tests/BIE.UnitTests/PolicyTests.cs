using BIE.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BIE.UnitTests;

[TestClass]
public class PolicyTests
{
    [DataTestMethod]
    [DataRow(1, "approved")]
    [DataRow(29, "approved")]
    [DataRow(31, "rejected")]
    [DataRow(365, "rejected")]
    public void TouristBoundary(int days, string expected) => Assert.AreEqual(expected, EntryPolicy.Evaluate(new("Test", "Orion", "Tourismus", days, false, false, false, true)).Status);
    [DataTestMethod]
    [DataRow(90)]
    [DataRow(180)]
    [DataRow(365)]
    public void WorkIsNotLimitedToThirtyDays(int days) => Assert.AreEqual("approved", EntryPolicy.Evaluate(new("Arbeit", "Orion", "Arbeit", days, false, false, false, true)).Status);
    [TestMethod]
    public void BlackHoleOverridesFungus() => Assert.AreEqual("rejected", EntryPolicy.Evaluate(new("Nox", "Orion", "Diplomatie", 3, true, true, false, true)).Status);
    [TestMethod]
    public void FungusRequiresQuarantine() => Assert.AreEqual("quarantine", EntryPolicy.Evaluate(new("Spora", "Kepler-186f", "Tourismus", 12, false, true, false, true)).Status);
    [TestMethod]
    public void ShapeshifterNeedsIdentity() => Assert.AreEqual("rejected", EntryPolicy.Evaluate(new("Lexa", "Orion", "Tourismus", 10, false, false, true, false)).Status);
    [TestMethod]
    public void RejectionOverridesQuarantine() => Assert.AreEqual("rejected", EntryPolicy.Evaluate(new("Test", "Orion", "Tourismus", 31, false, true, false, true)).Status);
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(366)]
    public void InvalidDurationThrows(int days) => Assert.ThrowsException<ArgumentOutOfRangeException>(() => EntryPolicy.Evaluate(new("Test", "Orion", "Arbeit", days, false, false, false, true)));
}
