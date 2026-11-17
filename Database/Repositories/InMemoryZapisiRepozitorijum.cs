using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Database.Repositories;

public sealed class InMemoryZapisiRepozitorijum : IZapisiRepozitorijum
{
    private readonly List<Zapis> _zapisi = new();

    public Result Sacuvaj(Zapis zapis)
    {
        if (_zapisi.Any(z => z.Id == zapis.Id))
            return Result.Failure("repo.duplikat", "Zapis sa ovim ID-em vec postoji.");

        _zapisi.Add(zapis);
        return Result.Success();
    }

    public Result Azuriraj(Zapis zapis)
    {
        int index = _zapisi.FindIndex(z => z.Id == zapis.Id);
        if (index == -1)
            return Result.Failure("repo.nije_pronadjeno", "Zapis nije pronadjen.");

        _zapisi[index] = zapis;
        return Result.Success();
    }

    public Result<Zapis> Pronadji(Guid id)
    {
        Zapis? zapis = _zapisi.FirstOrDefault(z => z.Id == id);
        return zapis is not null
            ? Result<Zapis>.Success(zapis)
            : Result<Zapis>.Failure("repo.nije_pronadjeno", "Zapis nije pronadjen.");
    }

    public Result<IReadOnlyList<Zapis>> Svi() =>
        Result<IReadOnlyList<Zapis>>.Success(_zapisi.ToList());

    public Result Obrisi(Guid id)
    {
        Zapis? zapis = _zapisi.FirstOrDefault(z => z.Id == id);
        if (zapis is null)
            return Result.Failure("repo.nije_pronadjeno", "Zapis nije pronadjen.");

        _zapisi.Remove(zapis);
        return Result.Success();
    }
}