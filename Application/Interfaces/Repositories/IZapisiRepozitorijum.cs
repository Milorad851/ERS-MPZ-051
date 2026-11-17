using Domain.Models;
using Shared;

namespace Application.Interfaces.Repositories;

public interface IZapisiReadRepozitorijum
{
    Result<Zapis> Pronadji(Guid id);
    Result<IReadOnlyList<Zapis>> Svi();
}

public interface IZapisiWriteRepozitorijum
{
    Result Sacuvaj(Zapis zapis);
    Result Azuriraj(Zapis zapis);
    Result Obrisi(Guid id);
}

public interface IZapisiRepozitorijum : IZapisiReadRepozitorijum, IZapisiWriteRepozitorijum
{
}