using System;
using System.Collections.Generic;
using System.Text;

namespace FleetwoodMac_Personel.Facade.BusinessLogic.Handlers.Base
{
    public interface IEventHandler<in T, in A>
    {
        void ApplyEvent(T @event, A aggregateRoot);
    }
}
