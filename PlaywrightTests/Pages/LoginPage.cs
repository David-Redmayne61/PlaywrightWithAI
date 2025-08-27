using Microsoft.Playwright;
using System.Threading.Tasks;

namespace PlaywrightTests.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;
        public LoginPage(IPage page) => _page = page;

        public async Task LoginAsync(string username, string password)
        {
            await _page.GotoAsync("https://localhost:7031");
            await _page.FillAsync("input[name='Username'], input#Username", username);
            await _page.FillAsync("input[name='Password'], input#Password", password);
            await _page.ClickAsync("button[type='submit'], button:has-text('Login')");
        }
    }
}
