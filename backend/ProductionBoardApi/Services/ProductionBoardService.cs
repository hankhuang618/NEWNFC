using ProductionBoardApi.Models;
using ProductionBoardApi.Repositories;

namespace ProductionBoardApi.Services;

public class ProductionBoardService : IProductionBoardService
{
    private static readonly IReadOnlyDictionary<string, string> PlantPrefixes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["ZH"] = "ZH",
        ["VN"] = "VN",
        ["TC"] = "TC"
    };

    private readonly IProductionBoardRepository _repository;

    public ProductionBoardService(IProductionBoardRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProductionBoardResponse>> GetProductionBoardAsync(ProductionBoardRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PlantPrefixes.TryGetValue(request.Plant, out var plantPrefix))
        {
            throw new ArgumentException("Unsupported plant specified.", nameof(request.Plant));
        }

        var departmentCode = BuildDepartmentCode(plantPrefix, request.Division, request.Section, request.ClassNumber);
        var records = await _repository.GetProductionBoardAsync(departmentCode, cancellationToken);

        return records.Select(record => new ProductionBoardResponse
        {
            DepartmentCode = record.DepartmentCode,
            DepartmentName = string.IsNullOrWhiteSpace(record.DepartmentName) ? departmentCode : record.DepartmentName,
            OnlineCount = record.OnlineCount,
            ExpectedAttendance = record.ExpectedAttendance,
            ActualAttendance = record.ActualAttendance,
            BorrowedCount = record.BorrowedCount,
            LentCount = record.LentCount,
            LeaveCount = record.LeaveCount
        }).ToList();
    }

    private static string BuildDepartmentCode(string plantPrefix, int division, int section, int classNumber)
    {
        return $"{plantPrefix}-{division}-{section}-{classNumber}";
    }
}
