using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumTest;

public class GitHubTests : IDisposable
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;
    private readonly string githubUsername = "saeedahmadi694";
    private readonly string githubPassword = "S@t970122";

    // Constructor for setup
    public GitHubTests()
    {
        driver = new ChromeDriver();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    // IDisposable for cleanup
    public void Dispose()
    {
        driver.Quit();
    }

    [Fact]
    public void TestLoginToGitHub()
    {
        driver.Navigate().GoToUrl("https://github.com/login");
        wait.Until(d => d.FindElement(By.Id("login_field"))).SendKeys(githubUsername);
        driver.FindElement(By.Id("password")).SendKeys(githubPassword);
        driver.FindElement(By.Name("commit")).Click();

        Assert.Contains("github.com", driver.Url);
        Console.WriteLine("Logged into GitHub successfully.");
    }

    [Fact]
    public void TestCreateNewRepository()
    {
        TestLoginToGitHub(); // Log in before creating a repository
        driver.Navigate().GoToUrl("https://github.com/new");
        wait.Until(d => d.FindElement(By.Id("repository_name"))).SendKeys("TestRepository");
        driver.FindElement(By.CssSelector("button.btn-primary")).Click();

        Assert.Contains($"github.com/{githubUsername}/TestRepository", driver.Url);
        Console.WriteLine("Repository created successfully.");
    }

    [Fact]
    public void TestCheckProfile()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl($"https://github.com/{githubUsername}");
        var profileName = wait.Until(d => d.FindElement(By.CssSelector(".vcard-names")));

        Assert.True(profileName.Displayed, "Profile name should be visible.");
        Console.WriteLine("Profile checked successfully.");
    }

    [Fact]
    public void TestListRepositories()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl($"https://github.com/{githubUsername}?tab=repositories");
        var repoElements = wait.Until(d => d.FindElements(By.CssSelector("h3 a")));

        Assert.NotEmpty(repoElements);
        Console.WriteLine("Repositories found:");
        foreach (var repo in repoElements)
        {
            Console.WriteLine(repo.Text);
        }
    }

    [Fact]
    public void TestStarRepository()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl("https://github.com/github/hub"); // Example public repository
        var starButton = wait.Until(d => d.FindElement(By.CssSelector("button.btn-with-count")));

        if (starButton.Text == "Star")
        {
            starButton.Click(); // Click to star the repository
            Console.WriteLine("Repository starred successfully.");
        }
        else
        {
            Console.WriteLine("Repository is already starred.");
        }

        Assert.Equal("Unstar", starButton.Text);
    }

    [Fact]
    public void TestForkRepository()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl("https://github.com/github/hub"); // Example public repository
        var forkButton = wait.Until(d => d.FindElement(By.CssSelector("button.btn-with-count[aria-label*='Fork your own copy']")));
        forkButton.Click();

        // Wait until fork completes, which navigates to the user's forked repository
        wait.Until(d => d.Url.Contains($"{githubUsername}/hub"));
        Console.WriteLine("Repository forked successfully.");
        Assert.Contains($"{githubUsername}/hub", driver.Url);
    }

    [Fact]
    public void TestSearchForRepository()
    {
        driver.Navigate().GoToUrl("https://github.com");
        var searchBox = wait.Until(d => d.FindElement(By.Name("q")));
        searchBox.SendKeys("selenium");
        searchBox.SendKeys(Keys.Enter);

        var results = wait.Until(d => d.FindElements(By.CssSelector(".repo-list-item")));
        Assert.NotEmpty(results);
        Console.WriteLine("Search results found:");

        foreach (var result in results)
        {
            Console.WriteLine(result.Text);
        }
    }

    [Fact]
    public void TestLogoutFromGitHub()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl("https://github.com");

        var profileMenu = wait.Until(d => d.FindElement(By.CssSelector("summary[aria-label='View profile and more']")));
        profileMenu.Click();

        var logoutButton = wait.Until(d => d.FindElement(By.CssSelector("button[role='menuitem'][data-ga-click*='Sign out']")));
        logoutButton.Click();

        Assert.Contains("login", driver.Url);
        Console.WriteLine("Logged out successfully.");
    }

    [Fact]
    public void TestEditProfileBio()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl("https://github.com/settings/profile");

        var bioField = wait.Until(d => d.FindElement(By.Id("user_profile_bio")));
        bioField.Clear();
        bioField.SendKeys("This is a test bio.");
        driver.FindElement(By.CssSelector("button.btn-primary")).Click(); // Save changes

        driver.Navigate().Refresh();
        var updatedBio = driver.FindElement(By.Id("user_profile_bio")).GetAttribute("value");

        Assert.Equal("This is a test bio.", updatedBio);
        Console.WriteLine("Profile bio updated successfully.");
    }
}
