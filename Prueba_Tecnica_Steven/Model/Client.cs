using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Prueba_Tecnica_Steven.Model
{
    public class Client
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string nit { get; set; }
        public string nombre { get; set; }
        public string ciudad { get; set; }
        public string correo { get; set; }

        public ICollection<Bill> Bills { get; set; }

        

        
    }
}
