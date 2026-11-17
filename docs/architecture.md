# Архитектура решења

За сваку ставку навести конкретне класе/интерфејсе из сопственог решења и доказ о примени (метод, тест или могућност замене имплементације). Наводи без везе са кодом се не прихватају.

## SRP — једна одговорност

- **Класе и интерфејси:** "Zapis" ("FunkcionalnaGreska", "GreskaInterfejsa"), "ZapisServis", "PregledZapisaServis", "JsonZapisiRepozitorijum", "SazetiZapisFormater", "DetaljniZapisFormater".
- **Доказ о примени:** Доменски модели у "Domain/Models/Zapis.cs" садрже искључиво валидацију ("Validiraj()") и правила прелаза стања ("OznaciKaoIspravljenu()", "PonovoOtvori()"). Сервиси у "Application/Services/" координишу пословне операције и позивају "ILoggerServis", без приступа конзоли или датотекама. Репозиторијуми у "Database/Repositories/" баве се само чувањем података, док класе у "Presentation/Formatting/" искључиво формирају текст за испис.

## OCP — отвореност за проширење

- **Класе и интерфејси:** "IZapisFormater", "SazetiZapisFormater", "DetaljniZapisFormater", "ConsoleMenu".
- **Доказ о примени:** Метода "PrikaziZapise" у "Presentation/Console/ConsoleMenu.cs" ради преко интерфејса "IZapisFormater" ("Formatiraj(Zapis zapis)"). Нови облик приказа додаје се креирањем нове класе која имплементира "IZapisFormater", без измене пословних сервиса "PregledZapisaServis" и "ZapisServis" нити постојећих форматера.

## LSP — заменљивост имплементација

- **Класе и интерфејси:** "IZapisiRepozitorijum", "InMemoryZapisiRepozitorijum", "JsonZapisiRepozitorijum".
- **Доказ о примени:** Обе класе имплементирају исти уговор ("Sacuvaj", "Azuriraj", "Pronadji", "Svi", "Obrisi"), подржавају полиморфну серијализацију и десеријализацију оба типа записа ("FunkcionalnaGreska" и "GreskaInterfejsa" преко "[JsonDerivedType]" атрибута на класи "Zapis") и враћају "Result.Failure("repo.nije_pronadjeno", ...)" за непостојећи ID. Могу се међусобно заменити у "AppContainer"-у без измене иједне линије у "Domain" и "Application".

## ISP — раздвајање интерфејса

- **Класе и интерфејси:** "IZapisiReadRepozitorijum", "IZapisiWriteRepozitorijum", "IZapisiRepozitorijum", "PregledZapisaServis", "ZapisServis".
- **Доказ о примени:** У "Application/Interfaces/Repositories/IZapisiRepozitorijum.cs" уговор је подељен на "IZapisiReadRepozitorijum" ("Pronadji", "Svi") и "IZapisiWriteRepozitorijum" ("Sacuvaj", "Azuriraj", "Obrisi"). Конструктор класе "PregledZapisaServis(IZapisiReadRepozitorijum readRepo)" прима искључиво интерфејс за читање, чиме је компонентама за F3 и F4 онемогућен приступ методама за упис и измену.

## DIP — инверзија зависности

- **Класе и интерфејси:** "AppContainer", "ZapisServis", "PregledZapisaServis", "ILoggerServis", "FileLoggerServis", "ConsoleLoggerServis".
- **Доказ о примени:** Пословни сервиси зависе искључиво од апстракција ("IZapisiReadRepozitorijum", "IZapisiWriteRepozitorijum", "ILoggerServis") које се прослеђују кроз конструктор. У "Presentation/Composition/AppContainer.cs" (метода "Create") бира се конкретан репозиторијум ("InMemoryZapisiRepozitorijum" или "JsonZapisiRepozitorijum") и логер ("FileLoggerServis" или "ConsoleLoggerServis"), без икаквих измена у пројектима "Domain" и "Application".

## Clean Architecture — смер зависности

Presentation ──► Application ──► Domain
                      ▲
                      │
          ┌───────────┴───────────┐
       Database            Infrastructure