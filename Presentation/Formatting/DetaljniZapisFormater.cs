using Domain.Models;

namespace Presentation.Formatting;

public sealed class DetaljniZapisFormater : IZapisFormater
{
    public string Formatiraj(Zapis zapis)
    {
        string tip = zapis is FunkcionalnaGreska ? "Funkcionalna greska" : "Greska interfejsa";
        string specificno = zapis switch
        {
            FunkcionalnaGreska fg => $"Koraci reprodukcije: {fg.KoraciReprodukcije}",
            GreskaInterfejsa gi => $"Naziv ekrana: {gi.NazivEkrana}",
            _ => string.Empty
        };

        return $"[{tip}] ID: {zapis.Id} | Naziv: {zapis.Naziv} | Status: {zapis.Status} | Modul: {zapis.NazivModula} | Prioritet: {zapis.Prioritet} | {specificno} | Opis: {zapis.Opis}";
    }
}