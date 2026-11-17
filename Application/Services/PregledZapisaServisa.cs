using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Application.Services;

public sealed class PregledZapisaServis
{
    private readonly IZapisiReadRepozitorijum _readRepo;

    public PregledZapisaServis(IZapisiReadRepozitorijum readRepo)
    {
        _readRepo = readRepo ?? throw new ArgumentNullException(nameof(readRepo));
    }

    public Result<IReadOnlyList<Zapis>> SviZapisi() => _readRepo.Svi();

    public Result<IReadOnlyList<Zapis>> OtvorenePrijave()
    {
        var svi = _readRepo.Svi();
        if (svi.IsFailure) return svi;

        var otvorene = svi.Value!.Where(z => z.Status == StatusObrade.Otvorena).ToList();
        return Result<IReadOnlyList<Zapis>>.Success(otvorene);
    }

    public Result<IReadOnlyList<Zapis>> PretraziPoModulu(string nazivModula)
    {
        var svi = _readRepo.Svi();
        if (svi.IsFailure) return svi;

        if (string.IsNullOrWhiteSpace(nazivModula))
            return Result<IReadOnlyList<Zapis>>.Success(Array.Empty<Zapis>());

        var filtrirani = svi.Value!
            .Where(z => z.NazivModula.Contains(nazivModula.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Result<IReadOnlyList<Zapis>>.Success(filtrirani);
    }
}