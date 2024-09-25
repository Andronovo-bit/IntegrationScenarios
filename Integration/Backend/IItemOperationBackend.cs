using Integration.Common;

namespace Integration.Backend;

public interface IItemOperationBackend
{
    Item SaveItem(string itemContent);
    List<Item> FindItemsWithContent(string itemContent);
    List<Item> GetAllItems();
}
