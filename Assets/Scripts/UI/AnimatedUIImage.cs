using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
    public struct AnimationData
    {
        public Sprite[] Sprites;
        public float FramesPerSecond;
        public float TimeBetweenFrames => 1 / FramesPerSecond;
    }
    public class AnimatedUIImage : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private List<AnimationData> animations;
        [SerializeField] private int initialAnimation = -1;
        [SerializeField] private bool isLooping = true;

        private bool isAnimating;
        private Coroutine loop;
        private Sprite[] currentAnim;
        private int currentFrame;
        private int numFrames;
        private readonly SerialDisposable animationDisposable = new();

        private void OnValidate()
        {
            if (animations.Count > initialAnimation && initialAnimation >= 0)
                image.sprite = animations[initialAnimation].Sprites[0];
        }

        private void Awake()
        {
            PlayAnimation(initialAnimation);
        }

        public void PlayAnimation(int id)
        {
            if (animations.Count > id && id >= 0) 
                PlayAnimation(animations[id]);
        }

        public void PlayAnimation(AnimationData anim)
        {
            StopAnimation();
            loop = StartCoroutine(AnimationLoop(anim));
        }

        private IEnumerator AnimationLoop(AnimationData anim)
        {
            currentAnim = anim.Sprites;
            numFrames = currentAnim.Length;
            image.sprite = currentAnim[0];

            isLooping = true;
            while (isLooping)
            {
                yield return new WaitForSeconds(anim.TimeBetweenFrames);
                currentFrame++;
                if (currentFrame >= numFrames)
                {
                    if (!isLooping)
                    {
                        animationDisposable.Disposable = null;
                        isLooping = false;
                        break;
                    }
                    currentFrame = 0;
                }
                image.sprite = currentAnim[currentFrame];
            }
            
        }
        public void StopAnimation()
        {
            if(loop!=null)
                StopCoroutine(loop);
            
            isLooping = false;
            currentFrame = 0;
        }
    }