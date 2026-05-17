using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class DepartmentEntryModel : PageModel
{
    public string Message { get; set; } = "";
    public bool Success { get; set; } = false;
    public List<DeptItem> Departments { get; set; } = new();
    public DeptItem? EditDept { get; set; }

    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet(int? edit)
{
    if (edit.HasValue) EditDept = LoadDept(edit.Value);
    LoadDepartments();
}

    public IActionResult OnPostAdd(string DeptName, string DeptPhone)
    {
                Message = $"Add called: '{DeptName}'";
            LoadDepartments(); 
            return Page();
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            using SqlCommand cmd = new("INSERT INTO tbl_Departments (Dept_Name, Dept_Phone) VALUES (@n, @p)", conn);
            cmd.Parameters.AddWithValue("@n", DeptName);
            cmd.Parameters.AddWithValue("@p", (object?)DeptPhone ?? DBNull.Value);
            cmd.ExecuteNonQuery();
            Message = $"Department '{DeptName}' added!"; Success = true;
        }
        catch (Exception ex) { Message = "Error: " + ex.Message; }
        LoadDepartments(); return Page();
    }

    public IActionResult OnPostUpdate(int DeptID, string DeptName, string DeptPhone)
    {
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            using SqlCommand cmd = new("UPDATE tbl_Departments SET Dept_Name=@n, Dept_Phone=@p WHERE Dept_ID=@id", conn);
            cmd.Parameters.AddWithValue("@n", DeptName);
            cmd.Parameters.AddWithValue("@p", (object?)DeptPhone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", DeptID);
            cmd.ExecuteNonQuery();
            Message = $"Department updated!"; Success = true;
        }
        catch (Exception ex) { Message = "Error: " + ex.Message; }
        LoadDepartments(); return Page();
    }

    public IActionResult OnPostDelete()
    {
        int DeptID = int.Parse(Request.Query["id"]);
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            using SqlCommand cmd = new("DELETE FROM tbl_Departments WHERE Dept_ID=@id", conn);
            cmd.Parameters.AddWithValue("@id", DeptID);
            cmd.ExecuteNonQuery();
            Message = "Department deleted."; Success = true;
        }
        catch (Exception ex) { Message = "Error: " + ex.Message; }
        LoadDepartments(); return Page();
    }

    private DeptItem? LoadDept(int id)
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        using SqlCommand cmd = new("SELECT Dept_ID, Dept_Name, Dept_Phone FROM tbl_Departments WHERE Dept_ID=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using SqlDataReader r = cmd.ExecuteReader();
        if (r.Read()) return new DeptItem { DeptID = r.GetInt32(0), DeptName = r.GetString(1), DeptPhone = r.IsDBNull(2) ? "" : r.GetString(2).Trim() };
        return null;
    }

    private void LoadDepartments()
{
    try
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        using SqlCommand cmd = new("SELECT Dept_ID, Dept_Name, Dept_Phone FROM tbl_Departments ORDER BY Dept_Name", conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read()) 
            Departments.Add(new DeptItem { 
                DeptID = r.GetInt32(0), 
                DeptName = r.GetString(1), 
                DeptPhone = r.IsDBNull(2) ? "" : r.GetString(2).Trim() 
            });
    }
    catch (Exception ex)
    {
        Message = "DB Error: " + ex.Message;
    }
}
}

public class DeptItem { public int DeptID; public string DeptName = "", DeptPhone = ""; }
