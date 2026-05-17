using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class WizardFormModel : PageModel
{
    public string Message { get; set; } = "";
    public bool Success { get; set; } = false;
    public Dictionary<int, string> Titles { get; set; } = new();
    public Dictionary<int, string> Departments { get; set; } = new();

    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet() => LoadDropdowns();

    public IActionResult OnPost(string FName, string LName, string BDate, int GenderID,
        int DeptID, int TitleID, string StartDate, decimal Wage, decimal CommissionRate, string IsActive,
        string Email, string Phone, string Cell, string City, string Province, string Address)
    {
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            string sql = @"INSERT INTO tbl_Employees
                (Empl_FName, Empl_LName, Empl_BDate, Gender_ID, Dept_ID, Title_ID,
                Empl_Start_Date, Empl_Wage, Empl_Commission_Rate, Is_Empl_Active,
                Empl_Email, Empl_Phone, Empl_Cell, Empl_City, Empl_Province, Empl_Address)
                VALUES (@fn,@ln,@bd,@gid,@did,@tid,@sd,@wg,@cr,@ia,@em,@ph,@ce,@ci,@pr,@ad)";
            using SqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@fn", FName);
            cmd.Parameters.AddWithValue("@ln", LName);
            cmd.Parameters.AddWithValue("@bd", string.IsNullOrEmpty(BDate) ? DBNull.Value : DateTime.Parse(BDate));
            cmd.Parameters.AddWithValue("@gid", GenderID);
            cmd.Parameters.AddWithValue("@did", DeptID);
            cmd.Parameters.AddWithValue("@tid", TitleID);
            cmd.Parameters.AddWithValue("@sd", string.IsNullOrEmpty(StartDate) ? DBNull.Value : DateTime.Parse(StartDate));
            cmd.Parameters.AddWithValue("@wg", Wage);
            cmd.Parameters.AddWithValue("@cr", CommissionRate);
            cmd.Parameters.AddWithValue("@ia", IsActive == "true" ? 1 : 0);
            cmd.Parameters.AddWithValue("@em", (object?)Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ph", (object?)Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ce", (object?)Cell ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ci", (object?)City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pr", (object?)Province ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ad", (object?)Address ?? DBNull.Value);
            cmd.ExecuteNonQuery();
            Success = true;
            Message = $"Employee {FName} {LName} added successfully!";
        }
        catch (Exception ex)
        {
            Message = "Error: " + ex.Message;
        }
        LoadDropdowns();
        return Page();
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
