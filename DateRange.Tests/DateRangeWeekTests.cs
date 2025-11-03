
using DateRangeHelpers;

namespace DateRangeTests
{
    public class DateRangeWeekTests
    {
        [Fact]
        public void Does_Monday_Give_Diff_Is_Zero()
        {
            var someMonday = new DateTime(2025, 11, 3);
            DateTimeWeekHelper d = new DateTimeWeekHelper(someMonday);
            Assert.True(d.DaysToMonday == 0);
        }

        [Fact]
        public void Does_Tuesday_Give_Diff_Is_1()
        {
            var someTuesday = new DateTime(2025, 11, 4);
            DateTimeWeekHelper d = new DateTimeWeekHelper(someTuesday);
            Assert.True(d.DaysToMonday == 1);
        }

        [Fact]
        public void Check_Week_From_Monday()
        {
            var someMonday = new DateTime(2025, 11, 3);
            DateTimeWeekHelper d = new DateTimeWeekHelper(someMonday);
            DateRange week = d.GetCurrentDateRange();
            Assert.True(week.startDate.DayOfWeek == DayOfWeek.Monday);
            Assert.True(week.endDate.DayOfWeek == DayOfWeek.Monday);
        }

        [Fact]
        public void Check_Week_From_Thursday()
        {
            var someThursday = new DateTime(2025, 11, 6);
            DateTimeWeekHelper d = new DateTimeWeekHelper(someThursday);
            DateRange week = d.GetCurrentDateRange();
            Assert.True(week.startDate.DayOfWeek == DayOfWeek.Monday);
            Assert.True(week.endDate.DayOfWeek == DayOfWeek.Monday);
        }

        [Fact]
        public void Check_NextWeek_From_Thursday()
        {
            var someThursday = new DateTime(2025, 11, 6);
            DateTimeWeekHelper d = new DateTimeWeekHelper(someThursday);
            d.GoToNextDateRange();
            DateRange week = d.GetCurrentDateRange();
            Assert.True(week.startDate.DayOfWeek == DayOfWeek.Monday);
            Assert.True(week.endDate.DayOfWeek == DayOfWeek.Monday);
        }

        [Fact]
        public void Check_PreviousWeek_From_Thursday()
        {
            var someThursday = new DateTime(2025, 11, 6);
            DateTimeWeekHelper d = new DateTimeWeekHelper(someThursday);
            d.GoToPreviousDateRange();

            DateRange week = d.GetCurrentDateRange();
            Assert.True(week.startDate.DayOfWeek == DayOfWeek.Monday);
            Assert.True(week.endDate.DayOfWeek == DayOfWeek.Monday);
        }

        [Fact]
        public void Check_PreviousWeek_From_YearStart()
        {
            DateTimeWeekHelper d = new DateTimeWeekHelper(new DateTime(2025, 1, 1));
            d.GoToPreviousDateRange();
            DateRange week = d.GetCurrentDateRange();
            Assert.True(week.startDate.DayOfWeek == DayOfWeek.Monday);
            Assert.True(week.endDate.DayOfWeek == DayOfWeek.Monday);
        }

        [Fact]
        public void Check_Next_Week_From_YearStart_Is_7_Days_Ahead()
        {
            DateTimeWeekHelper d = new DateTimeWeekHelper(new DateTime(2025, 1, 1));
            DateRange week = d.GetCurrentDateRange();
            d.GoToNextDateRange();
            DateRange weekLater = d.GetCurrentDateRange();
            Assert.True(week.startDate.DayOfWeek == DayOfWeek.Monday);
            Assert.True(weekLater.endDate.DayOfWeek == DayOfWeek.Monday);
            int startDaysDiff = weekLater.startDate.DayNumber - week.startDate.DayNumber;
            int endDaysDiff = weekLater.endDate.DayNumber - week.endDate.DayNumber;
            Assert.True(startDaysDiff == 7);
            Assert.True(endDaysDiff == 7);
        }

        [Fact]
        public void Check_Next_Week_From_YearStart_Is_7_Days_Ahead_Change_Year()
        {
            IGetDateRange d = new DateTimeWeekHelper(new DateTime(2024, 12, 30));
            DateRange week = d.GetCurrentDateRange();
            d.GoToNextDateRange();
            DateRange weekLater = d.GetCurrentDateRange();
            Assert.True(week.startDate.DayOfWeek == DayOfWeek.Monday);
            Assert.True(weekLater.endDate.DayOfWeek == DayOfWeek.Monday);
            int startDaysDiff = weekLater.startDate.DayNumber - week.startDate.DayNumber;
            int endDaysDiff = weekLater.endDate.DayNumber - week.endDate.DayNumber;
            Assert.True(startDaysDiff == 7);
            Assert.True(endDaysDiff == 7);
        }

        [Fact]
        public void Check_Previous_Week_From_YearStart_Is_7_Days_Before_Change_Year()
        {
            IGetDateRange d = new DateTimeWeekHelper(new DateTime(2025, 1, 3));
            DateRange week = d.GetCurrentDateRange();
            d.GoToPreviousDateRange();
            DateRange weekLater = d.GetCurrentDateRange();
            Assert.True(week.startDate.DayOfWeek == DayOfWeek.Monday);
            Assert.True(weekLater.endDate.DayOfWeek == DayOfWeek.Monday);
            int startDaysDiff = weekLater.startDate.DayNumber - week.startDate.DayNumber;
            int endDaysDiff = weekLater.endDate.DayNumber - week.endDate.DayNumber;
            Assert.True(startDaysDiff == -7);
            Assert.True(endDaysDiff == -7);
        }
    }
}
