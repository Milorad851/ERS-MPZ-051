using System.Text.Json;
using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Database.Repositories;

public sealed class JsonZapisiRepozitorijum : IZapisiRepozitorijum
{
    private readonly string _putanja;
    private readonly JsonSerializerOptions _opcije = new() { WriteIndented = true };

    public JsonZapisiRepozitorijum(string putanja = "zapisi.json")
    {
        _putanja = putanja;
    }

    private List<Zapis> UcitajSve()
    {
        if (!File.Exists(_putanja)) return new List<Zapis>();
        string json = File.ReadAllText(_putanja);
        if (string.IsNullOrWhiteSpace(json)) return new List<Zapis>();
        return JsonSerializer.Deserialize<List<Zapis>>(json, _opcije) ?? new List<Zapis>();
    }

    private void SnimiSve(List<Zapis> lista)
    {
        string json = JsonSerializer.Serialize(lista, _opcije);
        File.WriteAllText(_putanja, json);
    }

    public Result Sacuvaj(Zapis zapis)
    {
        var lista = UcitajSve();
        if (lista.Any(z => z.Id == zapis.Id))
            return Result.Failure("repo.duplikat", "Zapis vec postoji.");

        lista.Add(zapis);
        SnimiSve(lista);
        return Result.Success();
    }

    public Result Azuriraj(Zapis zapis)
    {
        var lista = UcitajSve();
        int index = lista.FindIndex(z => z.Id == zapis.Id);
        if (index == -1)
            return Result.Failure("repo.nije_pronadjeno", "Zapis nije pronadjen.");

        lista[index] = zapis;
        SnimiSve(lista);
        return Result.Success();
    }

    public Result<Zapis> Pronadji(Guid id)
    {
        var zapis = UcitajSve().FirstOrDefault(z => z.Id == id);
        return zapis is not null
            ? Result<Zapis>.Success(zapis)
            : Result<Zapis>.Failure("repo.nije_pronadjeno", "Zapis nije pronadjen.");
    }

    public Result<IReadOnlyList<Zapis>> Svi() =>
        Result<IReadOnlyList<Zapis>>.Success(UcitajSve());

    public Result Obrisi(Guid id)
    {
        var lista = UcitajSve();
        var zapis = lista.FirstOrDefault(z => z.Id == id);
        if (zapis is null)
            return Result.Failure("repo.nije_pronadjeno", "Zapis nije pronadjen.");

        lista.Remove(zapis);
        SnimiSve(lista);
        return Result.Success();
    }
}