using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class DeptReportModel : PageModel
{
    public Dictionary<string, List<EmployeeItem>> DepartmentGroups { get; set; } = new();
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }

    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        string sql = @"SELECT e.Empl_ID, e.Empl_Name, d.Dept_Name, l.Title,
                       e.Empl_Email, e.Empl_Phone, e.Empl_Start_Date, e.Is_Empl_Active
                       FROM tbl_Employees e
                       INNER JOIN tbl_Departments d ON e.Dept_ID = d.Dept_ID
                       INNER JOIN tbl_Lookups l ON e.Title_ID = l.LK_ID
                       ORDER BY d.Dept_Name, e.Empl_Name";
        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
        {
            var dept = r.IsDBNull(2) ? "Unknown" : r.GetString(2);
            var emp = new EmployeeItem
            {
                EmplID = r.GetInt32(0), FullName = r.IsDBNull(1) ? "" : r.GetString(1),
                Title = r.IsDBNull(3) ? "" : r.GetString(3),
                Email = r.IsDBNull(4) ? "" : r.GetString(4),
                Phone = r.IsDBNull(5) ? "" : r.GetString(5).Trim(),
                StartDate = r.IsDBNull(6) ? "" : r.GetDateTime(6).ToString("yyyy-MM-dd"),
                IsActive = !r.IsDBNull(7) && r.GetBoolean(7)
            };
            if (!DepartmentGroups.ContainsKey(dept)) DepartmentGroups[dept] = new();
            DepartmentGroups[dept].Add(emp);
        }
        TotalEmployees = DepartmentGroups.Values.Sum(v => v.Count);
        ActiveEmployees = DepartmentGroups.Values.SelectMany(v => v).Count(e => e.IsActive);
    }
}
