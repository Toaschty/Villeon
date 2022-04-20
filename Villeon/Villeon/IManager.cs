using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Components;
using Villeon.Systems;

namespace Villeon
{
    public interface IManager : IUpdate, IRender
    {
        IEntity CreateEntity(string name, Signature signature);

        void RemoveEntity(IEntity entity);

        void RegisterSystem(ISystem system);

        void UnregisterSystem(ISystem system);

    }
}
