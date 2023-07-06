using Prueba_Tecnica_Steven.Model;

namespace Prueba_Tecnica_Steven.Repository
{
    public interface IClientCollection
    {
        Task InsertClient(Client client);
        Task UpdateClient(Client client);
        Task DeleteClient(string id);
        Task <Client> GetClientById(string id);
        Task<Client> GetClientByNit(string nit);
        Task<List<Client>> GetAllClients();
        Task UpdateBillStatus(string nit, string billId, string newStatus);

        // Task UpdateBill(string billCode);

    }
}
