// See https://aka.ms/new-console-template for more information
using SeleniumTest;

Console.WriteLine("Hello, World!");
var test = new GitHubTests();
test.SetUp();
//test.TestLoginToGitHub();
test.TestCreateRepoBranchAndPullRequest();