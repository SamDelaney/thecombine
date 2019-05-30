using Microsoft.VisualStudio.TestTools.UnitTesting;
using BackendFramework.ValueModels;
using BackendFramework.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using BackendFramework.Context;
using static BackendFramework.Startup;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using NSubstitute;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace Unit_Testing
{
    //MongoTestHelpers is adapted from 
    //http://thoai-nguyen.blogspot.com/2012/06/how-i-mock-mongo-collection.html
    //most of it is magic to me
    //as far as I know, its methods create a MongoCollection<Word> for me
    //that I can *ideally* make a WordService with
    public static class MongoTestHelpers
    {
        private static readonly MongoServerSettings ServerSettings;
        private static readonly MongoServer Server;
        private static readonly MongoDatabaseSettings DatabaseSettings;
        private static readonly MongoDatabase Database;

        
        static MongoTestHelpers()
        {
            ServerSettings = new MongoServerSettings
            {
                Servers = new List<MongoServerAddress> {
                        new MongoServerAddress("unittest")
                    }
            };
            Server = new MongoServer(ServerSettings);
            DatabaseSettings = new MongoDatabaseSettings();
            Database = new MongoDatabase(Server, "databaseName", DatabaseSettings);
        }
        public static MongoCollection<Word> CreateMockCollection<Word>()
        {
            var collectionSettings = new MongoCollectionSettings();
            var m = Substitute.For<MongoCollection<Word>>(Database, "Words", collectionSettings);
            m.Database.Returns(Database);
            m.Settings.Returns(collectionSettings);
            return m;
        }

        public static MongoCollection<Word> ReturnsCollection<Word>(this MongoCollection<Word> collection, IEnumerable<Word> enumerable)
        {
            ReadPreferenceMode mode = (ReadPreferenceMode) 4; //Nearest
            ReadPreference rp = new ReadPreference(mode);
            var cursor = Substitute.For<MongoCursor<Word>>(collection, Substitute.For<IMongoQuery>(), rp, Substitute.For<IBsonSerializer>());
            cursor.GetEnumerator().Returns(enumerable.GetEnumerator());
            cursor.When(x => x.GetEnumerator())
                .Do(callInfo => enumerable.GetEnumerator().Reset());

            cursor.SetSortOrder(Arg.Any<IMongoSortBy>()).Returns(cursor);
            cursor.SetLimit(Arg.Any<int>()).Returns(cursor);
            cursor.SetFields(Arg.Any<IMongoFields>()).Returns(cursor);
            cursor.SetFields(Arg.Any<string[]>()).Returns(cursor);
            cursor.SetFields(Arg.Any<string>()).Returns(cursor);
            cursor.SetSkip(Arg.Any<int>()).Returns(cursor);

            collection.Find(Arg.Any<IMongoQuery>()).Returns(cursor);
            collection.FindAs<Word>(Arg.Any<IMongoQuery>()).Returns(cursor);
            collection.FindAll().Returns(cursor);
            //you properly need to setup more methods of cursor here

            return collection;
        }
    }
    [TestClass]
    public class UnitTests
    {

        public WordService serviceBuilder()
        {
            //using these, this should make a MongoCollection<Word>
            MongoCollection<Word> emcollection = MongoTestHelpers.CreateMockCollection<Word>(); 
            List<Word> wordList = new List<Word>();
            var collection = MongoTestHelpers.ReturnsCollection(emcollection, wordList);
            //Where we are stuck is how to get this to WordService,
            // which takes an IWordContext, that is jsut a wrapper for a collection
            //however this casting gives an error
            IWordContext context = (IWordContext) collection;
            WordService service = new WordService(context);
            return service;

        }

        public async Task<string[]> setUpDatabase(WordService service)
        {
            //if there are any words in the database, we want to delete them
            Task<List<Word>> getTask = service.GetAllWords();
            List<Word> getList = await getTask;
            foreach (Word word in getList)
            {
                bool deleted = await service.Delete(word.Id);
                if (!deleted) throw new System.Exception("Item not deleted!");
            }

            //let's always have these two words in the database
            Word word1 = new Word();
            word1.Vernacular = "One";
            word1.Gloss = 1;
            word1.Audio = "audio1.mp4";
            word1.Timestamp = "1:00";

            Word word2 = new Word();
            word2.Vernacular = "Two";
            word2.Gloss = 2;
            word2.Audio = "audio2.mp4";
            word2.Timestamp = "2:00";

            //since the ids will change every time, I'm going to return them for easy reference
            string[] idList = new string[2];
            word1 = await service.Create(word1);
            word2 = await service.Create(word2);
            idList[0] = word1.Id;
            idList[1] = word2.Id;

            return idList;
        }

        [TestMethod]
        public async Task TestGetAllWords()
        {
            //Test with empty database
            WordService service = serviceBuilder(); //build an empty database

            Task<List<Word>> getTask = service.GetAllWords(); //This is probably how to do async tasks...?
            List<Word> getList = await getTask; //get the actual list of items returned by the action
            Assert.AreEqual(getList.Count, 0); //empty database should have no entries
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => getList[0]); 
            //indexing into an empty list should throw an exception here

            //Test with populated database
            string[] idList = setUpDatabase(service).Result; 
            //populates the database and gives us the ids of the members

            getTask = service.GetAllWords(); 
            getList = await getTask;

            Assert.AreEqual(getList.Count, 2); //this time, there should be two entries

            Word wordInDb1 = getList[0];
            Word wordInDb2 = getList[1];

            //let's check that everything is right about them
            Assert.AreEqual(wordInDb1.Id, idList[0]);
            Assert.AreEqual(wordInDb1.Vernacular, "One");
            Assert.AreEqual(wordInDb1.Gloss, 1);
            Assert.AreEqual(wordInDb1.Audio, "audio1.mp4");
            Assert.AreEqual(wordInDb1.Timestamp, "1:00");

            Assert.AreEqual(wordInDb2.Id, idList[1]);
            Assert.AreEqual(wordInDb2.Vernacular, "Two");
            Assert.AreEqual(wordInDb2.Gloss, 2);
            Assert.AreEqual(wordInDb2.Audio, "audio2.mp4");
            Assert.AreEqual(wordInDb2.Timestamp, "2:00");

            //indexing to the third element should throw an exception
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => getList[2]);

        }


        //the following tests have not yet been configured to be async, 
        //I'm not sure when to await a task

        [TestMethod]
        public async Task TestGetWord()
        {
            //Test with empty database
            WordService service = serviceBuilder(); //build an empty database

            Task<List<Word>> getTask = service.GetWord("111111111111111111111111"); //this is not an id
            List<Word> getList = await getTask;
            Assert.IsNull(getList); //we shouldn't have found anything

            Assert.ThrowsException<System.FormatException>(() => service.GetWord("1"));
            //this is not a valid id, which gives us an exception

            //Test with populated database
            string[] idList = setUpDatabase(service).Result;
            //populates the database and gives us the ids of the members

            string wordId = idList[0];

            getTask = service.GetWord(wordId); //fetch out the first word in the database
            getList = await getTask;
            Word result = getList[0];

            //make sure we got everything correctly
            Assert.AreEqual(result.Vernacular, "One");
            Assert.AreEqual(result.Gloss, 1);
            Assert.AreEqual(result.Audio, "audio1.mp4");
            Assert.AreEqual(result.Timestamp, "1:00");

            getTask = service.GetWord("111111111111111111111111");
            getList = await getTask;
            Assert.IsNull(getList);

            Assert.ThrowsException<System.FormatException>(() => service.GetWord("1"));

        }

        [TestMethod]
        public async Task TestCreate()
        {
            //Test with empty database
            WordService service = serviceBuilder(); //build an empty database

            //TODO: Is there something to test here...?

            //Test with populated database
            setUpDatabase(service);
            //populates the database

            //make a couple of new words, one filled and one empty
            Word newWord1 = new Word();
            newWord1.Vernacular = "Hello";
            newWord1.Gloss = 5;
            newWord1.Audio = "N/A";
            newWord1.Timestamp = "4:30";

            Word emptyWord = new Word();

            //add them
            service.Create(newWord1);
            service.Create(emptyWord);

            //let's see if everything got in there correctly
            Task<List<Word>> getTask = service.GetAllWords();
            List<Word> getList = await getTask;

            Assert.AreEqual(getList.Count, 4);
            Word wordInDb1 = getList[2];
            Word wordInDb2 = getList[3];

            Assert.AreEqual(wordInDb1.Vernacular, "Hello");
            Assert.AreEqual(wordInDb1.Gloss, 5);
            Assert.AreEqual(wordInDb1.Audio, "N/A");
            Assert.AreEqual(wordInDb1.Timestamp, "4:30");

            Assert.IsNull(wordInDb2.Vernacular); //there should be nothing there for the empty word
            Assert.IsNull(wordInDb2.Gloss);
            Assert.IsNull(wordInDb2.Audio);
            Assert.IsNull(wordInDb2.Timestamp);

            //TODO: Perhaps a phoney attribute?
        }

        [TestMethod]
        public async Task TestUpdate()
        {
            //Test with empty database
            WordService service = serviceBuilder(); //build an empty database

            await service.Update("111111111111111111111111");
            //TODO: Assert result WriteResult has error code

            Assert.ThrowsException<System.FormatException>(() => service.Update("1"));

            //Test with populated database
            string[] idList = setUpDatabase(service).Result;
            //populates the database and gives us the ids of the members

            Task<List<Word>> getTask = service.GetWord(idList[0]); //we want to update the first word
            List<Word> getList = await getTask;
            Word wordToUpdate = getList[0];
            wordToUpdate.Vernacular = "Good";

            service.Update(idList[0]); //change it now
            //TODO: Assert result WriteResult has successful code

            //let's check if it worked
            getTask = service.GetWord(idList[0]);
            getList = await getTask;
            Word updatedWord = getList[0];

            Assert.AreEqual(updatedWord.Vernacular, "Good");
            Assert.AreEqual(updatedWord.Gloss, 1);
            Assert.AreEqual(updatedWord.Audio, "audio1.mp4");
            Assert.AreEqual(updatedWord.Timestamp, "1:00");
            Assert.AreEqual(updatedWord.Id, idList[0]);

        }

        [TestMethod]
        public async Task TestDelete()
        {
            //Test with empty database
            WordService service = serviceBuilder();

            service.Delete("111111111111111111111111");
            //TODO: Assert result is NotFoundResult

            Assert.ThrowsException<System.FormatException>(() => service.Delete("1"));
            //this is not a valid id, which gives us an exception

            //Test with populated database
            string[] idList = setUpDatabase(service).Result;
            //populates the database and gives us the ids of the members

            string wordToDelete = idList[0]; 
            //we want to get rid of the first word in the database

            service.Delete(wordToDelete);
            //TODO: Assert result is OkResult

            Task<List<Word>> getTask = service.GetWord(wordToDelete);
            List<Word> getList = await getTask;
            Word result = getList[0];

            Assert.IsNull(result); //it should be gone now

        }

        //[TestMethod]
        //public void TestMerge()
        //{
            
        //    //Test with empty database
        //    WordService service = serviceBuilder();

        //    //Test with populated database
        //    setUpDatabase(service);

        //}
    }
}
