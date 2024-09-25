using Integration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Service
{
    internal interface IBaseService
    {
        List<Item> GetAllItems();
        Result SaveItem(string itemContent);
    }

}
