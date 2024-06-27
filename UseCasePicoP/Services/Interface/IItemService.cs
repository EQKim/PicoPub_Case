using System.Collections.Generic;
using System.Threading.Tasks;
using UseCasePicoP.ClassLibrary;

namespace UseCasePicoP.Services.Interface
{
    public interface IItemService
    {
        Task<List<Item>> GetItemsAsync(int pageNumber, int pageSize, string sortBy = null, bool ascending = true);
        Task<Item> GetItemByIdAsync(string id);
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(string id, Item updatedItem);
        Task DeleteItemAsync(string id);
        Task<int> GetTotalItemsCountAsync();
    }
}