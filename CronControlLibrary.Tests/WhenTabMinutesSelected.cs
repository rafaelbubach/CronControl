using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace CronControlLibrary.Tests
{
    [TestFixture]
    public class WhenTabMinutesSelected
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
            _generator = () => { return generator.First(x => x > 1 && x <= 3600); };
        }

        [Test]
        public void ShouldBeInputedValue()
        {
            // Arrange
            var minutes = _generator();
            var control = new CronControl();
            control.tcMain.SelectTab("tabMinutes");
            control.nudMinutes.Value = minutes;

            // Act
            var result = control.GetMinutesExpression();

            // Assert
            result.Should().Be($"0 0/{minutes} * 1/1 * ? *");
        }

        [Test]
        public void ShouldThrowOnZero()
        {
            // Arrange
            var control = new CronControl();
            control.tcMain.SelectTab("tabMinutes");

            // Act
            Action result = () => control.nudMinutes.Value = 0;

            // Assert
            result.ShouldThrowExactly<ArgumentOutOfRangeException>();
        }

        [Test]
        public void ShouldThrowOnGreaterThenMax()
        {
            // Arrange
            var control = new CronControl();
            control.tcMain.SelectTab("tabMinutes");

            // Act
            Action result = () => control.nudMinutes.Value = 3600 + _generator();

            // Assert
            result.ShouldThrowExactly<ArgumentOutOfRangeException>();
        }

    }
}
