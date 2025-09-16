namespace ProperTea.Shared.Domain;

public interface IOrganizationScoped
{
    Guid OrganizationId { get; }
}