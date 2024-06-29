using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UseCasePicoP.MongoDB;
using UseCasePicoP.ClassLibrary;
using UseCasePicoP.Services.Interface;

namespace UseCasePicoP.Services
{
    public class ItemService : IItemService
    {
        private readonly MongoDBAccess _context;
        private readonly IMemoryCache _cache;
        private readonly List<string> _cacheKeys;

        public ItemService(MongoDBAccess context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
            _cacheKeys = new List<string>();
        }

        public async Task<List<Item>> GetItemsAsync(int pageNumber, int pageSize, string sortBy = null, bool ascending = true)
        {
            string cacheKey = $"{pageNumber}_{pageSize}_{sortBy}_{ascending}";
            
            if (!_cache.TryGetValue(cacheKey, out List<Item> items))
            {
                var sortDefinition = sortBy switch
                {
                    "name" => ascending ? Builders<Item>.Sort.Ascending(i => i.Name) : Builders<Item>.Sort.Descending(i => i.Name),
                    "price" => ascending ? Builders<Item>.Sort.Ascending(i => i.Price) : Builders<Item>.Sort.Descending(i => i.Price),
                    _ => ascending ? Builders<Item>.Sort.Ascending(i => i.Name) : Builders<Item>.Sort.Descending(i => i.Name),
                };

                items = await _context.Items.Find(item => true)
                    .Sort(sortDefinition)
                    .Skip((pageNumber - 1) * pageSize)
                    .Limit(pageSize)
                    .ToListAsync();

                _cache.Set(cacheKey, items, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });

                _cacheKeys.Add(cacheKey); 
            }

            return items;
        }

        public async Task<int> GetTotalItemsCountAsync()
        {
            const string cacheKey = "TotalItemsCount";
            if (!_cache.TryGetValue(cacheKey, out int count))
            {
                count = (int)await _context.Items.CountDocumentsAsync(FilterDefinition<Item>.Empty);

                _cache.Set(cacheKey, count, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });

                _cacheKeys.Add(cacheKey); 
            }

            return count;
        }

        public async Task<Item> GetItemByIdAsync(string id)
        {
            return await _context.Items.Find(item => item.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateItemAsync(Item item)
        {
            await _context.Items.InsertOneAsync(item);
            InvalidateAllCache();
        }

        public async Task UpdateItemAsync(string id, Item updatedItem)
        {
            await _context.Items.ReplaceOneAsync(item => item.Id == id, updatedItem);
            InvalidateAllCache();
        }

        public async Task DeleteItemAsync(string id)
        {
            await _context.Items.DeleteOneAsync(item => item.Id == id);
            InvalidateAllCache();
            
        }

        
        
        
        
        //Helper method
        private void InvalidateAllCache()
        {
            foreach (var key in _cacheKeys)
            {
                _cache.Remove(key);
            }
            _cacheKeys.Clear(); 
        }
    }
}
