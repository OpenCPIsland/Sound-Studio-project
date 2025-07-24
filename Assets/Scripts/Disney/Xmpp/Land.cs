using Matrix.Xml;

namespace Disney.Xmpp
{
	public class Land : XmppXElement
	{
		public long ID
		{
			get
			{
				return GetTagLong("id");
			}
			set
			{
				SetTag("id", value);
			}
		}

		public int Room
		{
			get
			{
				return GetTagInt("room");
			}
			set
			{
				SetTag("room", value);
			}
		}

		public int World
		{
			get
			{
				return GetTagInt("world");
			}
			set
			{
				SetTag("world", value);
			}
		}

		public Land()
			: base("disney:land:clubpenguin", "land")
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(Land))
			{
				return false;
			}
			Land land = (Land)obj;
			return ID == land.ID && Room == land.Room && World == land.World;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode() ^ Room.GetHashCode() ^ World.GetHashCode();
		}
	}
}
