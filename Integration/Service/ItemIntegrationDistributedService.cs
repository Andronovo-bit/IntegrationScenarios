using Integration.Common;
using Integration.Backend;
using StackExchange.Redis;
using System;

namespace Integration.Service;

public sealed class ItemIntegrationDistributedService : BaseService
{
    private readonly IItemOperationBackend _itemIntegrationBackend;

    // Redis connection for distributed locks management(As a example)
    private readonly IDatabase _db;

    public ItemIntegrationDistributedService(IItemOperationBackend itemIntegrationBackend, ConnectionMultiplexer redis)
        : base(itemIntegrationBackend)
    {
        _itemIntegrationBackend = itemIntegrationBackend;
        _db = redis.GetDatabase();
    }

    public override Result SaveItem(string itemContent)
    {
        string lockKey = $"lock:item:{itemContent}";
        string lockValue = Guid.NewGuid().ToString();
        TimeSpan expiry = TimeSpan.FromSeconds(30);

        // Get distributed lock
        bool acquired = _db.LockTake(lockKey, lockValue, expiry);

        if (!acquired)
        {
            return new Result(false, $"Another process is handling item with content {itemContent}.");
        }

        try
        {
            if (_itemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
            {
                return new Result(false, $"Duplicate item received with content {itemContent}.");
            }

            var item = _itemIntegrationBackend.SaveItem(itemContent);

            return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
        }
        finally
        {
            _db.LockRelease(lockKey, lockValue);
        }
    }
}
