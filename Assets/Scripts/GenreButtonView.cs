using DevonLocalization.Core;
using SoundStudio.Model;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class GenreButtonView : View
{
	public delegate void GenreSelectedAction(GenreVO genre);

	public Text genreNameText;

	private Button button;

	private AudioSource sound;

	private GenreVO genre;

	public event GenreSelectedAction OnGenreSelected;

	protected override void Awake()
	{
		base.Awake();
		sound = base.gameObject.GetComponent<AudioSource>();
		button = (base.gameObject.GetComponent("Button") as Button);
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(delegate
		{
			OnClick();
		});
	}

	public void loadGenre(GenreVO genre)
	{
		this.genre = genre;
		genreNameText.text = Localizer.Instance.GetTokenTranslation(genre.name);
		sound.clip = Resources.Load<AudioClip>(genre.buttonSound);
		if (!(sound.clip == null))
		{
		}
	}

	public void OnClick()
	{
		sound.Play();
		Invoke("OnClickDelay", base.GetComponent<AudioSource>().clip.length);
	}

	public void OnClickDelay()
	{
		if (this.OnGenreSelected != null)
		{
			this.OnGenreSelected(genre);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CancelInvoke("OnClickDelay");
	}
}
