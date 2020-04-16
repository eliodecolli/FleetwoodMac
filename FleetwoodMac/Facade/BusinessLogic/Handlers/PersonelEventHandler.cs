using FleetwoodMac_Personel.Facade.BusinessLogic.Handlers.Base;
using FleetwoodMac_Personel.Facade.Models;
using FleetwoodMac_Personel.Facade.Models.Events;

namespace FleetwoodMac_Personel.Facade.BusinessLogic.Handlers
{
    public class PersonelEventHandler : IEventHandler<UserTaxAddedEvent, PersonelData>, IEventHandler<ContractCompleted, PersonelData>, IEventHandler<ContractAssigned, PersonelData>,
        IEventHandler<ContractUpdated, PersonelData>
    {
        public void ApplyEvent(ContractCompleted @event, PersonelData aggregateRoot)
        {
            if (@event.PersistenceIndex != aggregateRoot.PersistenceIndex)
                throw new System.Exception("Invalid aggregate root.");

            var cur = aggregateRoot.CurrentContract;
            cur.Result = @event.Result;
            cur.IsActive = false;
            cur.ContractStatus = "Completed";

            aggregateRoot.AddPastContract(cur);
            aggregateRoot.SetContract(null);
        }

        public void ApplyEvent(UserTaxAddedEvent @event, PersonelData aggregateRoot)
        {
            if (@event.PersistenceIndex != aggregateRoot.PersistenceIndex)
                throw new System.Exception("Invalid aggregate root.");

            aggregateRoot.AddTax(new Tax() { Amount = @event.Amount, PersistanceIndex = @event.PersistenceIndex });
        }

        public void ApplyEvent(ContractAssigned @event, PersonelData aggregateRoot)
        {
            if (@event.PersistenceIndex != aggregateRoot.PersistenceIndex)
                throw new System.Exception("Invalid aggregate root.");

            aggregateRoot.SetContract(new PersonelContract() { Client = @event.Client, IsActive = true, ContractStatus = "Initial", PersistenceIndex = @event.PersistenceIndex, Result = "" });
        }

        public void ApplyEvent(ContractUpdated @event, PersonelData aggregateRoot)
        {
            if (@event.PersistenceIndex != aggregateRoot.PersistenceIndex)
                throw new System.Exception("Invalid aggregate root.");

            aggregateRoot.CurrentContract.ContractStatus = @event.Status;
        }
    }
}
