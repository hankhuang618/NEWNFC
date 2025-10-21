namespace ProductionBoardApi.Models;

public class ProductionBoardResponse
{
    public string DepartmentCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int OnlineCount { get; set; }
    public int ExpectedAttendance { get; set; }
    public int ActualAttendance { get; set; }
    public int BorrowedCount { get; set; }
    public int LentCount { get; set; }
    public int LeaveCount { get; set; }
}
