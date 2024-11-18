using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumTest;
public class SampleTests
{
    [Fact]
    public void TestAddition()
    {
        int result = 2 + 3;
        Assert.Equal(5, result);
    }

    [Fact]
    public void TestSubtraction()
    {
        int result = 5 - 3;
        Assert.Equal(2, result);
    }
}