using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;
using Shared;

namespace Application.Services;

public sealed class ZapisServis
{
    private readonly IZapisiWriteRepozitorijum _writeRepo;
    private readonly IZapisiReadRepozitorijum _readRepo;
    private readonly ILoggerServis _logger;

    public ZapisServis(IZapisiWriteRepozitorijum writeRepo, IZapisiReadRepozitorijum readRepo, ILoggerServis logger)
    {
        _writeRepo = writeRepo ?? throw new ArgumentNullException(nameof(writeRepo));
        _readRepo = readRepo ?? throw new ArgumentNullException(nameof(readRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Result<Guid> Dodaj(Zapis zapis)
    {
        if (zapis is null)
        {
            _logger.Loguj("Neuspesno dodavanje: zapis je null.");
            return Result<Guid>.Failure("validacija.null", "Zapis ne sme biti null.");
        }

        string greska = zapis.Validiraj();
        if (!string.IsNullOrEmpty(greska))
        {
            _logger.Loguj($"Neuspesno dodavanje ({zapis.Naziv}): {greska}");
            return Result<Guid>.Failure("validacija.neuspesna", greska);
        }

        Result rezultat = _writeRepo.Sacuvaj(zapis);
        if (rezultat.IsFailure)
        {
            _logger.Loguj($"Greska pri cuvanju zapisa {zapis.Id}: {rezultat.Error?.Message}");
            return Result<Guid>.Failure(rezultat.Error!.Code, rezultat.Error.Message);
        }

        _logger.Loguj($"Uspesno dodat zapis {zapis.Id} ({zapis.Naziv}).");
        return Result<Guid>.Success(zapis.Id);
    }

    public Result OznaciKaoIspravljenu(Guid id)
    {
        var pronadjen = _readRepo.Pronadji(id);
        if (pronadjen.IsFailure)
        {
            _logger.Loguj($"Neuspesno ispravljanje: ID {id} ne postoji.");
            return Result.Failure("zapis.ne_postoji", "Zapis sa zadatim ID-em ne postoji.");
        }

        Zapis zapis = pronadjen.Value!;
        if (!zapis.OznaciKaoIspravljenu())
        {
            _logger.Loguj($"Nedozvoljena promena stanja u Ispravljena za {id}.");
            return Result.Failure("stanje.nedozvoljeno", "Greska nije prihvacena pri dodavanju ili je vec ispravljena.");
        }

        var azurirano = _writeRepo.Azuriraj(zapis);
        if (azurirano.IsSuccess)
            _logger.Loguj($"Zapis {id} oznacen kao ispravljen.");

        return azurirano;
    }

    public Result PonovoOtvori(Guid id)
    {
        var pronadjen = _readRepo.Pronadji(id);
        if (pronadjen.IsFailure)
        {
            _logger.Loguj($"Neuspesno ponovno otvaranje: ID {id} ne postoji.");
            return Result.Failure("zapis.ne_postoji", "Zapis sa zadatim ID-em ne postoji.");
        }

        Zapis zapis = pronadjen.Value!;
        if (!zapis.PonovoOtvori())
        {
            _logger.Loguj($"Nedozvoljeno ponovno otvaranje za {id} (nije bila ispravljena).");
            return Result.Failure("stanje.nedozvoljeno", "Moguce je ponovo otvoriti samo prethodno ispravljenu gresku.");
        }

        var azurirano = _writeRepo.Azuriraj(zapis);
        if (azurirano.IsSuccess)
            _logger.Loguj($"Zapis {id} ponovo otvoren.");

        return azurirano;
    }
}