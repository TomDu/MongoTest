using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoTest
{
    class Test1
    {
        protected static IMongoClient _client = new MongoClient("mongodb://localhost:27017");
        protected static IMongoDatabase _database = _client.GetDatabase("test");

        public static void InsertDocumentDemo()
        {
            var document = new BsonDocument
            {
                { "address" , new BsonDocument
                    {
                        { "street", "2 Avenue" },
                        { "zipcode", "10075" },
                        { "building", "1480" },
                        { "coord", new BsonArray { 73.9557413, 40.7720266 } }
                    }
                },
                { "borough", "Manhattan" },
                { "cuisine", "Italian" },
                { "grades", new BsonArray
                    {
                        new BsonDocument
                        {
                            { "date", new DateTime(2014, 10, 1, 0, 0, 0, DateTimeKind.Utc) },
                            { "grade", "A" },
                            { "score", 11 }
                        },
                        new BsonDocument
                        {
                            { "date", new DateTime(2014, 1, 6, 0, 0, 0, DateTimeKind.Utc) },
                            { "grade", "B" },
                            { "score", 17 }
                        }
                    }
                },
                { "name", "dusizhong" },
                { "restaurant_id", "41704620" }
            };
            var collection = _database.GetCollection<BsonDocument>("restaurants");

            collection.InsertOne(document);

            Console.WriteLine("Done");
        }

        public static void QueryDemo()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            int count = 0;

            using (var cursor = collection.FindSync(new BsonDocument()))
            {
                while (cursor.MoveNext())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        // process document
                        ++count;
                    }
                }
            }

            Console.WriteLine(count);

            var filter = Builders<BsonDocument>.Filter.Eq("name", "dusizhong");
            var res = collection.FindSync(filter).ToList();
            foreach(var ele in res)
            {
                Console.WriteLine(ele.ToString());
            }
        }

        public static void UpdateDemo()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");
            var filter = Builders<BsonDocument>.Filter.Eq("name", "dusizhong");
            var update = Builders<BsonDocument>.Update
                .Set("cuisine", "American(Yoo)")
                .CurrentDate("LastModified");
            var result = collection.UpdateMany(filter, update);

            filter = Builders<BsonDocument>.Filter.Eq("name", "dusizhong");
            var res = collection.FindSync(filter).ToList();
            foreach (var ele in res)
            {
                Console.WriteLine(ele.ToString());
            }
        }

        public static void RemoveDemo()
        {
            var collection = _database.GetCollection<BsonDocument>("restaurants");

            var filter = Builders<BsonDocument>.Filter.Eq("borough", "Manhattan");
            var res = collection.FindSync(filter).ToList();
            foreach (var ele in res)
            {
                Console.WriteLine(ele.ToString());
            }

            var result = collection.DeleteMany(filter);

            Console.WriteLine("---------------------------------------");

            res = collection.FindSync(filter).ToList();
            foreach (var ele in res)
            {
                Console.WriteLine(ele.ToString());
            }
        }
    }
}
