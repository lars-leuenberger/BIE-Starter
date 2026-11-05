namespace BIE.Core;

public static class EntryPolicy
{
    public static Verdict Evaluate(Entry entry)
    {
        if (entry.Days < 1 || entry.Days > 365)
            throw new ArgumentOutOfRangeException(nameof(entry));
        if (entry.BlackHole)
            return new("rejected", "§ 04 · Schwarze Löcher gehören nicht ins Handgepäck. Auch keine kleinen.");
        if (entry.Shapeshifter && !entry.ValidIdentity)
            return new("rejected", "§ 09 · Formwandler benötigen einen gültigen Identitätsnachweis.");
        if (entry.Purpose == "Tourismus" && entry.Days > 30)
            return new("rejected", "§ 12 · Touristische Aufenthalte sind auf 30 Tage begrenzt.");
        if (entry.Fungus)
            return new("quarantine", "§ 07 · Weltraumpilz festgestellt. Bitte folgen Sie der gelben Linie zur Quarantäne.");
        return new("approved", "Einreise bewilligt. Verhalten Sie sich unauffällig. Die Menschheit hat bereits genug Fragen.");
    }
}
