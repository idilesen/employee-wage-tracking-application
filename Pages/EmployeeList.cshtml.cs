using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class EmployeeListModel : PageModel
{
    public List<EmployeeItem> Employees { get; set; } = new();
    public List<KeyValuePair<int, string>> Departments { get; set; } = new();

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
                       ORDER BY e.Empl_Name";

        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
        {
            Employees.Add(new EmployeeItem
            {
                EmplID = r.GetInt32(0),
                FullName = r.IsDBNull(1) ? "" : r.GetString(1),
                Department = r.IsDBNull(2) ? "" : r.GetString(2),
                Title = r.IsDBNull(3) ? "" : r.GetString(3),
                Email = r.IsDBNull(4) ? "" : r.GetString(4),
                Phone = r.IsDBNull(5) ? "" : r.GetString(5).Trim(),
                StartDate = r.IsDBNull(6) ? "" : r.GetDateTime(6).ToString("yyyy-MM-dd"),
                IsActive = !r.IsDBNull(7) && r.GetBoolean(7)
            });
        }
        r.Close();

        using SqlCommand cmd2 = new("SELECT Dept_ID, Dept_Name FROM tbl_Departments ORDER BY Dept_Name", conn);
        using SqlDataReader r2 = cmd2.ExecuteReader();
        while (r2.Read()) Departments.Add(new KeyValuePair<int, string>(r2.GetInt32(0), r2.GetString(1)));
    }
}

public class EmployeeItem
{
    public int EmplID { get; set; }
    public string FullName { get; set; } = "";
    public string Department { get; set; } = "";
    public string Title { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string StartDate { get; set; } = "";
    public bool IsActive { get; set; }
}