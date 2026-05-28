namespace PubSubConnect.Server.Services
{
    public interface ICupidService
    {
        void RegisterPerson(string connectionId, string username, string city, int age, string phone);
        void ConfirmReceipt(string connectionId);
        void BlockUser(string connectionId, string usernameToBlock);
        void RemovePerson(string connectionId);
    }
}
