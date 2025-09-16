namespace ProperTea.Shared.Application;

public interface IUnitOfWork
{
    Task<int> SaveAsync(CancellationToken ct = default);
}