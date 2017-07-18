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
    public class WhenTabWeekySelected
    {
        // Arrange
        // Act
        // Assert

        private Fixture _fixture;
        private IList<bool> _checks;
        private IList<bool> Checks => _checks ?? (_checks = _fixture.CreateMany<bool>(7).ToList());
        private IList<bool> OpositeChecks => Checks.Select(c => !c).ToList();

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void ShouldBeInputedValue()
        {
            // Arrange
            var time = _fixture.Create<DateTime>();

            var weekdays = new StringBuilder();
            if (Checks[0]) weekdays.Append(",1");
            if (Checks[1]) weekdays.Append(",2");
            if (Checks[2]) weekdays.Append(",3");
            if (Checks[3]) weekdays.Append(",4");
            if (Checks[4]) weekdays.Append(",5");
            if (Checks[5]) weekdays.Append(",6");
            if (Checks[6]) weekdays.Append(",7");
            weekdays.Remove(0, 1);

            var control = new CronControl();
            control.tcMain.SelectTab("tabWeekly");
            control.dtpWeeklyTime.Value = time;
            control.cbxSunday.Checked = Checks[0];
            control.cbxMonday.Checked = Checks[1];
            control.cbxTuesday.Checked = Checks[2];
            control.cbxWednesday.Checked = Checks[3];
            control.cbxThursday.Checked = Checks[4];
            control.cbxFriday.Checked = Checks[5];
            control.cbxSaturday.Checked = Checks[6];

            // Act
            var result = control.GetWeeklyExpression();

            // Assert
            result.Should().Be($"0 {time.Minute} {time.Hour} ? * {weekdays} *");
        }

        [Test]
        public void ShouldBeOpsiteInputedValue()
        {
            // Arrange
            var time = _fixture.Create<DateTime>();

            var weekdays = new StringBuilder();
            if (OpositeChecks[0]) weekdays.Append(",1");
            if (OpositeChecks[1]) weekdays.Append(",2");
            if (OpositeChecks[2]) weekdays.Append(",3");
            if (OpositeChecks[3]) weekdays.Append(",4");
            if (OpositeChecks[4]) weekdays.Append(",5");
            if (OpositeChecks[5]) weekdays.Append(",6");
            if (OpositeChecks[6]) weekdays.Append(",7");
            weekdays.Remove(0, 1);

            var control = new CronControl();
            control.tcMain.SelectTab("tabWeekly");
            control.dtpWeeklyTime.Value = time;
            control.cbxSunday.Checked = OpositeChecks[0];
            control.cbxMonday.Checked = OpositeChecks[1];
            control.cbxTuesday.Checked = OpositeChecks[2];
            control.cbxWednesday.Checked = OpositeChecks[3];
            control.cbxThursday.Checked = OpositeChecks[4];
            control.cbxFriday.Checked = OpositeChecks[5];
            control.cbxSaturday.Checked = OpositeChecks[6];

            // Act
            var result = control.GetWeeklyExpression();

            // Assert
            result.Should().Be($"0 {time.Minute} {time.Hour} ? * {weekdays} *");
        }
    }
}
