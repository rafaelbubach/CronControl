using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace CronControlLibrary.Tests
{
    [TestFixture]
    public class WhenTabDailySelected
    {
        // Arrange
        // Act
        // Assert

        private Fixture _fixture;
        private Func<decimal> _generator;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            var generator = _fixture.Create<Generator<decimal>>();
            _generator = () => { return generator.First(x => x > 1 && x <= 360); };
        }

        [Test]
        public void ShouldChangeSelectedRadioToWeekdays()
        {
            // Arrange
            var control = new CronControl();
            control.tcMain.SelectTab("tabDaily");

            // Act
            control.rbtDailyWeekDays.Checked = true;

            // Assert
            control.rbtDailyWeekDays.Checked.Should().BeTrue();
            control.rbtDailyEvery.Checked.Should().BeFalse();
        }

        [Test]
        public void ShouldChangeSelectedRadioToEvery()
        {
            // Arrange
            var control = new CronControl();
            control.tcMain.SelectTab("tabDaily");

            // Act
            control.rbtDailyEvery.Checked = true;

            // Assert
            control.rbtDailyEvery.Checked.Should().BeTrue();
            control.rbtDailyWeekDays.Checked.Should().BeFalse();
        }

        [Test]
        public void ShouldBeInputedValueWhenEverySelected()
        {
            // Arrange
            var days = _generator();
            var time = _fixture.Create<DateTime>();
            var control = new CronControl();
            control.tcMain.SelectTab("tabDaily");
            control.rbtDailyEvery.Checked = true;
            control.nudDailyDays.Value = days;
            control.dtpDailyTime.Value = time;

            // Act
            var result = control.GetDailyExpression();

            // Assert
            result.Should().Be($"0 {time.Minute} {time.Hour} 1/{days} * ? *");
        }

        [Test]
        public void ShouldBeInputedValueWhenWeekdaysSelected()
        {
            // Arrange
            var time = _fixture.Create<DateTime>();
            var control = new CronControl();
            control.tcMain.SelectTab("tabDaily");
            control.rbtDailyWeekDays.Checked = true;
            control.dtpDailyTime.Value = time;

            // Act
            var result = control.GetDailyExpression();

            // Assert
            result.Should().Be($"0 {time.Minute} {time.Hour} ? * 2-6 *");
        }
    }
}
