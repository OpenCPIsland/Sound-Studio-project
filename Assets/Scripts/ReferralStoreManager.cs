public class ReferralStoreManager
{
	private static IReferralStoreHelper mReferralStoreHelper;

	private static bool isReferralStoreOpen;

	public static void Show()
	{
		_InitializeReferralStoreHelper();
		mReferralStoreHelper.Show();
		isReferralStoreOpen = true;
	}

	public static void SetClosed()
	{
		isReferralStoreOpen = false;
	}

	public static bool IsReferralStoreOpen()
	{
		return isReferralStoreOpen;
	}

	private static void _InitializeReferralStoreHelper()
	{
		if (mReferralStoreHelper == null)
		{
			mReferralStoreHelper = new ReferralStoreHelperAndroid();
		}
	}
}
