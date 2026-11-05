namespace BIE.Core;

public record Entry(string Name, string Planet, string Purpose, int Days, bool BlackHole, bool Fungus, bool Shapeshifter, bool ValidIdentity);
public record Verdict(string Status, string Reason);
public record Case(long Id, Entry Applicant, Verdict Decision);
