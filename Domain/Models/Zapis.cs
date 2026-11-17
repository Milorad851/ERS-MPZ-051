using System.Text.Json.Serialization;

namespace Domain.Models;

public enum StatusObrade
{
    Otvorena,
    Ispravljena
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$tip")]
[JsonDerivedType(typeof(FunkcionalnaGreska), "funkcionalna")]
[JsonDerivedType(typeof(GreskaInterfejsa), "interfejs")]
public abstract class Zapis
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Naziv { get; init; } = string.Empty;
    public string Opis { get; init; } = string.Empty;
    public string NazivModula { get; init; } = string.Empty;
    public int Prioritet { get; init; }
    public bool PrihvacenaPriDodavanju { get; init; }
    public StatusObrade Status { get; private set; } = StatusObrade.Otvorena;
    public DateTimeOffset KreiranoUtc { get; init; } = DateTimeOffset.UtcNow;

    // Potrebno da bi System.Text.Json mogao da učita Status iz fajla
    [JsonInclude]
    public StatusObrade StatusJson
    {
        get => Status;
        init => Status = value;
    }

    public bool OznaciKaoIspravljenu()
    {
        if (!PrihvacenaPriDodavanju || Status == StatusObrade.Ispravljena)
            return false;

        Status = StatusObrade.Ispravljena;
        return true;
    }

    public bool PonovoOtvori()
    {
        if (Status != StatusObrade.Ispravljena)
            return false;

        Status = StatusObrade.Otvorena;
        return true;
    }

    public virtual string Validiraj()
    {
        if (string.IsNullOrWhiteSpace(Naziv)) return "Naziv ne sme biti prazan.";
        if (string.IsNullOrWhiteSpace(Opis)) return "Opis ne sme biti prazan.";
        if (string.IsNullOrWhiteSpace(NazivModula)) return "Naziv modula ne sme biti prazan.";
        if (Prioritet < 1 || Prioritet > 4) return "Prioritet mora biti izmedju 1 i 4.";
        return string.Empty;
    }
}