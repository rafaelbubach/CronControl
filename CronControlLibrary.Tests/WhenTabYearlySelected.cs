using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace CronControlLibrary.Tests
{
    [TestFixture]
    public class WhenTabYearlySelected
    {
        // Arrange
        // Act
        // Assert

        private Fixture _fixture;
        private Func<int, int, int> _intGenerator;
        private Func<int, int, decimal> _decimalGenerator;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            var decimalGenerator = _fixture.Create<Generator<decimal>>();
            _decimalGenerator = (begin, end) => { return decimalGenerator.First(x => x > begin && x <= end); };

            var intGenerator = _fixture.Create<Generator<int>>();
            _intGenerator = (begin, end) => { return intGenerator.First(x => x > begin && x <= end); };
        }

        [Test]
        public void ShouldChangeSelectedRadioToEvery()
        {
            // Arrange
            var control = new CronControl();
            control.tcMain.SelectTab("tabYearly");

            // Act
            control.rbtYearlyEvery.Checked = true;

            // Assert
            control.rbtYearlyEvery.Checked.Should().BeTrue();
            control.rbtYearlyOrdinal.Checked.Should().BeFalse();
        }

        [Test]
        public void ShouldChangeSelectedRadioToOrdinal()
        {
            // Arrange
            var control = new CronControl();
            control.tcMain.SelectTab("tabYearly");

            // Act
            control.rbtYearlyOrdinal.Checked = true;

            // Assert
            control.rbtYearlyOrdinal.Checked.Should().BeTrue();
            control.rbtYearlyEvery.Checked.Should().BeFalse();
        }

        [Test]
        public void ShouldBeInputedValueWhenEverySelected()
        {
            // Arrange
            var day = _decimalGenerator(1, 31);
            var month = _intGenerator(1, 12);
            var time = _fixture.Create<DateTime>();
            var control = new CronControl();
            control.tcMain.SelectTab("tabYearly");
            control.rbtYearlyEvery.Checked = true;
            control.dtpYearlyTime.Value = time;
            control.ddlYearlyMonths.SelectedIndex = month - 1;
            control.nudYearlyDay.Value = day;

            // Act
            var result = control.GetYearlyExpression();

            // Assert
            result.Should().Be($"0 {time.Minute} {time.Hour} {day} {month} ? *");
        }

        [Test]
        public void ShouldBeInputedValueWhenOrdinalSelected()
        {
            // Arrange
            var ordinal = _intGenerator(0, 4);
            var ordinalText = ordinal < 4 ? $"#{ordinal + 1}" : "L";
            var weekday = _intGenerator(1, 7);
            var month = _intGenerator(1, 12);
            var time = _fixture.Create<DateTime>();
            var control = new CronControl();
            control.tcMain.SelectTab("tabYearly");
            control.rbtYearlyOrdinal.Checked = true;
            control.dtpYearlyTime.Value = time;
            control.ddlYearlyMonths2.SelectedIndex = month - 1;
            control.ddlYearlyOrdinal.SelectedIndex = ordinal;
            control.ddlYearlyWeekdays.SelectedIndex = weekday - 1;

            // Act
            var result = control.GetYearlyExpression();

            // Assert
            result.Should().Be($"0 {time.Minute} {time.Hour} ? {month} {weekday}{ordinalText} *");
        }
    }
}
