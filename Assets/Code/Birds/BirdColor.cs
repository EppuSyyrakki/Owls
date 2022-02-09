using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Birds
{
    public class BirdColor : MonoBehaviour
    {

        [SerializeField] private Color[] colors = new Color[5]; // Open color list to inspector

        // Color birds randomly from array when they appear.
        void Awake()
        {
            int random = Random.Range(0, colors.Length);

            Color birdColor = colors[random]; // Choose bird color at random
            GetComponent<SpriteRenderer>().color = birdColor;  // Set chosen random color 

        }

    }
}
