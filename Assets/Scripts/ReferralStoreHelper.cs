using Disney.MobileNetwork;
using UnityEngine;

public class ReferralStoreHelper : MonoBehaviour
{
	public ReferralStoreManager EditorReferralStore;

	public ReferralStoreAndroidManager AndroidReferralStore;

	public ReferralStoreIOSManager IOSReferralStore;

	private ReferralStoreManager runtimeStore;

	public void Awake()
	{
	}

	public void Show()
	{
	}
}
