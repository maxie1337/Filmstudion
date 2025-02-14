## REST

Jag har försökt att uppfylla kraven på det API jag har skapat till applikationen har jag designat så gott jag kan efter REST metoden. Jag har använt mig av ASP.NET Core och c# för detta. Jag har delat upp API:et i endpoints där alla har en egen URL och jag har använt de HTTP metoder som jag tror är rätt. Skriver lite nedanför hur jag tänkte om mina klasser och interface:

### Klasser & Interface

Film & IFilm 
  
  Klassen Film representerar en film med egenskaper som `FilmId`, `Title`, `ImageURL`, `Director` och `Year`. Varje film innehåller också en lista på alla filmkopior som hanterar antalet som går att hyra.

FilmCopy & IFilmCopy

  Klassen FilmCopy använder sig av interfacet filmcopy och hanterar en kopia av en film. Den innehåller egenskaper som `FilmCopyId`, `FilmId` (för att koppla kopian till rätt film), `IsRented`, `RentedDate` och `RentedByStudioId`. Detta ska fungera på så sätt att en kopia kan markeras som uthyrd, när den hyrdes ut och vilken studio som hyrt den.

FilmStudio & IFilmStudio 

  Filmstudio-klassen använder sig av interfacet IFilmStudio och representerar en filmstudio med egenskaper som `FilmStudioId`, `Name` och `City`. Den innehåller även en lista med uthyrda filmkopior (`RentedFilmCopies`), vilket gör att en filmstudio kan se vilka filmer som är hyrda.

User & IUser

  Användare (administratörer eller pseudo-användare för filmstudios) representeras av klassen *User*, som implementerar *IUser*. Den innehåller egenskaper som `UserId`, `Username`, `Password` och `Role`, vilket gör det möjligt att hantera autentisering och auktorisering.

DTO

  DTOerna ser till att bara den datan som behövs skickas mellan klient och server, syftet med detta är att försöka förhindra att viss information hålls säker.

### Endpoints

Filmer (FilmsController):
GET /api/films: Returnerar en lista med filmer.
POST /api/films: Lägger till en ny film (endast admins kan göra detta) och skapar ett angivet antal filmkopior.
PATCH/PUT/POST /api/films/{id}: Uppdaterar filminformation (endast admin).
POST /api/films/rent?id={filmId}&studioid={studioId}: Låter en filmstudio hyra en kopia av en film. Endast en kopia per film kan hyras.
POST /api/films/return?id={filmId}&studioid={studioId}: Lämnar tillbaka en uthyrd filmkopia så att den blir tillgänglig igen.

Filmstudior (FilmStudioController):  
POST /api/filmstudio/register: Registrerar en ny filmstudio.
GET /api/filmstudio/filmstudios: Returnerar en lista med filmstudior.
GET /api/filmstudio/{id}: Returnerar detaljerad information om en specifik filmstudio beroende på vilken användare det är som efterfrågar infon.

Användare (UsersController): 
POST /api/users/register: Registrerar en administratör.
POST /api/users/authenticate: Autentiserar en användare (både admins och filmstudios). Vid filmstudio-inloggning får man tillbaka ett pseudo-användarobjekt med filmstudions id, namn, roll och en JWT-token.

Min Studio (MyStudioController):
GET /api/mystudio/rentals: Returnerar en lista med de filmer som den inloggade filmstudion har hyrt.

### REST-Principer

API:et följer REST-principerna genom att:

Använda unika URL:er: Varje resurs (film, filmstudio, användare, filmkopia) har en dedikerad endpoint.

HTTP-metoder: HTTP-metoder används för varje förfrågan (GET för att hämta data, POST för att skapa resurser, PATCH/PUT för att uppdatera data).

Stateless kommunikation: Varje förfrågan innehåller den information som är nödvändig, t.ex. JWT-token i headern, vilket gör API:et stateless.

Auktorisering med JWT: Vissa endpoints har bara användare som är auktoriserade tillgång till, och vissa har endast admins tillgång till. För att göra detta använder jag JWT och Authorize.