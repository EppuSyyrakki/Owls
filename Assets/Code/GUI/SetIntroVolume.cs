using System.Collections;
using UnityEngine;

namespace Owls.GUI
{
	public class SetIntroVolume : MonoBehaviour
	{
		private void Start()
		{
			var vc = GetComponent<VolumeController>();
			bool fx = vc.IsFxEnabled;
			bool music = vc.IsMusicEnabled;
			vc.EnableFx(fx);
			vc.EnableMusic(music);
		}
	}
}