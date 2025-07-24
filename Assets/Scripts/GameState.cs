using UnityEngine;

internal class GameState : MonoBehaviour
{

	private PlayerState playerState;

	private string swid = string.Empty;

	private ConnectionState connectionState = ConnectionState.DISCONNECTED;

	public PlayerState PlayerState
	{
		get
		{
			return playerState;
		}
		set
		{
			playerState = value;
		}
	}

	public string Swid
	{
		get
		{
			return swid;
		}
		set
		{
			swid = value;
		}
	}

	public ConnectionState ConnectionState
	{
		get
		{
			return connectionState;
		}
		set
		{
			connectionState = value;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
