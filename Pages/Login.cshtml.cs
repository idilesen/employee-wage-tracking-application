using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class LoginModel : PageModel
{
    public string ErrorMessage { get; set; } = "";
    private string connStr = "Server=localhost,1433;Database=db_EWTA;User Id=sa;Password=EwtaPass123!;TrustServerCertificate=True;";

    public void OnGet() { }

    public IActionResult OnPost(string username, string password)
    {
        using SqlConnection conn = new(connStr);
        conn.Open();
        string query = "SELECT COUNT(*) FROM tbl_Users WHERE User_Name=@u AND User_Psw=@p";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);
        int count = (int)cmd.ExecuteScalar();

        if (count > 0)
        {
            HttpContext.Session.SetString("username", username);
            return RedirectToPage("/Index");
        }
        else
        {
            ErrorMessage = "Invalid username or password!";
            return Page();
        }
    }
}