using ProductionBoardApi.Models;

namespace ProductionBoardApi.Services;

public interface IProductionBoardService
{
    Task<IReadOnlyList<ProductionBoardResponse>> GetProductionBoardAsync(ProductionBoardRequest request, CancellationToken cancellationToken = default);
}
