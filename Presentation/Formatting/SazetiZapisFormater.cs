using Domain.Models;

namespace Presentation.Formatting;

public sealed class SazetiZapisFormater : IZapisFormater
{
    public string Formatiraj(Zapis zapis)
    {
        string tip = zapis is FunkcionalnaGreska ? "Funkcionalna greska" : "Greska interfejsa";
        return $"[{tip}] ID: {zapis.Id} | Naziv: {zapis.Naziv} | Status: {zapis.Status}";
    }
}