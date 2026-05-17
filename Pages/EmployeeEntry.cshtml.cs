using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class EmployeeEntryModel : PageModel
{
    public string Message { get; set; } = "";
    public bool Success { get; set; } = false;
    public Dictionary<int, string> Titles { get; set; } = new();
    public Dictionary<int, string> Departments { get; set; } = new();
    public Dictionary<int, string> Cities { get; set; } = new();
    public Dictionary<int, string> Provinces { get; set; } = new();

    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;MultipleActiveResultSets=True;";

    public void OnGet()
    {
        LoadDropdowns();
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

        using SqlCommand cmd3 = new("SELECT LK_ID, City FROM tbl_Lookups WHERE City IS NOT NULL", conn);
        using SqlDataReader r3 = cmd3.ExecuteReader();
        while (r3.Read()) Cities[r3.GetInt32(0)] = r3.GetString(1);
        r3.Close();

        using SqlCommand cmd4 = new("SELECT LK_ID, Province FROM tbl_Lookups WHERE Province IS NOT NULL", conn);
        using SqlDataReader r4 = cmd4.ExecuteReader();
        while (r4.Read()) Provinces[r4.GetInt32(0)] = r4.GetString(1);
        r4.Close();
    }

    public IActionResult OnPost(string FName, string LName, string BDate, string StartDate,
        int GenderID, int TitleID, int DeptID, string Email, string Phone, string Cell,
        string City, string Province, string Address, string IsActive, string CVInfo)
    {
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            string sql = @"INSERT INTO tbl_Employees 
                (Empl_FName, Empl_LName, Empl_BDate, Empl_Start_Date, Gender_ID, Title_ID, 
                Dept_ID, Empl_Email, Empl_Phone, Empl_Cell, Empl_City, Empl_Province, Empl_Address, Is_Empl_Active, Empl_CV)
                VALUES (@fn, @ln, @bd, @sd, @gid, @tid, @did, @em, @ph, @ce, @ci, @pr, @ad, @ia, @cv)";
            using SqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@fn", FName);
            cmd.Parameters.AddWithValue("@ln", LName);
            cmd.Parameters.AddWithValue("@bd", string.IsNullOrEmpty(BDate) ? DBNull.Value : DateTime.Parse(BDate));
            cmd.Parameters.AddWithValue("@sd", string.IsNullOrEmpty(StartDate) ? DBNull.Value : DateTime.Parse(StartDate));
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
            cmd.Parameters.AddWithValue("@cv", (object?)CVInfo ?? DBNull.Value);
            cmd.ExecuteNonQuery();
            Success = true;
            Message = $"Employee {FName} {LName} successfully added!";
        }
        catch (Exception ex)
        {
            Message = "Error: " + ex.Message;
        }
        LoadDropdowns();
        return Page();
    }
}