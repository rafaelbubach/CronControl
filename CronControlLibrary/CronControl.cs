using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CronControlLibrary
{
    public partial class CronControl : UserControl
    {
        public CronControl()
        {
            InitializeComponent();
            ddlMonthlyWeekdays.SelectedIndex = 0;
            ddlMonthlyOrdinal.SelectedIndex = 0;
            ddlYearlyMonths.SelectedIndex = 0;
            ddlYearlyMonths2.SelectedIndex = 0;
            ddlYearlyOrdinal.SelectedIndex = 0;
            ddlYearlyWeekdays.SelectedIndex = 0;
        }

        public string Value
        {
            get
            {
                switch (tcMain.SelectedIndex)
                {
                    case 0:
                        return GetMinutesExpression();
                    case 1:
                        return GetHourlyExpression();
                    case 2:
                        return GetDailyExpression();
                    case 3:
                        return GetWeeklyExpression();
                    case 4:
                        return GetMonthlyExpression();
                    case 5:
                        return GetYearlyExpression();
                    default:
                        return string.Empty;
                }
            }
            set
            {
                if (value == null) return;
                if (Regex.IsMatch(value, @"0 0/\d+ \* 1/1 \* \? \*?", RegexOptions.IgnoreCase))
                {
                    SetMinutes(value);
                    return;
                }
                if (Regex.IsMatch(value, @"0 \d+ \d+/\d+ 1/1 \* \? \*?", RegexOptions.IgnoreCase))
                {
                    SetHourly(value);
                    return;
                }
                if (Regex.IsMatch(value, @"0 \d+ \d+ 1/\d+ \* \? \*?", RegexOptions.IgnoreCase))
                {
                    SetDaily(value);
                    return;
                }
                if (Regex.IsMatch(value, @"0 \d+ \d+ \? \* 2-6 \*?", RegexOptions.IgnoreCase))
                {
                    SetDailyWeekdays(value);
                    return;
                }
                if (Regex.IsMatch(value, @"0 \d+ \d+ \? \* (\d,?)+ \*?", RegexOptions.IgnoreCase))
                {
                    SetWeekly(value);
                    return;
                }
                if (Regex.IsMatch(value, @"0 \d+ \d+ \d+ 1/\d+ \? \*?", RegexOptions.IgnoreCase))
                {
                    SetMonthly(value);
                    return;
                }
                if (Regex.IsMatch(value, @"0 \d+ \d+ \? 1/\d+ \d(#\d|L) \*?", RegexOptions.IgnoreCase))
                {
                    SetMonthlyOrdinal(value);
                    return;
                }
                if (Regex.IsMatch(value, @"0 \d+ \d+ \d+ \d+ \? \*?", RegexOptions.IgnoreCase))
                {
                    SetYearly(value);
                    return;
                }
                if (Regex.IsMatch(value, @"0 \d+ \d+ \? \d+ \d(#\d|L) \*?", RegexOptions.IgnoreCase))
                {
                    SetYearlyOrdinal(value);
                    return;
                }
            }
        }

        private void SetMinutes(string expression)
        {
            var minutes = expression.Split(' ')[1].Replace("0/", string.Empty);
            nudMinutes.Value = Convert.ToDecimal(minutes);
        }

        private void SetHourly(string expression)
        {
            tcMain.SelectTab(1);

            var splited = expression.Split(' ');
            var minutes = splited[1];
            var hours = splited[2].Split('/')[0];
            var repeat = splited[2].Split('/')[1];

            dtpHourlyTime.Value = Convert.ToDateTime($"{hours}:{minutes}");
            nudHourlyHours.Value = Convert.ToDecimal(repeat);
        }

        private void SetDaily(string expression)
        {
            tcMain.SelectTab(2);

            var splited = expression.Split(' ');
            var minutes = splited[1];
            var hours = splited[2];
            var days = splited[3].Split('/')[1];

            dtpDailyTime.Value = Convert.ToDateTime($"{hours}:{minutes}");
            nudDailyDays.Value = Convert.ToDecimal(days);
        }

        private void SetDailyWeekdays(string expression)
        {
            tcMain.SelectTab(2);

            var splited = expression.Split(' ');
            var minutes = splited[1];
            var hours = splited[2];

            rbtDailyWeekDays.Checked = true;
            dtpDailyTime.Value = Convert.ToDateTime($"{hours}:{minutes}");
        }

        private void SetWeekly(string expression)
        {
            tcMain.SelectTab(3);

            var splited = expression.Split(' ');
            var minutes = splited[1];
            var hours = splited[2];
            var weekdays = splited[5].Split(',');

            dtpWeeklyTime.Value = Convert.ToDateTime($"{hours}:{minutes}");
            cbxSunday.Checked = weekdays.Contains("1");
            cbxMonday.Checked = weekdays.Contains("2");
            cbxTuesday.Checked = weekdays.Contains("3");
            cbxWednesday.Checked = weekdays.Contains("4");
            cbxThursday.Checked = weekdays.Contains("5");
            cbxFriday.Checked = weekdays.Contains("6");
            cbxSaturday.Checked = weekdays.Contains("7");
        }

        private void SetMonthly(string expression)
        {
            tcMain.SelectTab(4);

            var splited = expression.Split(' ');
            var minutes = splited[1];
            var hours = splited[2];
            var day = splited[3];
            var months = splited[4].Split('/')[1];

            dtpMonthlyTime.Value = Convert.ToDateTime($"{hours}:{minutes}");
            nudMonthlyDays.Value = Convert.ToDecimal(day);
            nudMonthlyMonths.Value = Convert.ToDecimal(months);
        }

        private void SetMonthlyOrdinal(string expression)
        {
            tcMain.SelectTab(4);

            var splited = expression.Split(' ');
            var minutes = splited[1];
            var hours = splited[2];
            var months = splited[4].Split('/')[1];
            var weekday = Convert.ToInt32(splited[5].Substring(0, 1));
            var ordinal = splited[5].Substring(1);
            var ordinalIndex = ordinal != "L" ? Convert.ToInt32(ordinal.Substring(1)) - 1 : 4;

            rbtMonthlyOrdinal.Checked = true;
            dtpMonthlyTime.Value = Convert.ToDateTime($"{hours}:{minutes}");
            nudMonthlyMonths2.Value = Convert.ToDecimal(months);
            ddlMonthlyWeekdays.SelectedIndex = weekday - 1;
            ddlMonthlyOrdinal.SelectedIndex = ordinalIndex;
        }

        private void SetYearly(string expression)
        {
            tcMain.SelectTab(5);

            var splited = expression.Split(' ');
            var minutes = splited[1];
            var hours = splited[2];
            var day = splited[3];
            var month = splited[4];

            dtpYearlyTime.Value = Convert.ToDateTime($"{hours}:{minutes}");
            nudYearlyDay.Value = Convert.ToDecimal(day);
            ddlYearlyMonths.SelectedIndex = Convert.ToInt32(month) - 1;
        }

        private void SetYearlyOrdinal(string expression)
        {
            tcMain.SelectTab(5);

            var splited = expression.Split(' ');
            var minutes = splited[1];
            var hours = splited[2];
            var month = splited[4];
            var weekday = Convert.ToInt32(splited[5].Substring(0, 1));
            var ordinal = splited[5].Substring(1);
            var ordinalIndex = ordinal != "L" ? Convert.ToInt32(ordinal.Substring(1)) - 1 : 4;

            rbtYearlyOrdinal.Checked = true;
            dtpYearlyTime.Value = Convert.ToDateTime($"{hours}:{minutes}");
            ddlYearlyWeekdays.SelectedIndex = weekday - 1;
            ddlYearlyOrdinal.SelectedIndex = ordinalIndex;
            ddlYearlyMonths2.SelectedIndex = Convert.ToInt32(month) - 1;
        }

        #region RadioButton 

        private void RbtDailyEvery_CheckedChanged(object sender, EventArgs e)
        {
            rbtDailyWeekDays.Checked = !rbtDailyEvery.Checked;
        }

        private void RbtDailyWeekDays_CheckedChanged(object sender, EventArgs e)
        {
            rbtDailyEvery.Checked = !rbtDailyWeekDays.Checked;
        }

        private void RbtMonthlyEvery_CheckedChanged(object sender, EventArgs e)
        {
            rbtMonthlyOrdinal.Checked = !rbtMonthlyEvery.Checked;
        }

        private void RbtMonthlyOrdinal_CheckedChanged(object sender, EventArgs e)
        {
            rbtMonthlyEvery.Checked = !rbtMonthlyOrdinal.Checked;
        }

        private void RbtYearlyEvery_CheckedChanged(object sender, EventArgs e)
        {
            rbtYearlyOrdinal.Checked = !rbtYearlyEvery.Checked;
        }

        private void RbtYearlyOrdinal_CheckedChanged(object sender, EventArgs e)
        {
            rbtYearlyEvery.Checked = !rbtYearlyOrdinal.Checked;
        }

        #endregion

        internal string GetMinutesExpression()
        {
            var minutes = nudMinutes.Value;
            return $"0 0/{minutes} * 1/1 * ? *";
        }

        internal string GetHourlyExpression()
        {
            var hours = nudHourlyHours.Value;
            var time = dtpHourlyTime.Value;
            return $"0 {time.Minute} {time.Hour}/{hours} 1/1 * ? *";
        }

        internal string GetDailyExpression()
        {
            var days = nudDailyDays.Value;
            var time = dtpDailyTime.Value;
            return rbtDailyEvery.Checked
                ? $"0 {time.Minute} {time.Hour} 1/{days} * ? *"
                : $"0 {time.Minute} {time.Hour} ? * 2-6 *";
        }

        public string GetWeeklyExpression()
        {
            var time = dtpWeeklyTime.Value;

            var weekdays = new StringBuilder();
            if (cbxSunday.Checked) weekdays.Append(",1");
            if (cbxMonday.Checked) weekdays.Append(",2");
            if (cbxTuesday.Checked) weekdays.Append(",3");
            if (cbxWednesday.Checked) weekdays.Append(",4");
            if (cbxThursday.Checked) weekdays.Append(",5");
            if (cbxFriday.Checked) weekdays.Append(",6");
            if (cbxSaturday.Checked) weekdays.Append(",7");
            weekdays.Remove(0, 1);

            return $"0 {time.Minute} {time.Hour} ? * {weekdays} *";
        }

        public string GetMonthlyExpression()
        {
            var time = dtpMonthlyTime.Value;
            if (rbtMonthlyEvery.Checked)
            {
                var months = nudMonthlyMonths.Value;
                var days = nudMonthlyDays.Value;
                return $"0 {time.Minute} {time.Hour} {days} 1/{months} ? *";
            }
            else
            {
                var months = nudMonthlyMonths2.Value;
                var ordinal = ddlMonthlyOrdinal.SelectedIndex < 4 ? $"#{ddlMonthlyOrdinal.SelectedIndex + 1}" : "L";
                var weekday = ddlMonthlyWeekdays.SelectedIndex + 1;

                return $"0 {time.Minute} {time.Hour} ? 1/{months} {weekday}{ordinal} *";
            }
        }

        public string GetYearlyExpression()
        {
            var time = dtpYearlyTime.Value;
            if (rbtYearlyEvery.Checked)
            {
                var day = nudYearlyDay.Value;
                var month = ddlYearlyMonths.SelectedIndex + 1;
                return $"0 {time.Minute} {time.Hour} {day} {month} ? *";
            }
            else
            {
                var month = ddlYearlyMonths2.SelectedIndex + 1;
                var ordinal = ddlYearlyOrdinal.SelectedIndex < 4 ? $"#{ddlYearlyOrdinal.SelectedIndex + 1}" : "L";
                var weekday = ddlYearlyWeekdays.SelectedIndex + 1;

                return $"0 {time.Minute} {time.Hour} ? {month} {weekday}{ordinal} *";
            }
        }
    }
}
