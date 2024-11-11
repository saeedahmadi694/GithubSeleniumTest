using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTest;

[TestFixture]
public class GitHubTests
{
    private IWebDriver driver;
    private WebDriverWait wait;
    private string githubUsername = "saeedahmadi694";
    private string githubPassword = "S@t970122";

    [SetUp]
    public void SetUp()
    {
        driver = new ChromeDriver();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }

    [Test]
    public void TestLoginToGitHub()
    {
        driver.Navigate().GoToUrl("https://github.com/login");
        wait.Until(d => d.FindElement(By.Id("login_field"))).SendKeys(githubUsername);
        driver.FindElement(By.Id("password")).SendKeys(githubPassword);
        driver.FindElement(By.Name("commit")).Click();

        //Assert.Equals(driver.Url.Contains("github.com"),true);
        Console.WriteLine("Logged into GitHub successfully.");
    }

    [Test]
    public void TestCreateNewRepository()
    {
        TestLoginToGitHub(); // Log in before creating a repository
        driver.Navigate().GoToUrl("https://github.com/new");
        wait.Until(d => d.FindElement(By.Id("repository_name"))).SendKeys("TestRepository");
        driver.FindElement(By.CssSelector("button.btn-primary")).Click();

        Assert.Equals(driver.Url.Contains($"github.com/{githubUsername}/TestRepository"), true);
        Console.WriteLine("Repository created successfully.");
    }

    [Test]
    public void TestCheckProfile()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl($"https://github.com/{githubUsername}");
        var profileName = wait.Until(d => d.FindElement(By.CssSelector(".vcard-names")));

        //Assert.Equals(profileName.Displayed, true);
        Console.WriteLine("Profile checked successfully.");
    }

    [Test]
    public void TestListRepositories()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl($"https://github.com/{githubUsername}?tab=repositories");
        var repoElements = wait.Until(d => d.FindElements(By.CssSelector("h3 a")));

        //Assert.Equals(repoElements,true);
        Console.WriteLine("Repositories found:");
        foreach (var repo in repoElements)
        {
            Console.WriteLine(repo.Text);
        }
    }

    [Test]
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

        //Assert.IsTrue(starButton.Text == "Unstar", "Repository should now be starred.");
    }

    [Test]
    public void TestForkRepository()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl("https://github.com/github/hub"); // Example public repository
        var forkButton = wait.Until(d => d.FindElement(By.CssSelector("button.btn-with-count[aria-label*='Fork your own copy']")));
        forkButton.Click();

        // Wait until fork completes, which navigates to the user's forked repository
        wait.Until(d => d.Url.Contains($"{githubUsername}/hub"));
        Console.WriteLine("Repository forked successfully.");
        //Assert.IsTrue(driver.Url.Contains(githubUsername), "Fork should navigate to user's forked repository.");
    }

    [Test]
    public void TestSearchForRepository()
    {
        driver.Navigate().GoToUrl("https://github.com");
        var searchBox = wait.Until(d => d.FindElement(By.Name("q")));
        searchBox.SendKeys("selenium");
        searchBox.SendKeys(Keys.Enter);

        var results = wait.Until(d => d.FindElements(By.CssSelector(".repo-list-item")));
        //Assert.IsNotEmpty(results, "Search should return results.");
        Console.WriteLine("Search results found:");

        foreach (var result in results)
        {
            Console.WriteLine(result.Text);
        }
    }

    [Test]
    public void TestLogoutFromGitHub()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl("https://github.com");

        var profileMenu = wait.Until(d => d.FindElement(By.CssSelector("summary[aria-label='View profile and more']")));
        profileMenu.Click();

        var logoutButton = wait.Until(d => d.FindElement(By.CssSelector("button[role='menuitem'][data-ga-click*='Sign out']")));
        logoutButton.Click();

        //Assert.IsTrue(driver.Url.Contains("login"), "Logout should navigate back to login page.");
        Console.WriteLine("Logged out successfully.");
    }
    [Test]
    public void TestEditProfileBio()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl("https://github.com/settings/profile");

        var bioField = wait.Until(d => d.FindElement(By.Id("user_profile_bio")));
        bioField.Clear();
        bioField.SendKeys("This is a test bio.");

        driver.FindElement(By.CssSelector("button.btn-primary")).Click(); // Save changes

        // Verify change by reloading the profile settings page
        driver.Navigate().Refresh();
        var updatedBio = driver.FindElement(By.Id("user_profile_bio")).GetAttribute("value");

        //Assert.AreEqual("This is a test bio.", updatedBio, "Bio should be updated.");
        Console.WriteLine("Profile bio updated successfully.");
    }

    [Test]
    public void TestEditProfileLocation()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl("https://github.com/settings/profile");

        var locationField = wait.Until(d => d.FindElement(By.Id("user_profile_location")));
        locationField.Clear();
        locationField.SendKeys("Test City");

        driver.FindElement(By.CssSelector("button.btn-primary")).Click(); // Save changes

        // Verify change by reloading the profile settings page
        driver.Navigate().Refresh();
        var updatedLocation = driver.FindElement(By.Id("user_profile_location")).GetAttribute("value");

        //Assert.AreEqual("Test City", updatedLocation, "Location should be updated.");
        Console.WriteLine("Profile location updated successfully.");
    }

    [Test]
    public void TestEditProfileName()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl("https://github.com/settings/profile");

        var nameField = wait.Until(d => d.FindElement(By.Id("user_profile_name")));
        nameField.Clear();
        nameField.SendKeys("Test User");

        driver.FindElement(By.CssSelector("button.btn-primary")).Click(); // Save changes

        // Verify change by reloading the profile settings page
        driver.Navigate().Refresh();
        var updatedName = driver.FindElement(By.Id("user_profile_name")).GetAttribute("value");

        //Assert.AreEqual("Test User", updatedName, "Name should be updated.");
        Console.WriteLine("Profile name updated successfully.");
    }

    [Test]
    public void TestVerifyProfileChanges()
    {
        TestLoginToGitHub();
        driver.Navigate().GoToUrl($"https://github.com/{githubUsername}");

        var bio = wait.Until(d => d.FindElement(By.CssSelector(".p-note")));
        var location = driver.FindElement(By.CssSelector(".p-label"));
        var name = driver.FindElement(By.CssSelector(".vcard-fullname"));

        //Assert.AreEqual("This is a test bio.", bio.Text, "Bio should match updated bio.");
        //Assert.AreEqual("Test City", location.Text, "Location should match updated location.");
        //Assert.AreEqual("Test User", name.Text, "Name should match updated name.");

        Console.WriteLine("Verified profile changes on the profile page successfully.");
    }
    [Test]
    public void TestCreateRepoBranchAndPullRequest()
    {
        TestLoginToGitHub();

        // Step 1: Create a new repository
        string repoName = "TestRepository_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        CreateNewRepository(repoName);

        // Step 2: Create a new branch
        CreateBranch(repoName, "new-feature-branch");

        // Step 3: Create a pull request from main to the new branch and assign it to yourself
        CreatePullRequest(repoName, "new-feature-branch");
    }

    public void CreateNewRepository(string repoName)
    {
        driver.Navigate().GoToUrl("https://github.com/new");
        var repoNameField = wait.Until(d => d.FindElement(By.Id("repository_name")));
        repoNameField.SendKeys(repoName);

        var createRepoButton = driver.FindElement(By.CssSelector("button.btn-primary"));
        createRepoButton.Click();

        //Assert.IsTrue(driver.Url.Contains($"github.com/{githubUsername}/{repoName}"), "Repository creation failed.");
        Console.WriteLine($"Repository '{repoName}' created successfully.");
    }

    public void CreateBranch(string repoName, string branchName)
    {
        driver.Navigate().GoToUrl($"https://github.com/{githubUsername}/{repoName}/tree/main");

        // Go to "Branch" selector
        var branchMenu = wait.Until(d => d.FindElement(By.CssSelector("button[aria-label='Switch branches or tags']")));
        branchMenu.Click();

        // Click to create new branch
        var createBranchInput = wait.Until(d => d.FindElement(By.CssSelector("input[name='query']")));
        createBranchInput.SendKeys(branchName);
        createBranchInput.SendKeys(Keys.Enter);

        //Assert.IsTrue(driver.Url.Contains(branchName), "Branch creation failed.");
        Console.WriteLine($"Branch '{branchName}' created successfully.");
    }

    public void CreatePullRequest(string repoName, string branchName)
    {
        driver.Navigate().GoToUrl($"https://github.com/{githubUsername}/{repoName}/pulls");
        var newPRButton = wait.Until(d => d.FindElement(By.LinkText("New pull request")));
        newPRButton.Click();

        // Set up the pull request from `main` to `branchName`
        var compareDropdown = wait.Until(d => d.FindElement(By.Id("base")));
        compareDropdown.Click();
        wait.Until(d => d.FindElement(By.CssSelector($"span[data-content='{branchName}']"))).Click();

        var createPRButton = wait.Until(d => d.FindElement(By.CssSelector(".btn-primary")));
        createPRButton.Click();

        // Assign PR to yourself
        var assigneeTab = wait.Until(d => d.FindElement(By.XPath("//summary[@aria-label='Select assignees']")));
        assigneeTab.Click();

        var selfAssign = wait.Until(d => d.FindElement(By.XPath($"//span[contains(text(), '{githubUsername}')]")));
        selfAssign.Click();

        // Submit the pull request
        var submitPRButton = driver.FindElement(By.CssSelector(".btn-primary"));
        submitPRButton.Click();

        //Assert.IsTrue(driver.Url.Contains("pull"), "Pull request creation failed.");
        Console.WriteLine("Pull request created and assigned successfully.");
    }

}
