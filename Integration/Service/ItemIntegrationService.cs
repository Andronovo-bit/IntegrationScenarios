using Integration.Common;
using Integration.Backend;
using System.Collections.Concurrent;

namespace Integration.Service;

public sealed class ItemIntegrationService : BaseService
{
    private readonly IItemOperationBackend _itemIntegrationBackend;

    // ConcurrentDictionary for holding content-based locks
    private static readonly ConcurrentDictionary<string, object> _locks = new();

    public ItemIntegrationService(IItemOperationBackend itemIntegrationBackend) 
        : base(itemIntegrationBackend)
    {
        _itemIntegrationBackend = itemIntegrationBackend;
    }

    public override Result SaveItem(string itemContent)
    {
        // Get or create a lock object for content
        var lockObject = _locks.GetOrAdd(itemContent, new object());

        lock (lockObject)
        {
            if (_itemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
            {
                return new Result(false, $"Duplicate item received with content {itemContent}.");
            }

            var item = _itemIntegrationBackend.SaveItem(itemContent);

            return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
        }
    }

}
