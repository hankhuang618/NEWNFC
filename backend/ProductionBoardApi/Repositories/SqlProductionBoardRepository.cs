using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using ProductionBoardApi.Models;
using ProductionBoardApi.Services;

namespace ProductionBoardApi.Repositories;

public class SqlProductionBoardRepository : IProductionBoardRepository
{
    private readonly ProductionBoardOptions _options;
    private const string Query = @"WITH Attendance AS (

       SELECT Department  AS 部門 , SUM(CASE WHEN show='T' THEN 1 ELSE 0 END)AS 應出席人數, SUM(CASE WHEN STATUS IN('請假','年休','工傷','工傷陪護') THEN 1 ELSE 0 END)  AS 請假人數 
         FROM C_department_user LEFT JOIN H_USER_STATUS ON  C_department_user.jobnumber=H_USER_STATUS.JOB_NUMBER AND H_USER_STATUS.MFG_DAY= CONVERT([varchar](8), GETDATE(), (112)) 
         GROUP BY Department


),

Attendance_ok as(

   select a.department as 部門,count(b.job_number) + SUM(CASE WHEN C.STATUS='借出' THEN 1 ELSE 0 END)as 實際出席
   from C_department_user a left join H_NFC_Trans b on  MFG_Day = CONVERT([varchar](50), GETDATE(), (112)) AND Status = '01' and job_number=jobnumber
   left join H_USER_STATUS C ON  C.MFG_Day = CONVERT([varchar](50), GETDATE(), (112)) AND C.JOB_NUMBER=A.jobnumber
   group by a.department
),
Attendance_real AS (

 select distinct(a.Department) as Department, COALESCE(b.在線人數, 0) AS 在線人數  from C_Seat_Table a left join (
    SELECT 
            Department AS Department, 
            COUNT(job_number)  AS 在線人數
        FROM 
            H_NFC_Trans 
        WHERE 
            MFG_Day = CONVERT([varchar](50), GETDATE(), (112)) AND Status = '01'
        GROUP BY 
           Department
   ) b on a.Department=b.Department
),

Borrowed AS (
    SELECT 
        a.Department  AS Department, 
        COALESCE(COUNT(a.job_number), 0) AS 借入人數
    FROM 
        H_NFC_Trans a
    LEFT JOIN 
        C_department_user b ON a.job_number = b.jobnumber
    WHERE 
        MFG_Day = CONVERT([varchar](50), GETDATE(), (112)) AND a.Status = '01'
        AND   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(a.Department, '-1', ''), '-2', ''), '-3', ''), '-4', ''), '-4', '') != b.department
    GROUP BY 
      a.Department
),
Lent AS (
    SELECT 
 b.department AS department,
        COUNT(a.job_number) AS 借出人數
    FROM 
        H_NFC_Trans a
    LEFT JOIN 
        C_department_user b ON a.job_number = b.jobnumber
    WHERE 
        MFG_Day = CONVERT([varchar](50), GETDATE(), (112)) AND a.Status = '01'
        AND   REPLACE(REPLACE(REPLACE(REPLACE(a.Department, '-1', ''), '-2', ''), '-3', ''), '-4', '') != b.department
    GROUP BY 
 b.department
)
SELECT 
 al.Department,
 al.在線人數,
 A.部門,
  A.應出席人數,
 o.實際出席,
 COALESCE(B.借入人數, 0) AS 借入人數,
 COALESCE(L.借出人數, 0) AS 借出人數,
 A.請假人數

FROM Attendance_real al
FULL join
    Attendance A on REPLACE(REPLACE(REPLACE(REPLACE(Department, '-1', ''), '-2', ''), '-3', ''), '-4', '')=a.部門
FULL JOIN 
    Borrowed B ON al.Department = B.Department
FULL JOIN 
    Lent L ON  REPLACE(REPLACE(REPLACE(REPLACE(al.Department, '-1', ''), '-2', ''), '-3', ''), '-4', '') = L.department
FULL JOIN 
   Attendance_ok  o ON REPLACE(REPLACE(REPLACE(REPLACE(al.Department, '-1', ''), '-2', ''), '-3', ''), '-4', '') = o.部門

WHERE  al.Department= @Department

ORDER BY 
    A.部門";

    public SqlProductionBoardRepository(IOptions<ProductionBoardOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IReadOnlyList<ProductionBoardRecord>> GetProductionBoardAsync(string department, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException("Connection string is not configured.");
        }

        await using var connection = new SqlConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(Query, connection)
        {
            CommandType = CommandType.Text
        };

        command.Parameters.AddWithValue("@Department", department);

        var results = new List<ProductionBoardRecord>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var record = new ProductionBoardRecord
            {
                DepartmentCode = reader["Department"] as string ?? string.Empty,
                DepartmentName = reader["部門"] as string ?? string.Empty,
                OnlineCount = GetSafeInt(reader, "在線人數"),
                ExpectedAttendance = GetSafeInt(reader, "應出席人數"),
                ActualAttendance = GetSafeInt(reader, "實際出席"),
                LeaveCount = GetSafeInt(reader, "請假人數"),
                BorrowedCount = GetSafeInt(reader, "借入人數"),
                LentCount = GetSafeInt(reader, "借出人數")
            };

            results.Add(record);
        }

        return results;
    }

    private static int GetSafeInt(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
    }
}
