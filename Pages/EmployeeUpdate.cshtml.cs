using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class EmployeeUpdateModel : PageModel
{
    public string Message { get; set; } = "";
    public bool Success { get; set; } = false;
    public List<EmpItem> Employees { get; set; } = new();
    public EmpDetail? EditEmployee { get; set; }
    public Dictionary<int, string> Titles { get; set; } = new();
    public Dictionary<int, string> Departments { get; set; } = new();

    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;MultipleActiveResultSets=True;";

    public void OnGet(int? edit)
    {
        LoadDropdowns();
        if (edit.HasValue)
            EditEmployee = LoadEmployee(edit.Value);
        else
            LoadEmployees();
    }

    public IActionResult OnPostDelete()
    {
        int id = int.Parse(Request.Query["id"]);
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();

            using SqlCommand cmd1 = new("DELETE FROM tbl_Wages WHERE Empl_ID=@id", conn);
            cmd1.Parameters.AddWithValue("@id", id);
            cmd1.ExecuteNonQuery();

            using SqlCommand cmd2 = new("DELETE FROM tbl_Employees WHERE Empl_ID=@id", conn);
            cmd2.Parameters.AddWithValue("@id", id);
            cmd2.ExecuteNonQuery();

            Message = "Employee deleted successfully.";
            Success = true;
        }
        catch (Exception ex)
        {
            Message = "Error: " + ex.Message;
        }
        LoadDropdowns();
        LoadEmployees();
        return Page();
    }

    public IActionResult OnPostUpdate(int EmplID, string FName, string LName, string BDate, string StartDate,
        string LeftDate, string LeftReason, int GenderID, int TitleID, int DeptID,
        string Email, string Phone, string Cell, string City, string Province, string Address,
        string IsActive, string IsManager, decimal Wage, decimal CommissionRate)
    {
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            string sql = @"UPDATE tbl_Employees SET Empl_FName=@fn, Empl_LName=@ln, Empl_BDate=@bd,
                Empl_Start_Date=@sd, Empl_Left_Date=@ld, Empl_Left_Reason=@lr,
                Gender_ID=@gid, Title_ID=@tid, Dept_ID=@did, Empl_Email=@em,
                Empl_Phone=@ph, Empl_Cell=@ce, Empl_City=@ci, Empl_Province=@pr,
                Empl_Address=@ad, Is_Empl_Active=@ia, Is_Empl_Manager=@im,
                Empl_Wage=@wg, Empl_Commission_Rate=@cr
                WHERE Empl_ID=@id";
            using SqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@fn", FName);
            cmd.Parameters.AddWithValue("@ln", LName);
            cmd.Parameters.AddWithValue("@bd", string.IsNullOrEmpty(BDate) ? DBNull.Value : DateTime.Parse(BDate));
            cmd.Parameters.AddWithValue("@sd", string.IsNullOrEmpty(StartDate) ? DBNull.Value : DateTime.Parse(StartDate));
            cmd.Parameters.AddWithValue("@ld", string.IsNullOrEmpty(LeftDate) ? DBNull.Value : DateTime.Parse(LeftDate));
            cmd.Parameters.AddWithValue("@lr", (object?)LeftReason ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@gid", GenderID);
            cmd.Parameters.AddWithValue("@tid", TitleID);
            cmd.Parameters.AddWithValue("@did", DeptID);
            cmd.Parameters.AddWithValue("@em", (object?)Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ph", (object?)Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ce", (object?)Cell ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ci", (object?)City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pr", (object?)Province ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ad", (object?)Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ia", IsActive == "true" ? 1 : 0);
            cmd.Parameters.AddWithValue("@im", IsManager == "true" ? 1 : 0);
            cmd.Parameters.AddWithValue("@wg", Wage);
            cmd.Parameters.AddWithValue("@cr", CommissionRate);
            cmd.Parameters.AddWithValue("@id", EmplID);
            cmd.ExecuteNonQuery();
            Message = $"{FName} {LName} updated successfully!";
            Success = true;
        }
        catch (Exception ex)
        {
            Message = "Error: " + ex.Message;
        }
        LoadDropdowns();
        LoadEmployees();
        return Page();
    }

    private void LoadEmployees()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        string sql = @"SELECT e.Empl_ID, e.Empl_Name, d.Dept_Name, l.Title, e.Empl_Email, e.Is_Empl_Active
                       FROM tbl_Employees e
                       INNER JOIN tbl_Departments d ON e.Dept_ID = d.Dept_ID
                       INNER JOIN tbl_Lookups l ON e.Title_ID = l.LK_ID
                       ORDER BY e.Empl_Name";
        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
            Employees.Add(new EmpItem { EmplID = r.GetInt32(0), FullName = r.IsDBNull(1) ? "" : r.GetString(1), Department = r.IsDBNull(2) ? "" : r.GetString(2), Title = r.IsDBNull(3) ? "" : r.GetString(3), Email = r.IsDBNull(4) ? "" : r.GetString(4), IsActive = !r.IsDBNull(5) && r.GetBoolean(5) });
    }

    private EmpDetail? LoadEmployee(int id)
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        string sql = @"SELECT Empl_ID, Empl_FName, Empl_LName, Empl_Name, Empl_BDate, Empl_Start_Date,
                       Empl_Left_Date, Empl_Left_Reason, Gender_ID, Title_ID, Dept_ID,
                       Empl_Email, Empl_Phone, Empl_Cell, Empl_City, Empl_Province, Empl_Address,
                       Is_Empl_Active, Is_Empl_Manager, Empl_Wage, Empl_Commission_Rate
                       FROM tbl_Employees WHERE Empl_ID=@id";
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        using SqlDataReader r = cmd.ExecuteReader();
        if (r.Read())
            return new EmpDetail
            {
                EmplID = r.GetInt32(0), FName = r.GetString(1), LName = r.GetString(2),
                FullName = r.IsDBNull(3) ? "" : r.GetString(3),
                BDate = r.IsDBNull(4) ? "" : r.GetDateTime(4).ToString("yyyy-MM-dd"),
                StartDate = r.IsDBNull(5) ? "" : r.GetDateTime(5).ToString("yyyy-MM-dd"),
                LeftDate = r.IsDBNull(6) ? "" : r.GetDateTime(6).ToString("yyyy-MM-dd"),
                LeftReason = r.IsDBNull(7) ? "" : r.GetString(7),
                GenderID = r.GetInt32(8), TitleID = r.GetInt32(9), DeptID = r.GetInt32(10),
                Email = r.IsDBNull(11) ? "" : r.GetString(11),
                Phone = r.IsDBNull(12) ? "" : r.GetString(12).Trim(),
                Cell = r.IsDBNull(13) ? "" : r.GetString(13).Trim(),
                City = r.IsDBNull(14) ? "" : r.GetString(14),
                Province = r.IsDBNull(15) ? "" : r.GetString(15),
                Address = r.IsDBNull(16) ? "" : r.GetString(16),
                IsActive = !r.IsDBNull(17) && r.GetBoolean(17),
                IsManager = !r.IsDBNull(18) && r.GetBoolean(18),
                Wage = r.IsDBNull(19) ? 0 : r.GetDecimal(19),
                CommissionRate = r.IsDBNull(20) ? 0 : (decimal)r.GetDouble(20)
            };
        return null;
    }

    private void LoadDropdowns()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        using SqlCommand cmd1 = new("SELECT LK_ID, Title FROM tbl_Lookups WHERE Title IS NOT NULL", conn);
        using SqlDataReader r1 = cmd1.ExecuteReader();
        while (r1.Read()) Titles[r1.GetInt32(0)] = r1.GetString(1);
        r1.Close();
        using SqlCommand cmd2 = new("SELECT Dept_ID, Dept_Name FROM tbl_Departments", conn);
        using SqlDataReader r2 = cmd2.ExecuteReader();
        while (r2.Read()) Departments[r2.GetInt32(0)] = r2.GetString(1);
    }
}

public class EmpItem { public int EmplID; public string FullName = "", Department = "", Title = "", Email = ""; public bool IsActive; }
public class EmpDetail { public int EmplID, GenderID, TitleID, DeptID; public string FName = "", LName = "", FullName = "", BDate = "", StartDate = "", LeftDate = "", LeftReason = "", Email = "", Phone = "", Cell = "", City = "", Province = "", Address = ""; public bool IsActive, IsManager; public decimal Wage, CommissionRate; }