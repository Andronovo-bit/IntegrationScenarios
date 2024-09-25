using Integration.Backend;
using Integration.Common;

namespace Integration.Service
{
    public abstract class BaseService: IBaseService
    {
        private readonly IItemOperationBackend _itemIntegrationBackend;

        
        public BaseService(IItemOperationBackend itemIntegrationBackend)
        {
            _itemIntegrationBackend = itemIntegrationBackend;
        }
        public List<Item> GetAllItems()
        {
            return _itemIntegrationBackend.GetAllItems();
        }

        public abstract Result SaveItem(string itemContent);
    }
}
