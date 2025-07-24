using Matrix;

namespace Disney.XmppInternal
{
	public interface IXmppRoster
	{
		void AddInvitation(string aSwid, Jid aJid);

		bool CheckInvitation(string aName);

		void AddSentInvitation(string aSwid);

		bool CheckSentInvitation(string aSwid);

		void RemoveSentInvitation(string aSwid);
	}
}
