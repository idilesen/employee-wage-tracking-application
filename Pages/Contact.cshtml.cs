using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
public class ContactModel : PageModel
{
    public bool Sent { get; set; } = false;
    public void OnGet() { }
    public IActionResult OnPost(string Name, string Email, string Message) { Sent = true; return Page(); }
}
