using AJE.Domain.Ai;

namespace AJE.Test.Unit;

public class CompletionAdjustorTests
{
    [Fact]
    public void GetSettingsThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CompletionAdjustor.GetSettings(0));
    }

    [Theory]
    [InlineData(1, 0.1, 40)]
    [InlineData(2, 0.2, 40)]
    [InlineData(3, 0.3, 40)]
    [InlineData(4, 0.4, 40)]
    [InlineData(5, 0.5, 40)]
    [InlineData(6, 0.6, 40)]
    [InlineData(7, 0.7, 40)]
    [InlineData(8, 0.8, 40)]
    [InlineData(9, 0.9, 40)]
    [InlineData(10, 1.0, 40)]

    [InlineData(11, 0.1, 10)]
    [InlineData(12, 0.1, 20)]
    [InlineData(13, 0.1, 30)]
    [InlineData(14, 0.1, 40)]
    [InlineData(15, 0.1, 50)]
    [InlineData(16, 0.1, 60)]
    [InlineData(17, 0.1, 70)]
    [InlineData(18, 0.1, 80)]
    [InlineData(19, 0.1, 90)]
    [InlineData(20, 0.1, 100)]

    [InlineData(21, 0.2, 10)]
    [InlineData(22, 0.2, 20)]
    [InlineData(23, 0.2, 30)]
    [InlineData(24, 0.2, 40)]
    [InlineData(25, 0.2, 50)]
    [InlineData(26, 0.2, 60)]
    [InlineData(27, 0.2, 70)]
    [InlineData(28, 0.2, 80)]
    [InlineData(29, 0.2, 90)]
    [InlineData(30, 0.2, 100)]

    [InlineData(31, 0.3, 10)]
    [InlineData(32, 0.3, 20)]
    [InlineData(33, 0.3, 30)]
    [InlineData(34, 0.3, 40)]
    [InlineData(35, 0.3, 50)]
    [InlineData(36, 0.3, 60)]
    [InlineData(37, 0.3, 70)]
    [InlineData(38, 0.3, 80)]
    [InlineData(39, 0.3, 90)]
    [InlineData(40, 0.3, 100)]

    [InlineData(41, 0.4, 10)]
    [InlineData(42, 0.4, 20)]
    [InlineData(43, 0.4, 30)]
    [InlineData(44, 0.4, 40)]
    [InlineData(45, 0.4, 50)]
    [InlineData(46, 0.4, 60)]
    [InlineData(47, 0.4, 70)]
    [InlineData(48, 0.4, 80)]
    [InlineData(49, 0.4, 90)]
    [InlineData(50, 0.4, 100)]

    [InlineData(51, 0.5, 10)]
    [InlineData(52, 0.5, 20)]
    [InlineData(53, 0.5, 30)]
    [InlineData(54, 0.5, 40)]
    [InlineData(55, 0.5, 50)]
    [InlineData(56, 0.5, 60)]
    [InlineData(57, 0.5, 70)]
    [InlineData(58, 0.5, 80)]
    [InlineData(59, 0.5, 90)]
    [InlineData(60, 0.5, 100)]

    [InlineData(61, 0.6, 10)]
    [InlineData(62, 0.6, 20)]
    [InlineData(63, 0.6, 30)]
    [InlineData(64, 0.6, 40)]
    [InlineData(65, 0.6, 50)]
    [InlineData(66, 0.6, 60)]
    [InlineData(67, 0.6, 70)]
    [InlineData(68, 0.6, 80)]
    [InlineData(69, 0.6, 90)]
    [InlineData(70, 0.6, 100)]

    [InlineData(71, 0.7, 10)]
    [InlineData(72, 0.7, 20)]
    [InlineData(73, 0.7, 30)]
    [InlineData(74, 0.7, 40)]
    [InlineData(75, 0.7, 50)]
    [InlineData(76, 0.7, 60)]
    [InlineData(77, 0.7, 70)]
    [InlineData(78, 0.7, 80)]
    [InlineData(79, 0.7, 90)]
    [InlineData(80, 0.7, 100)]

    [InlineData(81, 0.8, 10)]
    [InlineData(82, 0.8, 20)]
    [InlineData(83, 0.8, 30)]
    [InlineData(84, 0.8, 40)]
    [InlineData(85, 0.8, 50)]
    [InlineData(86, 0.8, 60)]
    [InlineData(87, 0.8, 70)]
    [InlineData(88, 0.8, 80)]
    [InlineData(89, 0.8, 90)]
    [InlineData(90, 0.8, 100)]

    [InlineData(91, 0.9, 10)]
    [InlineData(92, 0.9, 20)]
    [InlineData(93, 0.9, 30)]
    [InlineData(94, 0.9, 40)]
    [InlineData(95, 0.9, 50)]
    [InlineData(96, 0.9, 60)]
    [InlineData(97, 0.9, 70)]
    [InlineData(98, 0.9, 80)]
    [InlineData(99, 0.9, 90)]
    [InlineData(100, 0.9, 100)]

    [InlineData(101, 1.0, 10)]
    [InlineData(102, 1.0, 20)]
    [InlineData(103, 1.0, 30)]
    [InlineData(104, 1.0, 40)]
    [InlineData(105, 1.0, 50)]
    [InlineData(106, 1.0, 60)]
    [InlineData(107, 1.0, 70)]
    [InlineData(108, 1.0, 80)]
    [InlineData(109, 1.0, 90)]
    [InlineData(110, 1.0, 100)]
    public void GetSettings(int tryNumber, double temperature, int topK)
    {
        var settings = CompletionAdjustor.GetSettings(tryNumber);
        var expectedTemperature = Math.Round(temperature, 1);
        Assert.Equal(expectedTemperature, settings.Temperature);
        Assert.Equal(topK, settings.TopK);
    }

    [Fact]
    public void GetSettingsRandom()
    {
        for (int i = 111; i < 200; i++)
        {
            var settings = CompletionAdjustor.GetSettings(i);
            Assert.InRange(settings.Temperature, 0.1, 1.0);
            Assert.InRange(settings.TopK, 10, 100);
        }
    }
}
