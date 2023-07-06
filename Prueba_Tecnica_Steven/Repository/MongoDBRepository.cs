using MongoDB.Driver;

namespace Prueba_Tecnica_Steven.Repository
{
    public class MongoDBRepository
    {
        public MongoClient client;

        public IMongoDatabase db;

        public MongoDBRepository()
        {
            client = new MongoClient("mongodb://localhost:27017");

            db = client.GetDatabase("Prueba_Tecnica");
        }
    }
}
