using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace GithubSeleniumTest;

[TestFixture]
public class GitHubTests: IDisposable
{
    private IWebDriver _driver;
    private WebDriverWait _wait;
    private readonly string _githubUsername = "saeedahmadi694";
    private readonly string _githubPassword = "S@t970122";

    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Quit();
    }

    [Test]
    public void Login_ShouldBeSuccessful()
    {
        _driver.Navigate().GoToUrl("https://github.com/login");
        _wait.Until(d => d.FindElement(By.Id("login_field"))).SendKeys(_githubUsername);
        _driver.FindElement(By.Id("password")).SendKeys(_githubPassword);
        _driver.FindElement(By.Name("commit")).Click();

        Assert.IsTrue(_driver.Url.Contains("github.com"));
        Console.WriteLine("Logged into GitHub successfully.");
    }



    [Test]
    public void Profile_ShouldBeAccessible()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}");
        var profileName = _wait.Until(d => d.FindElement(By.CssSelector(".vcard-names")));

        Assert.IsTrue(profileName.Displayed, "Profile name should be visible.");
        Console.WriteLine("Profile checked successfully.");
    }

    [Test]
    public void ListRepositories_ShouldReturnResults()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}?tab=repositories");
        var repoElements = _wait.Until(d => d.FindElements(By.CssSelector("h3 a")));

        Assert.IsNotEmpty(repoElements);
        Console.WriteLine("Repositories found:");
        foreach (var repo in repoElements)
        {
            Console.WriteLine(repo.Text);
        }
    }

    [Test]
    public void StarRepository_ShouldChangeButtonState()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/github/hub");
        var starButton = _wait.Until(d => d.FindElement(By.CssSelector("button.btn-with-count")));

        if (starButton.Text == "Star")
        {
            starButton.Click();
            Console.WriteLine("Repository starred successfully.");
        }
        else
        {
            Console.WriteLine("Repository is already starred.");
        }

        Assert.AreEqual("Unstar", starButton.Text);
    }

    [Test]
    public void ForkRepository_ShouldCreateFork()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/github/hub");
        var forkButton = _wait.Until(d => d.FindElement(By.CssSelector("button.btn-with-count[aria-label*='Fork your own copy']")));
        forkButton.Click();

        _wait.Until(d => d.Url.Contains($"{_githubUsername}/hub"));
        Assert.IsTrue(_driver.Url.Contains($"{_githubUsername}/hub"));
        Console.WriteLine("Repository forked successfully.");
    }

    [Test]
    public void SearchRepository_ShouldReturnResults()
    {
        _driver.Navigate().GoToUrl("https://github.com");
        var searchBox = _wait.Until(d => d.FindElement(By.Name("q")));
        searchBox.SendKeys("selenium");
        searchBox.SendKeys(Keys.Enter);

        var results = _wait.Until(d => d.FindElements(By.CssSelector(".repo-list-item")));
        Assert.IsNotEmpty(results);
        Console.WriteLine("Search results found:");

        foreach (var result in results)
        {
            Console.WriteLine(result.Text);
        }
    }

    [Test]
    public void UpdateProfileBio_ShouldSucceed()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/settings/profile");

        var bioField = _wait.Until(d => d.FindElement(By.Id("user_profile_bio")));
        bioField.Clear();
        bioField.SendKeys("This is a test bio.");
        _driver.FindElement(By.CssSelector("button.btn-primary")).Click();

        _driver.Navigate().Refresh();
        var updatedBio = _driver.FindElement(By.Id("user_profile_bio")).GetAttribute("value");

        Assert.AreEqual("This is a test bio.", updatedBio);
        Console.WriteLine("Profile bio updated successfully.");
    }

    [Test]
    public void CreateRepository_ShouldSucceed()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/new");
        _wait.Until(d => d.FindElement(By.Id("repository_name"))).SendKeys("TestRepository");
        _driver.FindElement(By.CssSelector("button.btn-primary")).Click();

        Assert.IsTrue(_driver.Url.Contains($"github.com/{_githubUsername}/TestRepository"));
        Console.WriteLine("Repository created successfully.");
    }

    [Test]
    public void Logout_ShouldRedirectToLoginPage()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com");

        var profileMenu = _wait.Until(d => d.FindElement(By.CssSelector("summary[aria-label='View profile and more']")));
        profileMenu.Click();

        var logoutButton = _wait.Until(d => d.FindElement(By.CssSelector("button[role='menuitem'][data-ga-click*='Sign out']")));
        logoutButton.Click();

        Assert.IsTrue(_driver.Url.Contains("login"));
        Console.WriteLine("Logged out successfully.");
    }



    [Test]
    public void DeleteRepository_ShouldSucceed()
    {
        Login_ShouldBeSuccessful();
        CreateRepository_ShouldSucceed();

        _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}/TestRepository/settings");
        _wait.Until(d => d.FindElement(By.XPath("//summary[contains(text(),'Delete this repository')]"))).Click();

        var confirmationField = _wait.Until(d => d.FindElement(By.XPath("//input[@aria-label='Type in the name of the repository to confirm deletion.']")));
        confirmationField.SendKeys($"{_githubUsername}/TestRepository");

        _driver.FindElement(By.XPath("//button[contains(text(),'I understand the consequences, delete this repository')]")).Click();

        Assert.IsFalse(_driver.Url.Contains("TestRepository"));
        Console.WriteLine("Repository deleted successfully.");
    }

    [Test]
    public void CheckNotifications_ShouldReturnResults()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/notifications");
        var notifications = _wait.Until(d => d.FindElements(By.CssSelector(".notifications-list-item")));

        if (notifications.Count > 0)
        {
            Console.WriteLine($"Notifications found: {notifications.Count}");
            Assert.IsNotEmpty(notifications);
        }
        else
        {
            Console.WriteLine("No notifications found.");
            Assert.Pass("No notifications to check.");
        }
    }

    [Test]
    public void VerifyContributionsGraph_ShouldBeVisible()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}");
        var contributionsGraph = _wait.Until(d => d.FindElement(By.CssSelector(".js-yearly-contributions")));

        Assert.IsTrue(contributionsGraph.Displayed);
        Console.WriteLine("Contributions graph is visible.");
    }

    [Test]
    public void VerifyUserSettings_ShouldBeAccessible()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/settings/profile");

        var profileHeader = _wait.Until(d => d.FindElement(By.CssSelector(".subnav h2")));
        Assert.IsTrue(profileHeader.Displayed);
        Console.WriteLine("User settings page is accessible.");
    }

    [Test]
    public void CreateIssue_ShouldSucceed()
    {
        Login_ShouldBeSuccessful();
        CreateRepository_ShouldSucceed();

        _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}/TestRepository/issues/new");
        var issueTitle = _wait.Until(d => d.FindElement(By.Id("issue_title")));
        issueTitle.SendKeys("Test Issue");

        var issueDescription = _driver.FindElement(By.Id("issue_body"));
        issueDescription.SendKeys("This is a test issue.");

        _driver.FindElement(By.CssSelector("button[data-disable-with='Submitting…']")).Click();

        Assert.IsTrue(_driver.Url.Contains("issues/"));
        Console.WriteLine("Issue created successfully.");
    }

    [Test]
    public void CheckPullRequestsPage_ShouldBeAccessible()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}/TestRepository/pulls");

        var pullRequestsHeader = _wait.Until(d => d.FindElement(By.CssSelector(".blankslate h3")));
        Assert.IsTrue(pullRequestsHeader.Displayed);
        Console.WriteLine("Pull Requests page is accessible.");
    }

    [Test]
    public void CheckUserRepositoriesList_ShouldContainTestRepository()
    {
        Login_ShouldBeSuccessful();
        CreateRepository_ShouldSucceed();

        _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}?tab=repositories");
        var repositories = _wait.Until(d => d.FindElements(By.CssSelector(".repo-list-item")));

        Assert.IsTrue(repositories.Any(r => r.Text.Contains("TestRepository")));
        Console.WriteLine("TestRepository is listed under user's repositories.");
    }

    [Test]
    public void ExploreGitHubPage_ShouldLoadSuccessfully()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/explore");

        var exploreHeader = _wait.Until(d => d.FindElement(By.CssSelector("h1")));
        Assert.IsTrue(exploreHeader.Displayed && exploreHeader.Text.Contains("Explore"));
        Console.WriteLine("GitHub Explore page loaded successfully.");
    }


    public void Dispose()
    {
        _driver.Quit();
    }
}
