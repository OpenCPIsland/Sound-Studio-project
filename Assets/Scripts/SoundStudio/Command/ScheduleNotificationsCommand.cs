using DevonLocalization.Core;
using strange.extensions.command.impl;
using System;
using System.Globalization;
using System.Threading;

namespace SoundStudio.Command
{
	public class ScheduleNotificationsCommand : EventCommand
	{
		public const DayOfWeek NOTIFICATION_DAY = DayOfWeek.Saturday;

		public const int NOTIFOCATION_HOUR = 8;

		public const int NOTIFICATION_MINUTE = 58;

		public const int NOTIFICATION_WEEKS = 6;

		public const int DAYS_IN_WEEK = 7;

		private readonly string[] NOTIFICATION_TOKENS = new string[9]
		{
			"soundstudio.notifications.1",
			"soundstudio.notifications.2",
			"soundstudio.notifications.3",
			"soundstudio.notifications.4",
			"soundstudio.notifications.5",
			"soundstudio.notifications.6",
			"soundstudio.notifications.8",
			"soundstudio.notifications.9",
			"soundstudio.notifications.10"
		};

		[Inject]
		public LocalNotificationPlugin localNotificationPlugin
		{
			get;
			set;
		}

		public override void Execute()
		{
			ClearNotifications();
			ScheduleNextNotifications();
		}

		private void ClearNotifications()
		{
			localNotificationPlugin.cancelAllNotifications();
		}

		private void ScheduleNextNotifications()
		{
			DateTime now = DateTime.Now;
			string tokenTranslation = Localizer.Instance.GetTokenTranslation("soundstudio.notifications.title");
			for (int i = 0; i < 6; i++)
			{
				DateTime dateTime = CreateNotificationDateTime(now, i, DayOfWeek.Saturday, 8, 58);
				if (dateTime > now)
				{
					string tokenTranslation2 = Localizer.Instance.GetTokenTranslation(GetTokenForDateTime(dateTime));
					localNotificationPlugin.scheduleLocalNotification((int)dateTime.Subtract(now).TotalSeconds, tokenTranslation2, tokenTranslation);
				}
			}
		}

		public static DateTime CreateNotificationDateTime(DateTime now, int weekIndex, DayOfWeek notificationDay, int notificationHour, int notificationMinute)
		{
			return Utils.GetNextWeekday(now.Date.AddDays(7 * weekIndex), notificationDay).AddHours(notificationHour).AddMinutes(notificationMinute);
		}

		public string GetTokenForDateTime(DateTime date)
		{
			CalendarWeekRule rule = CalendarWeekRule.FirstFourDayWeek;
			DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
			Calendar calendar = Thread.CurrentThread.CurrentCulture.Calendar;
			int weekOfYear = calendar.GetWeekOfYear(date, rule, firstDayOfWeek);
			int num = weekOfYear % NOTIFICATION_TOKENS.Length;
			return NOTIFICATION_TOKENS[num];
		}
	}
}
