using Owls.GUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Levels
{
    public class StoryCheck : MonoBehaviour
    {
        public void LoadStoryOrGame()
		{
            var loader = GetComponent<SceneLoader>();

            if (LevelStory.HasNewStory())
			{
                loader.LoadScene(Scenes.LevelStories, false);
                return;
			}

            loader.LoadSelectedScene();
		}
    }
}
