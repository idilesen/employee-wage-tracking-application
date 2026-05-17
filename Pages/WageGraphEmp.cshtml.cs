using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class WageGraphEmpModel : PageModel
{
    public Dictionary<string, decimal> EmployeeData { get; set; } = new();
    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        string sql = @"SELECT e.Empl_Name, SUM(w.Wage_Total)
                       FROM tbl_Wages w INNER JOIN tbl_Employees e ON w.Empl_ID = e.Empl_ID
                       GROUP BY e.Empl_Name ORDER BY e.Empl_Name";
        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read()) EmployeeData[r.IsDBNull(0) ? "Unknown" : r.GetString(0)] = r.GetDecimal(1);
    }
}
