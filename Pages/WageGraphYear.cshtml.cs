using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class WageGraphYearModel : PageModel
{
    public Dictionary<int, decimal> YearlyData { get; set; } = new();
    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        using SqlCommand cmd = new("SELECT Wage_Year, SUM(Wage_Total) FROM tbl_Wages GROUP BY Wage_Year ORDER BY Wage_Year", conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read()) YearlyData[r.GetInt32(0)] = r.GetDecimal(1);
    }
}
