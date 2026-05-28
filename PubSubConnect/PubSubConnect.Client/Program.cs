using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5251/kupidon")
    .Build();

string username = ReadText("Unesite username: ");
string city = ReadText("Unesite grad: ");
int age = ReadPositiveInt("Unesite godine: ");
string phone = ReadText("Unesite broj telefona: ");

int pendingLetter = 0;

connection.On<LetterDto>("ReceiveLetter", letter =>
{
    Interlocked.Exchange(ref pendingLetter, 1);
    Console.WriteLine("\n--- Primili ste pismo ---");
    Console.WriteLine($"Od:      {letter.SenderUsername}");
    Console.WriteLine($"Grad:    {letter.SenderCity}");
    Console.WriteLine($"Godine:  {letter.SenderAge}");
    if (letter.SenderPhone != null)
        Console.WriteLine($"Telefon: {letter.SenderPhone}");
    Console.WriteLine($"Poruka:  {letter.Message}");
    Console.WriteLine("Pritisnite Enter za potvrdu prijema...");
});

await connection.StartAsync();
await connection.InvokeAsync("InitSinglePerson", username, city, age, phone);

Console.WriteLine("Prijavljeni ste. Cekajte pisma...");
Console.WriteLine("Komanda za blokiranje: /block <username>");

while (true)
{
    string input = Console.ReadLine() ?? string.Empty;

    if (string.IsNullOrEmpty(input) && Interlocked.CompareExchange(ref pendingLetter, 0, 1) == 1)
    {
        await connection.InvokeAsync("ConfirmReceipt");
        Console.WriteLine("Prijem potvrdjen.");
        continue;
    }

    if (input.StartsWith("/block "))
    {
        string toBlock = input.Substring(7).Trim();
        if (!string.IsNullOrEmpty(toBlock))
        {
            await connection.InvokeAsync("BlockUser", toBlock);
            Console.WriteLine($"Korisnik '{toBlock}' je blokiran.");
        }
        else
        {
            Console.WriteLine("Niste naveli korisnika za blokiranje.");
        }
    }
}

static string ReadText(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string value = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine("Polje ne sme biti prazno.");
            continue;
        }

        return value;
    }
}

static int ReadPositiveInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string value = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine("Polje ne sme biti prazno.");
            continue;
        }

        if (!int.TryParse(value, out int result))
        {
            Console.WriteLine("Morate uneti broj, ne karaktere.");
            continue;
        }

        if (result <= 0)
        {
            Console.WriteLine("Broj mora biti pozitivan.");
            continue;
        }

        return result;
    }
}

class LetterDto
{
    public string SenderUsername { get; set; } = string.Empty;
    public string SenderCity { get; set; } = string.Empty;
    public int SenderAge { get; set; }
    public string? SenderPhone { get; set; }
    public string Message { get; set; } = string.Empty;
}