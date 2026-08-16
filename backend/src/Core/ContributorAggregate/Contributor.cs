using Core.ContributorAggregate.Events;

namespace Core.ContributorAggregate;

public class Contributor(string name) : EntityBase<Contributor, int>, IAggregateRoot
{
    public string Name { get; private set; } = name;
    public ContributorStatus Status { get; private set; } = ContributorStatus.NotSet;
    public PhoneNumber? PhoneNumber { get; private set; }

    public Contributor UpdatePhoneNumber(PhoneNumber newPhoneNumber)
    {
        PhoneNumber = newPhoneNumber;
        return this;
    }

    public Contributor UpdateName(string newName)
    {
        if (Name == newName) return this;
        Name = newName;
        RegisterDomainEvent(new ContributorNameUpdatedEvent(this));
        return this;
    }
}
