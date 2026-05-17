using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class WagesByEmpModel : PageModel
{
    public Dictionary<string, List<WageRow>> WageGroups { get; set; } = new();
    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        string sql = @"SELECT e.Empl_Name, w.Wage_Date, l.Month, w.Wage_Amount, w.Wage_Commission, w.Wage_Total, w.Wage_Year
                       FROM tbl_Wages w
                       INNER JOIN tbl_Employees e ON w.Empl_ID = e.Empl_ID
                       INNER JOIN tbl_Lookups l ON w.Month_ID = l.LK_ID
                       ORDER BY e.Empl_Name, w.Wage_Date";
        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
        {
            var name = r.IsDBNull(0) ? "Unknown" : r.GetString(0);
            if (!WageGroups.ContainsKey(name)) WageGroups[name] = new();
            WageGroups[name].Add(new WageRow
            {
                WageDate = r.GetDateTime(1).ToString("yyyy-MM-dd"),
                Month = r.IsDBNull(2) ? "" : r.GetString(2),
                Amount = r.GetDecimal(3), Commission = r.GetDecimal(4),
                Total = r.GetDecimal(5), Year = r.GetInt32(6)
            });
        }
    }
}

public class WageRow { public string WageDate = "", Month = ""; public decimal Amount, Commission, Total; public int Year; }
