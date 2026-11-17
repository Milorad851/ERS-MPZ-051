namespace Domain.Models;

public sealed class GreskaInterfejsa : Zapis
{
    public string NazivEkrana { get; init; } = string.Empty;

    public override string Validiraj()
    {
        string osnovnaGreska = base.Validiraj();
        if (!string.IsNullOrEmpty(osnovnaGreska)) return osnovnaGreska;
        if (string.IsNullOrWhiteSpace(NazivEkrana)) return "Naziv ekrana je obavezan.";
        return string.Empty;
    }
}