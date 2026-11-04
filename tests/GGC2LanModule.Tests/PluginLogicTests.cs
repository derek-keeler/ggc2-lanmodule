using Xunit;

public class PluginLogicTests
{
    [Theory]
    [InlineData(1920f, 1080f, 1920f, 1080f, 1f)]     // exact reference resolution
    [InlineData(3840f, 2160f, 1920f, 1080f, 2f)]     // uniform upscale
    [InlineData(960f, 540f, 1920f, 1080f, 0.5f)]     // uniform downscale
    [InlineData(2560f, 1080f, 1920f, 1080f, 1f)]     // ultrawide: height is the limiting axis
    [InlineData(1920f, 1200f, 1920f, 1080f, 1f)]     // taller-than-reference: width is the limiting axis
    public void ComputeScale_ReturnsSmallerOfTheTwoAxisRatios(float screenWidth, float screenHeight, float refWidth, float refHeight, float expected)
    {
        float actual = PluginLogic.ComputeScale(screenWidth, screenHeight, refWidth, refHeight);

        Assert.Equal(expected, actual, precision: 5);
    }

    [Theory]
    [InlineData(0, "0ms")]
    [InlineData(42, "42ms")]
    [InlineData(999, "999ms")]       // exactly at the cap - still shown as a real number
    [InlineData(1000, "999+ms")]     // just over the cap - switches to the "999+" label
    [InlineData(5000, "999+ms")]     // far over the cap - stays capped, doesn't grow
    public void FormatPing_CapsDisplayAt999Plus(int rttMs, string expected)
    {
        string actual = PluginLogic.FormatPing(rttMs);

        Assert.Equal(expected, actual);
    }
}
