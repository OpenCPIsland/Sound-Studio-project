using DevonLocalization.Core;
using strange.extensions.mediation.impl;

namespace SoundStudio.Controller
{
	public class ImageLocalizerMediator : Mediator
	{
		[Inject]
		public ImageLocalizerView view
		{
			get;
			set;
		}

		public override void OnRegister()
		{
			LocalizeView();
		}

		private void LocalizeView()
		{
			view.image_EN.SetActive(value: false);
			view.image_DE.SetActive(value: false);
			view.image_ES.SetActive(value: false);
			view.image_FR.SetActive(value: false);
			view.image_PT.SetActive(value: false);
			view.image_RU.SetActive(value: false);
			switch (Localizer.Instance.Language)
			{
			case Language.en_US:
				view.image_EN.SetActive(value: true);
				break;
			case Language.de_DE:
				view.image_DE.SetActive(value: true);
				break;
			case Language.es_LA:
				view.image_ES.SetActive(value: true);
				break;
			case Language.fr_FR:
				view.image_FR.SetActive(value: true);
				break;
			case Language.pt_BR:
				view.image_PT.SetActive(value: true);
				break;
			case Language.ru_RU:
				view.image_RU.SetActive(value: true);
				break;
			}
		}
	}
}
