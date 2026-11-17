# Практични задатак ERS27 — Пријаве програмских грешака (051)

Конзолна апликација за евидентирање и обраду записа у систему „Пријаве програмских грешака“ (`FunkcionalnaGreska` и `GreskaInterfejsa`), реализована по принципима Clean Architecture и SOLID (S1–S5).

## Покретање

Потребан је .NET 8 SDK.

```bash
dotnet restore
dotnet build
dotnet test
# Покретање у меморијском режиму (подразумевано)
dotnet run --project Presentation -- in-memory
# Покретање у JSON режиму (трајно чување у podaci.json)
dotnet run --project Presentation -- json