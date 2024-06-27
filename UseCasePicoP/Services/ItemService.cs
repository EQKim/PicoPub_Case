using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using UseCasePicoP.MongoDB;
using UseCasePicoP.ClassLibrary;
using UseCasePicoP.Services.Interface;

namespace UseCasePicoP.Services
{
    public class ItemService : IItemService
    {
        private readonly MongoDBAccess _context;

        public ItemService(MongoDBAccess context)
        {
            _context = context;
        }

        public async Task<List<Item>> GetItemsAsync(int pageNumber, int pageSize, string sortBy = null, bool ascending = true)
        {
            var sortDefinition = sortBy switch
            {
                "name" => ascending ? Builders<Item>.Sort.Ascending(i => i.Name) : Builders<Item>.Sort.Descending(i => i.Name),
                "price" => ascending ? Builders<Item>.Sort.Ascending(i => i.Price) : Builders<Item>.Sort.Descending(i => i.Price),
                _ => ascending ? Builders<Item>.Sort.Ascending(i => i.Name) : Builders<Item>.Sort.Descending(i => i.Name),
            };

            return await _context.Items.Find(item => true)
                .Sort(sortDefinition)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }


        public async Task<Item> GetItemByIdAsync(string id)
        {
            return await _context.Items.Find(item => item.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateItemAsync(Item item)
        {
            await _context.Items.InsertOneAsync(item);
        }

        public async Task UpdateItemAsync(string id, Item updatedItem)
        {
            await _context.Items.ReplaceOneAsync(item => item.Id == id, updatedItem);
        }

        public async Task DeleteItemAsync(string id)
        {
            await _context.Items.DeleteOneAsync(item => item.Id == id);
        }

        public async Task<int> GetTotalItemsCountAsync()
        {
            var count = await _context.Items.CountDocumentsAsync(FilterDefinition<Item>.Empty);
            return (int)count;
        }
    }
}
