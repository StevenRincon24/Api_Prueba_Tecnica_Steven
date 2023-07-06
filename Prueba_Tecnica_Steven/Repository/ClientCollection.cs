using MongoDB.Bson;
using MongoDB.Driver;
using Prueba_Tecnica_Steven.Model;

namespace Prueba_Tecnica_Steven.Repository
{
    public class ClientCollection : IClientCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();
        private IMongoCollection<Client> Collection;

        public ClientCollection()
        {
            Collection = _repository.db.GetCollection<Client>("Client");
        }


        public async Task DeleteClient(String id)
        {
            var filter = Builders<Client>.Filter.Eq(c => c.Id, new ObjectId(id));
            await Collection.DeleteOneAsync(filter);
        }

        public async Task<List<Client>> GetAllClients()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task<Client> GetClientById(string id)
        {
            return await Collection.FindAsync(
                new BsonDocument { { "_id", new ObjectId(id) } }).Result.FirstAsync();
        }

        public async Task InsertClient(Client client)
        {
            await Collection.InsertOneAsync(client);
        }

        public async Task UpdateClient(Client client)
        {
            var filter = Builders<Client>.Filter.Eq(c => c.Id, client.Id);
            var update = Builders<Client>.Update
                .Set(c => c.nombre, client.nombre)
                .Set(c => c.nit, client.nit)
                .Set(c => c.nit, client.nit)
                .Set(c => c.ciudad, client.ciudad)
                .Set(c => c.correo, client.correo)
                .Set(c => c.Bills, client.Bills);

            await Collection.UpdateOneAsync(filter, update);
        }
       
        public async Task<Client> GetClientByNit(string nit)
        {
            var filter = Builders<Client>.Filter.Eq(c => c.nit, nit);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> SendEmailToClient(string clientId, string subject, string body)
        {
            var client = await GetClientById(clientId);
            if (client == null)
            {
                return false;
            }

            return true;
        }

        public async Task UpdateBillStatus(string clientNit, string billId, string newStatus)
        {
            var client = await GetClientByNit(clientNit);
            if (client != null)
            {
                var bill = client.Bills.FirstOrDefault(b => b.billCode == billId);
                if (bill != null)
                {
                    bill.billStatus = newStatus;
                    await UpdateClient(client);
                }
            }
        }
    }
}
