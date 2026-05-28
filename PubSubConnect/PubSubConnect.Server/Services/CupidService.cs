using Microsoft.AspNetCore.SignalR;
using PubSubConnect.Server.Hubs;
using PubSubConnect.Server.Models;
using System.Security.Cryptography;

namespace PubSubConnect.Server.Services
{
    public class CupidService : ICupidService, IHostedService, IDisposable
    {
        private readonly IHubContext<CupidHub> _hubContext;
        private readonly List<Person> _persons = new();
        private readonly object _lock = new();
        private Timer? _timer;

        private static readonly string[] Messages =
        {
            "Radujem se nasem susretu!",
            "Zelim da se upoznamo.",
            "Nisam zainteresovan/a za upoznavanje."
        };

        public CupidService(IHubContext<CupidHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(_ => _ = SendLettersAsync(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async Task SendLettersAsync()
        {
            List<(Person recipient, Letter letter)> toSend = new();

            lock (_lock)
            {
                foreach (var recipient in _persons)
                {
                    if (recipient.PendingConfirmation)
                        continue;

                    var candidates = _persons
                        .Where(p => p.ConnectionId != recipient.ConnectionId
                                 && !recipient.BlockedUsers.Contains(p.Username))
                        .ToList();

                    if (!candidates.Any())
                        continue;

                    var best = candidates
                        .Select(p => new { Person = p, Score = CalculateScore(recipient, p) })
                        .OrderByDescending(x => x.Score)
                        .First();

                    var message = Messages[GetRandomInt(0, Messages.Length)];

                    var letter = new Letter
                    {
                        SenderUsername = best.Person.Username,
                        SenderCity = best.Person.City,
                        SenderAge = best.Person.Age,
                        SenderPhone = message == "Nisam zainteresovan/a za upoznavanje." ? null : best.Person.Phone,
                        Message = message
                    };

                    recipient.PendingConfirmation = true;
                    toSend.Add((recipient, letter));
                }
            }

            foreach (var (recipient, letter) in toSend)
            {
                await _hubContext.Clients.Client(recipient.ConnectionId)
                    .SendAsync("ReceiveLetter", letter);
            }
        }

        private int CalculateScore(Person recipient, Person sender)
        {
            int score = 0;

            if (recipient.City.Equals(sender.City, StringComparison.OrdinalIgnoreCase))
                score += 30;

            if (Math.Abs(recipient.Age - sender.Age) <= 2)
                score += 20;

            score += GetRandomInt(0, 101);

            return score;
        }

        #pragma warning disable SYSLIB0023
        private int GetRandomInt(int min, int max)
        {
            using var rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[4];
            rng.GetBytes(bytes);
            int value = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
            return min + (value % (max - min));
        }
        #pragma warning restore SYSLIB0023

        public void RegisterPerson(string connectionId, string username, string city, int age, string phone)
        {
            lock (_lock)
            {
                _persons.Add(new Person
                {
                    ConnectionId = connectionId,
                    Username = username,
                    City = city,
                    Age = age,
                    Phone = phone
                });
            }
        }

        public void ConfirmReceipt(string connectionId)
        {
            lock (_lock)
            {
                var person = _persons.FirstOrDefault(p => p.ConnectionId == connectionId);
                if (person != null)
                    person.PendingConfirmation = false;
            }
        }

        public void BlockUser(string connectionId, string usernameToBlock)
        {
            lock (_lock)
            {
                var person = _persons.FirstOrDefault(p => p.ConnectionId == connectionId);
                person?.BlockedUsers.Add(usernameToBlock);
            }
        }

        public void RemovePerson(string connectionId)
        {
            lock (_lock)
            {
                _persons.RemoveAll(p => p.ConnectionId == connectionId);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
