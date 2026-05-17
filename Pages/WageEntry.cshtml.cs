using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class WageEntryModel : PageModel
{
    public string Message { get; set; } = "";
    public bool Success { get; set; } = false;
    public Dictionary<int, string> Employees { get; set; } = new();
    public Dictionary<int, string> Months { get; set; } = new();

    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet() => LoadDropdowns();

    public IActionResult OnPost(int EmplID, string WageDate, decimal WageAmount, decimal WageCommission, int MonthID)
    {
        try
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            string sql = "INSERT INTO tbl_Wages (Empl_ID, Wage_Date, Wage_Amount, Wage_Commission, Month_ID) VALUES (@eid, @wd, @wa, @wc, @mid)";
            using SqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@eid", EmplID);
            cmd.Parameters.AddWithValue("@wd", DateTime.Parse(WageDate));
            cmd.Parameters.AddWithValue("@wa", WageAmount);
            cmd.Parameters.AddWithValue("@wc", WageCommission);
            cmd.Parameters.AddWithValue("@mid", MonthID);
            cmd.ExecuteNonQuery();
            Success = true;
            Message = "Wage record saved successfully!";
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
        using SqlCommand cmd1 = new("SELECT Empl_ID, Empl_Name FROM tbl_Employees WHERE Is_Empl_Active=1 ORDER BY Empl_Name", conn);
        using SqlDataReader r1 = cmd1.ExecuteReader();
        while (r1.Read()) Employees[r1.GetInt32(0)] = r1.IsDBNull(1) ? "" : r1.GetString(1);
        r1.Close();
        using SqlCommand cmd2 = new("SELECT LK_ID, Month FROM tbl_Lookups WHERE Month IS NOT NULL ORDER BY LK_ID", conn);
        using SqlDataReader r2 = cmd2.ExecuteReader();
        while (r2.Read()) Months[r2.GetInt32(0)] = r2.GetString(1);
    }
}
