using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        var category = Request.Query["category"].ToString();

        if ((category == "Forms" || category == "Reports")
            && string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
        {
            return RedirectToPage("/Login");
        }

        return Page();
    }
}