using ProductionBoardApi.Models;

namespace ProductionBoardApi.Repositories;

public interface IProductionBoardRepository
{
    Task<IReadOnlyList<ProductionBoardRecord>> GetProductionBoardAsync(string department, CancellationToken cancellationToken = default);
}
