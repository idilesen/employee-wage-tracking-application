using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class WageUpdateModel : PageModel
{
    public string Message { get; set; } = "";
    public bool Success { get; set; } = false;
    public List<WageItem> Wages { get; set; } = new();
    public WageItem? EditWage { get; set; }
    public Dictionary<int, string> Employees { get; set; } = new();
    public Dictionary<int, string> Months { get; set; } = new();

    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet(int? edit)
    {
        LoadDropdowns();
        if (edit.HasValue) EditWage = LoadWage(edit.Value);
        else LoadWages();
    }

    public IActionResult OnPostDelete()
    {
        int id = int.Parse(Request.Query["id"]);
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            using SqlCommand cmd = new("DELETE FROM tbl_Wages WHERE Wage_ID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            Message = "Wage record deleted."; Success = true;
        }
        catch (Exception ex) { Message = "Error: " + ex.Message; }
        LoadDropdowns(); LoadWages(); return Page();
    }

    public IActionResult OnPostUpdate(int WageID, int EmplID, string WageDate, decimal WageAmount, decimal WageCommission, int MonthID)
    {
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            string sql = "UPDATE tbl_Wages SET Empl_ID=@eid, Wage_Date=@wd, Wage_Amount=@wa, Wage_Commission=@wc, Month_ID=@mid WHERE Wage_ID=@id";
            using SqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@eid", EmplID);
            cmd.Parameters.AddWithValue("@wd", DateTime.Parse(WageDate));
            cmd.Parameters.AddWithValue("@wa", WageAmount);
            cmd.Parameters.AddWithValue("@wc", WageCommission);
            cmd.Parameters.AddWithValue("@mid", MonthID);
            cmd.Parameters.AddWithValue("@id", WageID);
            cmd.ExecuteNonQuery();
            Message = "Wage record updated successfully!"; Success = true;
        }
        catch (Exception ex) { Message = "Error: " + ex.Message; }
        LoadDropdowns(); LoadWages(); return Page();
    }

    private void LoadWages()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        string sql = @"SELECT w.Wage_ID, e.Empl_Name, w.Wage_Date, w.Wage_Amount, w.Wage_Commission, w.Wage_Total, w.Wage_Year, w.Empl_ID, w.Month_ID
                       FROM tbl_Wages w INNER JOIN tbl_Employees e ON w.Empl_ID = e.Empl_ID ORDER BY w.Wage_Date DESC";
        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader r = cmd.ExecuteReader();
        while (r.Read())
            Wages.Add(new WageItem { WageID = r.GetInt32(0), EmployeeName = r.IsDBNull(1) ? "" : r.GetString(1), WageDate = r.GetDateTime(2).ToString("yyyy-MM-dd"), Amount = r.GetDecimal(3), Commission = r.GetDecimal(4), Total = r.GetDecimal(5), Year = r.GetInt32(6), EmplID = r.GetInt32(7), MonthID = r.GetInt32(8) });
    }

    private WageItem? LoadWage(int id)
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        using SqlCommand cmd = new("SELECT Wage_ID, Empl_ID, Wage_Date, Wage_Amount, Wage_Commission, Wage_Total, Wage_Year, Month_ID FROM tbl_Wages WHERE Wage_ID=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using SqlDataReader r = cmd.ExecuteReader();
        if (r.Read()) return new WageItem { WageID = r.GetInt32(0), EmplID = r.GetInt32(1), WageDate = r.GetDateTime(2).ToString("yyyy-MM-dd"), Amount = r.GetDecimal(3), Commission = r.GetDecimal(4), Total = r.GetDecimal(5), Year = r.GetInt32(6), MonthID = r.GetInt32(7) };
        return null;
    }

    private void LoadDropdowns()
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        using SqlCommand cmd1 = new("SELECT Empl_ID, Empl_Name FROM tbl_Employees ORDER BY Empl_Name", conn);
        using SqlDataReader r1 = cmd1.ExecuteReader();
        while (r1.Read()) Employees[r1.GetInt32(0)] = r1.IsDBNull(1) ? "" : r1.GetString(1);
        r1.Close();
        using SqlCommand cmd2 = new("SELECT LK_ID, Month FROM tbl_Lookups WHERE Month IS NOT NULL ORDER BY LK_ID", conn);
        using SqlDataReader r2 = cmd2.ExecuteReader();
        while (r2.Read()) Months[r2.GetInt32(0)] = r2.GetString(1);
    }
}

public class WageItem { public int WageID, EmplID, MonthID, Year; public string EmployeeName = "", WageDate = ""; public decimal Amount, Commission, Total; }
