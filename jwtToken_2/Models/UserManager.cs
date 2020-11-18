using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jwtToken_2.Models
{
    public class UserManager
    {
        public readonly IMongoCollection<User> userCollection;

        public UserManager(string mongoDbConnectionString, string dbName, string collectionName)
        {
            var client = new MongoClient(mongoDbConnectionString);
            var database = client.GetDatabase(dbName);
            userCollection = database.GetCollection<User>(collectionName);
        }

        public void Create(User model)
        {
            userCollection.InsertOne(model);
        }

        public List<User> findAll()
        {
            return userCollection.AsQueryable<User>().ToList();
        }

        public User Find(string id)
        {
            var docId = new ObjectId(id);
            return userCollection.Find<User>(m => m.Id == docId).FirstOrDefault();
        }

        public void update(string id, User user)
        {
            var docId = new ObjectId(id);
            userCollection.ReplaceOne(m => m.Id == docId, user);
        }



    }
}
