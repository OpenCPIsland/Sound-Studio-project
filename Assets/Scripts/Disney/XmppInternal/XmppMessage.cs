using Disney.Utility;
using Disney.Xmpp;
using Matrix;
using Matrix.Xmpp;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Muc.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Disney.XmppInternal
{
	public class XmppMessage
	{
		private enum TimeStamp
		{
			YEAR,
			MONTH,
			DAY,
			HOUR,
			MINUTE,
			SECOND
		}

		private IXmppManagerInternal manager;

		private IXmppRoom xmppRoom;

		public XmppMessage(IXmppManagerInternal aManager, IXmppRoom aRoom)
		{
			manager = aManager;
			xmppRoom = aRoom;
		}

		public void SendMessage(string user, string message)
		{
			ChatMessage chatMessage = new ChatMessage();
			chatMessage.Type = MessageType.chat;
			chatMessage.To = user + XmppConsts.DOMAIN;
			chatMessage.Body = message;
			chatMessage.Product = new Product();
			chatMessage.Location = new Location
			{
				Room = manager.GetMySwid() + ":" + user + XmppConsts.DOMAIN
			};
			chatMessage.Version = new Disney.Xmpp.Version();
			ChatMessage chatMessage2 = chatMessage;
			chatMessage2.XmlLanguage = "en";
			manager.Send(chatMessage2);
		}

		public void SendGroupMessage(string room, string message)
		{
			ChatMessage chatMessage = new ChatMessage();
			chatMessage.Type = MessageType.groupchat;
			chatMessage.To = room + XmppConsts.MUC_DOMAIN;
			chatMessage.Body = message;
			chatMessage.Product = new Product();
			chatMessage.Location = new Location
			{
				Room = room + XmppConsts.MUC_DOMAIN
			};
			chatMessage.Version = new Disney.Xmpp.Version();
			ChatMessage chatMessage2 = chatMessage;
			chatMessage2.XmlLanguage = "en";
			manager.Send(chatMessage2);
		}

		public void SendSticker(string user, string name)
		{
			AnimationStickerMessage animationStickerMessage = new AnimationStickerMessage();
			animationStickerMessage.To = user + XmppConsts.DOMAIN;
			animationStickerMessage.From = manager.GetMySwid() + XmppConsts.DOMAIN;
			animationStickerMessage.XmlLanguage = "en";
			animationStickerMessage.StickerMetaMessage = new AnimationStickerMetaMessage
			{
				Type = "disney:friends:animation",
				Sticker = new AnimationSticker
				{
					Type = "animation",
					TypeName = name
				}
			};
			AnimationStickerMessage aElement = animationStickerMessage;
			manager.Send(aElement);
		}

		public void SendGroupSticker(string room, string name)
		{
			MetaMessage metaMessage = new MetaMessage();
			metaMessage.SetValue(name);
			metaMessage.Type = "disney:spark:muc:sticker";
			CustomGroupChatMetaMessage customGroupChatMetaMessage = new CustomGroupChatMetaMessage();
			customGroupChatMetaMessage.Type = MessageType.groupchat;
			customGroupChatMetaMessage.To = room + XmppConsts.MUC_DOMAIN;
			customGroupChatMetaMessage.From = manager.GetMySwid() + XmppConsts.DOMAIN + XmppConsts.RESOURCE;
			customGroupChatMetaMessage.Product = new Product();
			customGroupChatMetaMessage.Location = new Location
			{
				Room = room + XmppConsts.MUC_DOMAIN
			};
			customGroupChatMetaMessage.MetaMessage = metaMessage;
			CustomGroupChatMetaMessage customGroupChatMetaMessage2 = customGroupChatMetaMessage;
			customGroupChatMetaMessage2.Body = null;
			customGroupChatMetaMessage2.XmlLanguage = "en";
			manager.Send(customGroupChatMetaMessage2);
		}

		public void SendGogChallenge(string user, string message)
		{
			CustomMetaMessage customMetaMessage = new CustomMetaMessage();
			customMetaMessage.Type = MessageType.normal;
			customMetaMessage.To = user + XmppConsts.DOMAIN;
			customMetaMessage.From = manager.GetMySwid() + XmppConsts.DOMAIN;
			customMetaMessage.XmlLanguage = "en";
			customMetaMessage.Body = CdataWrap(message);
			customMetaMessage.MetaMessage = new MetaMessage
			{
				Type = "disney:spark:gog:challenge"
			};
			CustomMetaMessage aElement = customMetaMessage;
			manager.Send(aElement);
		}

		public void SendGogResults(string user, string message)
		{
			CustomMetaMessage customMetaMessage = new CustomMetaMessage();
			customMetaMessage.Type = MessageType.normal;
			customMetaMessage.To = user + XmppConsts.DOMAIN;
			customMetaMessage.From = manager.GetMySwid() + XmppConsts.DOMAIN;
			customMetaMessage.XmlLanguage = "en";
			customMetaMessage.Body = CdataWrap(message);
			customMetaMessage.MetaMessage = new MetaMessage
			{
				Type = "disney:spark:gog:results"
			};
			CustomMetaMessage aElement = customMetaMessage;
			manager.Send(aElement);
		}

		public void SendGag(string user, string message)
		{
			CustomMetaMessage customMetaMessage = new CustomMetaMessage();
			customMetaMessage.To = user + XmppConsts.DOMAIN;
			customMetaMessage.From = manager.GetMySwid() + XmppConsts.DOMAIN;
			customMetaMessage.XmlLanguage = "en";
			customMetaMessage.Body = CdataWrap(message);
			customMetaMessage.MetaMessage = new MetaMessage
			{
				Type = "disney:spark:gag"
			};
			CustomMetaMessage aElement = customMetaMessage;
			manager.Send(aElement);
		}

		public void SendUgc(string user, string message)
		{
			CustomMetaMessage customMetaMessage = new CustomMetaMessage();
			customMetaMessage.To = user + XmppConsts.DOMAIN;
			customMetaMessage.From = manager.GetMySwid() + XmppConsts.DOMAIN;
			customMetaMessage.XmlLanguage = "en";
			customMetaMessage.Body = CdataWrap(message);
			customMetaMessage.MetaMessage = new MetaMessage
			{
				Type = "disney:spark:ugc"
			};
			CustomMetaMessage aElement = customMetaMessage;
			manager.Send(aElement);
		}

		public void ParseMessage(Message message)
		{
			Jid from = message.From;
			string user = message.From.User;
			string body = message.Body;
			Dictionary<string, XElement> dictionary = new Dictionary<string, XElement>();
			foreach (XElement item in message.Elements())
			{
				string key = item.Name.ToString();
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, item);
				}
			}
			DateTime aSent = DateTime.Now;
			if (dictionary.ContainsKey("{urn:xmpp:delay}delay"))
			{
				XAttribute xAttribute = dictionary["{urn:xmpp:delay}delay"].Attribute("stamp");
				if (xAttribute != null)
				{
					string value = xAttribute.Value;
					string[] array = value.Split('-', 'T', ':');
					try
					{
						aSent = new DateTime(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]));
					}
					catch (FormatException ex)
					{
						Util.Log("Date sent string conversion fail " + ex.ToString());
					}
				}
			}
			switch (message.Type)
			{
			case MessageType.error:
				break;
			case MessageType.chat:
				manager.SendOnChatMessageReceived(user, aSent, body);
				break;
			case MessageType.groupchat:
			{
				X xMucUser = message.XMucUser;
				if (xMucUser != null)
				{
					if (xMucUser.HasStatus(104) && xMucUser.HasStatus(172))
					{
						xmppRoom.EnableInviting();
						manager.EnterRoom(user);
					}
				}
				else if (body.Length > 0)
				{
					string mySwid = manager.GetMySwid();
					if (from.Resource != mySwid)
					{
						manager.SendOnGroupChatMessageReceived(from, aSent, body);
					}
				}
				else
				{
					if (!dictionary.ContainsKey("{disney:social:metamessage}meta"))
					{
						break;
					}
					XElement xElement2 = dictionary["{disney:social:metamessage}meta"];
					XAttribute xAttribute4 = xElement2.Attribute("type");
					if (xAttribute4 == null)
					{
						break;
					}
					switch (xAttribute4.Value)
					{
					case "disney:spark:muc:members":
					{
						string value2 = xElement2.Value;
						manager.SendOnRoomMembers(user, value2);
						break;
					}
					case "disney:spark:muc:sticker":
						manager.SendOnGroupStickerMessageReceived(from, aSent, xElement2.Value);
						break;
					case "disney:spark:muc:leave":
						if (from.Resource != manager.GetMySwid())
						{
							manager.SendOnRoomMemberLeave(user, from.Resource);
						}
						break;
					}
				}
				break;
			}
			case MessageType.normal:
			{
				X xMucUser = message.XMucUser;
				if (xMucUser != null)
				{
					Invite invite = xMucUser.GetInvites().First();
					if (invite != null)
					{
						manager.EnterRoom(user);
						manager.SendOnRoomReceived(user);
					}
				}
				else
				{
					if (!dictionary.ContainsKey("{disney:social:metamessage}meta"))
					{
						break;
					}
					XElement xElement = dictionary["{disney:social:metamessage}meta"];
					XAttribute xAttribute2 = xElement.Attribute("type");
					if (xAttribute2 != null)
					{
						switch (xAttribute2.Value)
						{
						case "disney:friends:animation":
						{
							IEnumerable<XElement> enumerable = xElement.Elements();
							foreach (XElement item2 in enumerable)
							{
								XAttribute xAttribute3 = item2.Attribute("type");
								if (xAttribute3 != null && xAttribute3.Value == "animation")
								{
									manager.SendOnStickerMessageReceived(user, aSent, item2.Attribute("name").Value);
									break;
								}
							}
							break;
						}
						case "disney:spark:gog:challenge":
							manager.SendOnGogChallengeReceived(user, aSent, CdataUnwrap(body));
							break;
						case "disney:spark:gog:results":
							manager.SendOnGogResultsReceived(user, aSent, CdataUnwrap(body));
							break;
						case "disney:spark:gag":
							manager.SendOnGagReceived(user, aSent, CdataUnwrap(body));
							break;
						case "disney:spark:ugc":
							manager.SendOnUgcReceived(user, aSent, CdataUnwrap(body));
							break;
						}
					}
				}
				break;
			}
			}
		}

		private string CdataWrap(string aMessage)
		{
			return "<![CDATA[" + aMessage + "]]>";
		}

		private string CdataUnwrap(string aMessage)
		{
			int num = aMessage.IndexOf("CDATA[") + 6;
			int num2 = aMessage.LastIndexOf("]]");
			return aMessage.Substring(num, num2 - num);
		}

		private string GetRoomOwnerFromRoomName(string aRoom)
		{
			string[] array = aRoom.Split('{', '}');
			if (array.Length > 2)
			{
				return "{" + array[1] + "}";
			}
			return string.Empty;
		}
	}
}
