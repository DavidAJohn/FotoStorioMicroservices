using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Store.PlaywrightTests
{
    public class HomePageTests
    {
        [Test]
        public async Task HomePageLoads_Success()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync("https://localhost:5080");
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot.png" });

            Assert.Pass();
        }
    }
}