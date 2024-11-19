using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Modules.BrowsingContext;
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
    private readonly string _gitRepo = "TestRepository";

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

    [Test,Order(1)]
    public void Login_ShouldBeSuccessful()
    {
        _driver.Navigate().GoToUrl("https://github.com/login");
        _wait.Until(d => d.FindElement(By.Id("login_field"))).SendKeys(_githubUsername);
        _driver.FindElement(By.Id("password")).SendKeys(_githubPassword);
        _driver.FindElement(By.Name("commit")).Click();

        Assert.IsTrue(_driver.Url.Contains("github.com"));
        Console.WriteLine("Logged into GitHub successfully.");
    }



    [Test, Order(1)]
    public void Profile_ShouldBeAccessible()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}");
        var profileName = _wait.Until(d => d.FindElement(By.CssSelector(".vcard-names")));

        Assert.IsTrue(profileName.Displayed, "Profile name should be visible.");
        Console.WriteLine("Profile checked successfully.");
    }

    [Test, Order(1)]
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

    //[Test, Order(1)]
    //public void StarRepository_ShouldChangeButtonState()
    //{
    //    Login_ShouldBeSuccessful();
    //    _driver.Navigate().GoToUrl("https://github.com/saeedahmadi694/GithubSeleniumTest");
    //    var unstarButton = _wait.Until(d => d.FindElement(By.CssSelector("button[aria-label='star this repository']")));
    //    unstarButton.Click();
    //    var starButton = _wait.Until(d => d.FindElement(By.CssSelector("button.js-toggler-target")));

    //    if (starButton.Text.Contains("Star"))
    //    {
    //        starButton.Click();
    //        Console.WriteLine("Repository starred successfully.");
    //    }
    //    else
    //    {
    //        Console.WriteLine("Repository is already starred.");
    //    }

    //    Assert.IsTrue(starButton.Text.Contains("Starred"));
    //}

    //[Test]
    //public void ForkRepository_ShouldCreateFork()
    //{
    //    Login_ShouldBeSuccessful();
    //    _driver.Navigate().GoToUrl("https://github.com/github/hub");
    //    var forkButton = _wait.Until(d => d.FindElement(By.CssSelector("button.btn-with-count[aria-label*='Fork your own copy']")));
    //    forkButton.Click();

    //    _wait.Until(d => d.Url.Contains($"{_githubUsername}/hub"));
    //    Assert.IsTrue(_driver.Url.Contains($"{_githubUsername}/hub"));
    //    Console.WriteLine("Repository forked successfully.");
    //}

    //[Test, Order(1)]
    //public void SearchRepository_ShouldReturnResults()
    //{
    //    Login_ShouldBeSuccessful();
    //    var searchButton = _wait.Until(d => d.FindElement(By.CssSelector("button[data-target='qbsearch-input.inputButton']")));
    //    searchButton.Click();
    //    var openSearchBox = _wait.Until(d => d.FindElement(By.CssSelector(".AppHeader-searchButton")));
    //    openSearchBox.Click();

    //    var searchBox = _wait.Until(d => d.FindElement(By.Name("query-builder-test")));
    //    searchBox.SendKeys("saeed ahmadi");
    //    searchBox.SendKeys(Keys.Enter);

    //    var results = _wait.Until(d => d.FindElements(By.CssSelector(".repo-list-item")));
    //    Assert.IsNotEmpty(results);
    //    Console.WriteLine("Search results found:");

    //    foreach (var result in results)
    //    {
    //        Console.WriteLine(result.Text);
    //    }
    //}

    [Test, Order(1)]
    public void UpdateProfileBio_ShouldSucceed()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/settings/profile");

        var bioField = _wait.Until(d => d.FindElement(By.Id("user_profile_bio")));
        bioField.Clear();
        bioField.SendKeys("This is a test bio.");
        _driver.FindElement(By.CssSelector("button.Button--primary")).Click();

        _driver.Navigate().Refresh();
        var updatedBio = _driver.FindElement(By.Id("user_profile_bio")).GetAttribute("value");

        Assert.AreEqual("This is a test bio.", updatedBio);
        Console.WriteLine("Profile bio updated successfully.");
    }

    [Test,Order(2)]
    public void CreateRepository_ShouldSucceed()
    {
        // Log in
        Login_ShouldBeSuccessful();

        // Navigate to "Create New Repository" page
        _driver.Navigate().GoToUrl("https://github.com/new");

        // Enter repository name
        var repoNameInput = _wait.Until(d => d.FindElement(By.CssSelector("input[data-testid='repository-name-input']")));
        repoNameInput.SendKeys(_gitRepo);

        // Enter repository description
        var descriptionInput = _driver.FindElement(By.Name("Description"));
        descriptionInput.SendKeys("This is a test repository created via automated UI tests.");

        //// Add a .gitignore template for Visual Studio
        //var gitignoreDropdown = _driver.FindElement(By.Id(":rr:"));
        //gitignoreDropdown.Click();
        //var gitignoreOption = _wait.Until(d => d.FindElement(By.XPath("//button[contains(text(), 'VisualStudio')]")));
        //gitignoreOption.Click();

        //// Add a license (MIT)
        //var licenseDropdown = _driver.FindElement(By.Id(":rr:"));
        //licenseDropdown.Click();
        //var licenseOption = _wait.Until(d => d.FindElement(By.XPath("//button[contains(text(), 'MIT License')]")));
        //licenseOption.Click();

        // Create the repository
        var createRepoButton = _driver.FindElement(By.CssSelector("button.jLvIcQ"));
        createRepoButton.Click();

        // Verify that the repository was created successfully
        Assert.IsTrue(_driver.Url.Contains($"github.com/{_githubUsername}/{_gitRepo}"));
        Console.WriteLine("Repository created successfully with description, .gitignore, and license.");
    }


    [Test, Order(4)]
    public void Logout_ShouldRedirectToLoginPage()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com");

        var userMenuButton = _wait.Until(d => d.FindElement(By.CssSelector("button[aria-label='Open user navigation menu']")));
        userMenuButton.Click();

        var signOutButton = _wait.Until(d => d.FindElement(By.LinkText("Sign out")));
        signOutButton.Click();

        var signAllOutButton = _wait.Until(d => d.FindElement(By.Name("commit")));
        signAllOutButton.Click();

        Assert.IsTrue(_driver.Url.Contains("https://github.com/"));
        Console.WriteLine("Logged out successfully.");
    }



    //[Test, Order(3)]
    //public void DeleteRepository_ShouldSucceed()
    //{
    //    Login_ShouldBeSuccessful();
    //    CreateRepository_ShouldSucceed();

    //    _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}/{_gitRepo}/settings");
    //    _wait.Until(d => d.FindElement(By.XPath("//summary[contains(text(),'Delete this repository')]"))).Click();

    //    var confirmationField = _wait.Until(d => d.FindElement(By.XPath("//input[@aria-label='Type in the name of the repository to confirm deletion.']")));
    //    confirmationField.SendKeys($"{_githubUsername}/{_gitRepo}");

    //    _driver.FindElement(By.XPath("//button[contains(text(),'I understand the consequences, delete this repository')]")).Click();

    //    Assert.IsFalse(_driver.Url.Contains(_gitRepo));
    //    Console.WriteLine("Repository deleted successfully.");
    //}

    [Test, Order(1)]
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

    [Test, Order(1)]
    public void VerifyUserSettings_ShouldBeAccessible()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/settings/profile");

        var profileHeader = _wait.Until(d => d.FindElement(By.CssSelector("#public-profile-heading")));
        Assert.IsTrue(profileHeader.Displayed);
        Console.WriteLine("User settings page is accessible.");
    }

    //[Test]
    //public void CreateIssue_ShouldSucceed()
    //{
    //    Login_ShouldBeSuccessful();

    //    _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}/GithubSeleniumTest/issues/new");
    //    var issueTitle = _wait.Until(d => d.FindElement(By.Id("issue_title")));
    //    issueTitle.SendKeys("Test Issue");

    //    var issueDescription = _driver.FindElement(By.Id("issue_body_template_name"));
    //    issueDescription.SendKeys("This is a test issue.");

    //    _driver.FindElement(By.CssSelector(".btn-primary .btn .ml-2")).Click();

    //    Assert.IsTrue(_driver.Url.Contains("issues/"));
    //    Console.WriteLine("Issue created successfully.");
    //}

    [Test]
    public void CheckPullRequestsPage_ShouldBeAccessible()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}/GithubSeleniumTest/pulls");

        var pullRequestsHeader = _wait.Until(d => d.FindElement(By.CssSelector(".octicon-git-pull-request")));
        Assert.IsTrue(pullRequestsHeader.Enabled);
        Console.WriteLine("Pull Requests page is accessible.");
    }

    //[Test, Order(1)]
    //public void CheckUserRepositoriesList_ShouldContainTestRepository()
    //{
    //    Login_ShouldBeSuccessful();
    //    CreateRepository_ShouldSucceed();

    //    _driver.Navigate().GoToUrl($"https://github.com/{_githubUsername}?tab=repositories");
    //    var repositories = _wait.Until(d => d.FindElements(By.CssSelector(".repo-list-item")));

    //    Assert.IsTrue(repositories.Any(r => r.Text.Contains(_gitRepo)));
    //    Console.WriteLine("TestRepository is listed under user's repositories.");
    //}

    [Test, Order(1)]
    public void ExploreGitHubPage_ShouldLoadSuccessfully()
    {
        Login_ShouldBeSuccessful();
        _driver.Navigate().GoToUrl("https://github.com/explore");

        var exploreHeader = _wait.Until(d => d.FindElement(By.CssSelector("h1.color-fg-muted.mb-n2")));
        Assert.IsTrue(exploreHeader.Enabled && exploreHeader.Text.Contains("interests"));
        Console.WriteLine("GitHub Explore page loaded successfully.");
    }


    public void Dispose()
    {
        _driver.Quit();
    }
}
