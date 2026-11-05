using BIE.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BIE.UnitTests;

[TestClass]
public class LearnerUnitTests
{
    [TestMethod]
    public void TouristAt30DaysIsApproved()
    {
        // Arrange
        var entry = new Entry("Sam Test", "Orion", "Tourismus", 30, false, false, false, true);

        // Act
        var result = EntryPolicy.Evaluate(entry);

        // Assert
        Assert.AreEqual("approved", result.Status);
    }
}
