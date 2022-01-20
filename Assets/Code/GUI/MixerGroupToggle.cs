using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Owls.GUI
{
    public class MixerGroupToggle : MonoBehaviour
    {
        private const string VOLUME = "Volume";

        [SerializeField]
        private AudioMixerGroup mixerGroup = null;

        private bool isMuted = false;



		public void Mute()
		{
            mixerGroup.audioMixer.SetFloat(VOLUME, -80f);
		}

        public void Unmute()
		{

		}
    }
}
