### O projektu

PubSub aplikacija koja simulira **Haotičnog kupidona**. Implementirana je u **ASP.NET Core**.

Server izlaže jedan SignalR hub za osobe koje traže partnera, dok logiku kupidona (matching i periodično slanje pisama) izvršava pozadinski servis.

### Arhitektura i komunikacija

Sistem se sastoji od jednog **ASP.NET Core** servera i više konzolnih klijenata, koji komuniciraju preko **SignalR** hub-a - dvosmerne, real-time komunikacije preko WebSocket-a.

### Kako sistem radi

- Osoba se prijavljuje za traženje partnera preko metode `InitSinglePerson`, unosom username-a, grada, godina i broja telefona preko konzole. Neispravan unos (prazno polje, karakteri umesto brojeva, negativne vrednosti) se prijavljuje odgovarajućom porukom, a zauzet username traži ponovni unos.
- Kupidon svakog minuta šalje po jedno ljubavno pismo svim prijavljenim osobama, na osnovu sledećeg algoritma:
  - ista lokacija = +30 poena
  - slične godine (do 2 godine razlike) = +20 poena
  - nasumični faktor = +0-100 poena (`RNGCryptoServiceProvider`)
  - pismo se šalje osobi sa najvećim ukupnim score-om
- Kada pismo stigne, na konzoli se ispisuju detalji pošiljaoca uz jednu od nasumično odabranih poruka. Za poruku "Nisam zainteresovan/a za upoznavanje." broj telefona se ne prikazuje.
- Korisnik ne prima novo pismo dok ne potvrdi preko konzole (Enter) da je pročitao prethodno.
- Korisnik može blokirati drugog korisnika komandom `/block username`, čime taj korisnik više ne može da mu šalje pisma.

### Tehnologije

ASP.NET Core, SignalR (WebSocket hub), C#, konzolni SignalR klijent.