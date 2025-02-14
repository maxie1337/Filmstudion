# Filmstudion

Denna applikation är byggd med ASP.NET Core, C#, Javascript, html & Bootstrap. Projektet har ett API som hanterar filmer, filmstudios, användare och filmkopior. API:et använder en in-memory-databas (där all data återställs vid omstart) samt JWT-baserad autentisering. Klientsidan är byggd med Javascript, html och bootstrap för en lätthanterlig UI där användaren kan hyra filmer, registrera konton samt lämna tillbaka filmer.

## Krav för att använda applikationen

- .NET 6 eller senare
- Git
- Live server extension

## Så startar du Applikationen

1. Klona repot:
   ```bash
   git clone <repo-url>
   cd <repo-directory>

2. Bygg projektet
   dotnet build

3. Starta API:et
   dotnet run

4. Starta HTML dokumentet med liveserver

## Använd API:et

1. Starta API:et
   dotnet watch

2. Gå in på localhosten och använd scalar
   http://localhost:5146/scalar/v1

3. Nu ser du endpoints och kan testa APIets funktioner (Hoppas allt funkar som det ska)