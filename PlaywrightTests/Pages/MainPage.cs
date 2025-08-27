using Microsoft.Playwright;
using System.Threading.Tasks;

namespace PlaywrightTests.Pages
{
    public class MainPage
    {
        private readonly IPage _page;
        public MainPage(IPage page) => _page = page;

        public async Task<bool> IsLoadedAsync()
        {
            var title = await _page.TitleAsync();
            return title.Contains("First Project");
        }

        public async Task ClickViewPeopleAsync()
        {
            var viewPeopleLink = await _page.QuerySelectorAsync("a:text('View People')");
            if (viewPeopleLink != null)
                await viewPeopleLink.ClickAsync();
        }
    }
}
