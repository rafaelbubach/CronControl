using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace CronControlLibrary.Tests
{
    [TestFixture]
    public class WhenSetValue
    {
        // Arrange
        // Act
        // Assert

        private Fixture _fixture;
        private Func<int, int, int> _intGenerator;
        private Func<int, int, decimal> _decimalGenerator;
        private IList<bool> _checks;
        private IList<bool> Checks => _checks ?? (_checks = _fixture.CreateMany<bool>(7).ToList());
        private IList<bool> OpositeChecks => Checks.Select(c => !c).ToList();

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
        public void ShouldSelectMinutesTabIfMinutesExpression()
        {
            // Arrange
            var control = new CronControl();
            var minutes = _decimalGenerator(1, 3600);
            var expression = $"0 0/{minutes} * 1/1 * ? *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(0);
            control.nudMinutes.Value.Should().Be(minutes);
        }

        [Test]
        public void ShouldSelectHoursTabIfHourlyExpression()
        {
            // Arrange
            var control = new CronControl();
            var hours = _decimalGenerator(1, 23);
            var time = _fixture.Create<DateTime>();
            var expression = $"0 {time.Minute} {time.Hour}/{hours} 1/1 * ? *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(1);
            control.nudHourlyHours.Value.Should().Be(hours);
            control.dtpHourlyTime.Value.Hour.Should().Be(time.Hour);
            control.dtpHourlyTime.Value.Minute.Should().Be(time.Minute);
        }

        [Test]
        public void ShouldSelectDaysTabIfDailyExpression()
        {
            // Arrange
            var control = new CronControl();
            var days = _decimalGenerator(1, 360);
            var time = _fixture.Create<DateTime>();
            var expression = $"0 {time.Minute} {time.Hour} 1/{days} * ? *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(2);
            control.nudDailyDays.Value.Should().Be(days);
            control.dtpDailyTime.Value.Hour.Should().Be(time.Hour);
            control.dtpDailyTime.Value.Minute.Should().Be(time.Minute);
        }

        [Test]
        public void ShouldSelectDaysTabAndWeekdaysIfDailyWeekdaysExpression()
        {
            // Arrange
            var control = new CronControl();
            var time = _fixture.Create<DateTime>();
            var expression = $"0 {time.Minute} {time.Hour} ? * 2-6 *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(2);
            control.rbtDailyWeekDays.Checked.Should().BeTrue();
            control.dtpDailyTime.Value.Hour.Should().Be(time.Hour);
            control.dtpDailyTime.Value.Minute.Should().Be(time.Minute); ;
        }

        [Test]
        public void ShouldSelectWeeksTabIfWeekyExpression()
        {
            // Arrange
            var control = new CronControl();
            var weekdays = new StringBuilder();
            if (Checks[0]) weekdays.Append(",1");
            if (Checks[1]) weekdays.Append(",2");
            if (Checks[2]) weekdays.Append(",3");
            if (Checks[3]) weekdays.Append(",4");
            if (Checks[4]) weekdays.Append(",5");
            if (Checks[5]) weekdays.Append(",6");
            if (Checks[6]) weekdays.Append(",7");
            weekdays.Remove(0, 1);

            var time = _fixture.Create<DateTime>();
            var expression = $"0 {time.Minute} {time.Hour} ? * {weekdays} *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(3);
            control.dtpWeeklyTime.Value.Hour.Should().Be(time.Hour);
            control.dtpWeeklyTime.Value.Minute.Should().Be(time.Minute);
            control.cbxSunday.Checked.Should().Be(Checks[0]);
            control.cbxMonday.Checked.Should().Be(Checks[1]);
            control.cbxTuesday.Checked.Should().Be(Checks[2]);
            control.cbxWednesday.Checked.Should().Be(Checks[3]);
            control.cbxThursday.Checked.Should().Be(Checks[4]);
            control.cbxFriday.Checked.Should().Be(Checks[5]);
            control.cbxSaturday.Checked.Should().Be(Checks[6]);
        }

        [Test]
        public void ShouldSelectWeeksTabIfWeekyExpressionOposite()
        {
            // Arrange
            var control = new CronControl();
            var weekdays = new StringBuilder();
            if (OpositeChecks[0]) weekdays.Append(",1");
            if (OpositeChecks[1]) weekdays.Append(",2");
            if (OpositeChecks[2]) weekdays.Append(",3");
            if (OpositeChecks[3]) weekdays.Append(",4");
            if (OpositeChecks[4]) weekdays.Append(",5");
            if (OpositeChecks[5]) weekdays.Append(",6");
            if (OpositeChecks[6]) weekdays.Append(",7");
            weekdays.Remove(0, 1);

            var time = _fixture.Create<DateTime>();
            var expression = $"0 {time.Minute} {time.Hour} ? * {weekdays} *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(3);
            control.dtpWeeklyTime.Value.Hour.Should().Be(time.Hour);
            control.dtpWeeklyTime.Value.Minute.Should().Be(time.Minute);
            control.cbxSunday.Checked.Should().Be(OpositeChecks[0]);
            control.cbxMonday.Checked.Should().Be(OpositeChecks[1]);
            control.cbxTuesday.Checked.Should().Be(OpositeChecks[2]);
            control.cbxWednesday.Checked.Should().Be(OpositeChecks[3]);
            control.cbxThursday.Checked.Should().Be(OpositeChecks[4]);
            control.cbxFriday.Checked.Should().Be(OpositeChecks[5]);
            control.cbxSaturday.Checked.Should().Be(OpositeChecks[6]);
        }

        [Test]
        public void ShouldSelectMonthsTabIfMonthlyExpression()
        {
            // Arrange
            var control = new CronControl();
            var days = _decimalGenerator(1, 31);
            var months = _decimalGenerator(1, 12);
            var time = _fixture.Create<DateTime>();
            var expression = $"0 {time.Minute} {time.Hour} {days} 1/{months} ? *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(4);
            control.nudMonthlyDays.Value.Should().Be(days);
            control.nudMonthlyMonths.Value.Should().Be(months);
            control.dtpMonthlyTime.Value.Hour.Should().Be(time.Hour);
            control.dtpMonthlyTime.Value.Minute.Should().Be(time.Minute);
        }

        [Test]
        public void ShouldSelectMonthsTabIfMonthlyOrdinalExpression()
        {
            // Arrange
            var control = new CronControl();
            var ordinal = _intGenerator(0, 4);
            var ordinalText = ordinal < 4 ? $"#{ordinal + 1}" : "L";
            var weekday = _intGenerator(0, 6);
            var months = _decimalGenerator(1, 12);
            var time = _fixture.Create<DateTime>();
            var expression = $"0 {time.Minute} {time.Hour} ? 1/{months} {weekday + 1}{ordinalText} *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(4);
            control.rbtMonthlyOrdinal.Checked.Should().BeTrue();
            control.nudMonthlyMonths2.Value.Should().Be(months);
            control.ddlMonthlyWeekdays.SelectedIndex.Should().Be(weekday);
            control.ddlMonthlyOrdinal.SelectedIndex.Should().Be(ordinal);
            control.dtpMonthlyTime.Value.Hour.Should().Be(time.Hour);
            control.dtpMonthlyTime.Value.Minute.Should().Be(time.Minute);
        }

        [Test]
        public void ShouldSelectYearsTabIfYearlyExpression()
        {
            // Arrange
            var control = new CronControl();
            var day = _decimalGenerator(1, 31);
            var month = _intGenerator(1, 12);
            var time = _fixture.Create<DateTime>();
            var expression = $"0 {time.Minute} {time.Hour} {day} {month} ? *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(5);
            control.nudYearlyDay.Value.Should().Be(day);
            control.ddlYearlyMonths.SelectedIndex.Should().Be(month - 1);
            control.dtpYearlyTime.Value.Hour.Should().Be(time.Hour);
            control.dtpYearlyTime.Value.Minute.Should().Be(time.Minute);
        }

        [Test]
        public void ShouldSelectYearsTabIfYearlyOrdinalExpression()
        {
            // Arrange
            var control = new CronControl();
            var ordinal = _intGenerator(0, 4);
            var ordinalText = ordinal < 4 ? $"#{ordinal + 1}" : "L";
            var weekday = _intGenerator(0, 6);
            var month = _intGenerator(1, 12);
            var time = _fixture.Create<DateTime>();
            var expression = $"0 {time.Minute} {time.Hour} ? {month} {weekday + 1}{ordinalText} *";

            // Act
            control.Value = expression;

            // Assert
            control.tcMain.SelectedIndex.Should().Be(5);
            control.rbtYearlyOrdinal.Checked.Should().BeTrue();
            control.ddlYearlyMonths2.SelectedIndex.Should().Be(month - 1);
            control.ddlYearlyWeekdays.SelectedIndex.Should().Be(weekday);
            control.ddlYearlyOrdinal.SelectedIndex.Should().Be(ordinal);
            control.dtpYearlyTime.Value.Hour.Should().Be(time.Hour);
            control.dtpYearlyTime.Value.Minute.Should().Be(time.Minute);
        }
    }
}
