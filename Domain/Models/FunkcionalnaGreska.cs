namespace Domain.Models;

public sealed class FunkcionalnaGreska : Zapis
{
    public string KoraciReprodukcije { get; init; } = string.Empty;

    public override string Validiraj()
    {
        string osnovnaGreska = base.Validiraj();
        if (!string.IsNullOrEmpty(osnovnaGreska)) return osnovnaGreska;
        if (string.IsNullOrWhiteSpace(KoraciReprodukcije)) return "Koraci reprodukcije su obavezni.";
        return string.Empty;
    }
}