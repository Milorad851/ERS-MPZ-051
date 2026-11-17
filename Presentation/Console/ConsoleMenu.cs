using Domain.Models;
using Presentation.Composition;
using Presentation.Formatting;
using Presentation.Interfaces;

namespace Presentation.Console;

public sealed class ConsoleMenu
{
    private readonly IConsoleIO _io;
    private readonly AppContainer _container;

    public ConsoleMenu(IConsoleIO io, AppContainer container)
    {
        _io = io;
        _container = container;
    }

    public void Run()
    {
        while (true)
        {
            _io.WriteLine("\n=== PRIJAVE PROGRAMSKIH GRESAKA ===");
            _io.WriteLine("1. Dodaj funkcionalnu gresku (F1)");
            _io.WriteLine("2. Dodaj gresku interfejsa (F2)");
            _io.WriteLine("3. Prikazi sve zapise (F3)");
            _io.WriteLine("4. Prikazi otvorene prijave (F3)");
            _io.WriteLine("5. Pretraga po nazivu modula (F4)");
            _io.WriteLine("6. Oznaci gresku kao ispravljenu (F5)");
            _io.WriteLine("7. Ponovo otvori gresku (F6)");
            _io.WriteLine("0. Izlaz");
            _io.Write("Izbor: ");

            string izbor = _io.ReadLine().Trim();
            switch (izbor)
            {
                case "1": DodajFunkcionalnuGresku(); break;
                case "2": DodajGreskuInterfejsa(); break;
                case "3": PrikaziZapise(samoOtvorene: false); break;
                case "4": PrikaziZapise(samoOtvorene: true); break;
                case "5": PretraziPoModulu(); break;
                case "6": OznaciKaoIspravljenu(); break;
                case "7": PonovoOtvori(); break;
                case "0": return;
                default: _io.WriteLine("Nepoznata opcija."); break;
            }
        }
    }

    private (string naziv, string opis, string modul, int prioritet, bool prihvacena) UnesiOsnovno()
    {
        _io.Write("Naziv: ");
        string naziv = _io.ReadLine();

        _io.Write("Opis: ");
        string opis = _io.ReadLine();

        _io.Write("Naziv modula: ");
        string modul = _io.ReadLine();

        _io.Write("Prioritet (1-4): ");
        int.TryParse(_io.ReadLine(), out int prioritet);

        _io.Write("Prihvacena pri dodavanju (da/ne): ");
        string prih = _io.ReadLine().Trim();
        bool prihvacena = prih.Equals("da", StringComparison.OrdinalIgnoreCase);

        return (naziv, opis, modul, prioritet, prihvacena);
    }

    private void DodajFunkcionalnuGresku()
    {
        var (naziv, opis, modul, prioritet, prihvacena) = UnesiOsnovno();
        _io.Write("Koraci reprodukcije: ");
        string koraci = _io.ReadLine();

        var zapis = new FunkcionalnaGreska
        {
            Naziv = naziv,
            Opis = opis,
            NazivModula = modul,
            Prioritet = prioritet,
            PrihvacenaPriDodavanju = prihvacena,
            KoraciReprodukcije = koraci
        };

        var rezultat = _container.Servis.Dodaj(zapis);
        if (rezultat.IsSuccess)
            _io.WriteLine($"Uspesno dodato. Dodeljeni ID: {rezultat.Value}");
        else
            _io.WriteLine($"Greska: {rezultat.Error?.Message}");
    }

    private void DodajGreskuInterfejsa()
    {
        var (naziv, opis, modul, prioritet, prihvacena) = UnesiOsnovno();
        _io.Write("Naziv ekrana: ");
        string ekran = _io.ReadLine();

        var zapis = new GreskaInterfejsa
        {
            Naziv = naziv,
            Opis = opis,
            NazivModula = modul,
            Prioritet = prioritet,
            PrihvacenaPriDodavanju = prihvacena,
            NazivEkrana = ekran
        };

        var rezultat = _container.Servis.Dodaj(zapis);
        if (rezultat.IsSuccess)
            _io.WriteLine($"Uspesno dodato. Dodeljeni ID: {rezultat.Value}");
        else
            _io.WriteLine($"Greska: {rezultat.Error?.Message}");
    }

    private void PrikaziZapise(bool samoOtvorene)
    {
        var rezultat = samoOtvorene
            ? _container.PregledServis.OtvorenePrijave()
            : _container.PregledServis.SviZapisi();

        if (rezultat.IsFailure)
        {
            _io.WriteLine($"Greska: {rezultat.Error?.Message}");
            return;
        }

        if (rezultat.Value!.Count == 0)
        {
            _io.WriteLine("Nema evidentiranih zapisa.");
            return;
        }

        _io.Write("Format prikaza (1 = sazeti, 2 = detaljni): ");
        string formatIzbor = _io.ReadLine().Trim();
        IZapisFormater formater = formatIzbor == "2"
            ? new DetaljniZapisFormater()
            : new SazetiZapisFormater();

        foreach (var zapis in rezultat.Value)
        {
            _io.WriteLine(formater.Formatiraj(zapis));
        }
    }

    private void PretraziPoModulu()
    {
        _io.Write("Unesite naziv modula: ");
        string modul = _io.ReadLine();

        var rezultat = _container.PregledServis.PretraziPoModulu(modul);
        if (rezultat.IsFailure)
        {
            _io.WriteLine($"Greska: {rezultat.Error?.Message}");
            return;
        }

        if (rezultat.Value!.Count == 0)
        {
            _io.WriteLine("Nema rezultata za zadati naziv modula.");
            return;
        }

        IZapisFormater formater = new DetaljniZapisFormater();
        foreach (var zapis in rezultat.Value)
        {
            _io.WriteLine(formater.Formatiraj(zapis));
        }
    }

    private void OznaciKaoIspravljenu()
    {
        _io.Write("Unesite ID greske: ");
        if (!Guid.TryParse(_io.ReadLine().Trim(), out Guid id))
        {
            _io.WriteLine("Neispravan format ID-a.");
            return;
        }

        var rezultat = _container.Servis.OznaciKaoIspravljenu(id);
        _io.WriteLine(rezultat.IsSuccess
            ? "Greska je uspesno oznacena kao ispravljena."
            : $"Greska: {rezultat.Error?.Message}");
    }

    private void PonovoOtvori()
    {
        _io.Write("Unesite ID greske: ");
        if (!Guid.TryParse(_io.ReadLine().Trim(), out Guid id))
        {
            _io.WriteLine("Neispravan format ID-a.");
            return;
        }

        var rezultat = _container.Servis.PonovoOtvori(id);
        _io.WriteLine(rezultat.IsSuccess
            ? "Greska je ponovo otvorena."
            : $"Greska: {rezultat.Error?.Message}");
    }
}