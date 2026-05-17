using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class CommunicationModel : PageModel
{
    public List<CommItem> Employees { get; set; } = new();
    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        string sql = @"SELECT e.Empl_ID, e.Empl_Name, d.Dept_Name, e.Empl_Phone, e.Empl_Cell, e.Empl_Email
                       FROM tbl_Employees e
                       INNER JOIN tbl_Departments d ON e.Dept_ID = d.Dept_ID
                       ORDER BY e.Empl_Name";
        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
            Employees.Add(new CommItem
            {
                EmplID = r.GetInt32(0),
                FullName = r.IsDBNull(1) ? "" : r.GetString(1),
                Department = r.IsDBNull(2) ? "" : r.GetString(2),
                Phone = r.IsDBNull(3) ? "" : r.GetString(3).Trim(),
                Cell = r.IsDBNull(4) ? "" : r.GetString(4).Trim(),
                Email = r.IsDBNull(5) ? "" : r.GetString(5)
            });
    }
}

public class CommItem { public int EmplID; public string FullName = "", Department = "", Phone = "", Cell = "", Email = ""; }
