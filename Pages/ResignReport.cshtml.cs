using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class ResignReportModel : PageModel
{
    public List<ResignItem> ResignedEmployees { get; set; } = new();
    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        string sql = @"SELECT e.Empl_ID, e.Empl_Name, d.Dept_Name, e.Empl_Start_Date, e.Empl_Left_Date, e.Empl_Left_Reason
                       FROM tbl_Employees e
                       INNER JOIN tbl_Departments d ON e.Dept_ID = d.Dept_ID
                       WHERE e.Empl_Left_Date IS NOT NULL
                       ORDER BY e.Empl_Left_Date DESC";
        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
            ResignedEmployees.Add(new ResignItem
            {
                EmplID = r.GetInt32(0),
                FullName = r.IsDBNull(1) ? "" : r.GetString(1),
                Department = r.IsDBNull(2) ? "" : r.GetString(2),
                StartDate = r.IsDBNull(3) ? "" : r.GetDateTime(3).ToString("yyyy-MM-dd"),
                LeftDate = r.IsDBNull(4) ? "" : r.GetDateTime(4).ToString("yyyy-MM-dd"),
                LeftReason = r.IsDBNull(5) ? "" : r.GetString(5)
            });
    }
}

public class ResignItem { public int EmplID; public string FullName = "", Department = "", StartDate = "", LeftDate = "", LeftReason = ""; }
