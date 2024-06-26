﻿using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class ScreenFader : Window
    {
        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _fadeDuration;

        public async UniTask FadeAsync()
        {
            await _fadeImage.DOFade(1, _fadeDuration).AsyncWaitForCompletion();
        }

        public async UniTask UnfadeAsync()
        {
            await _fadeImage.DOFade(0, _fadeDuration).AsyncWaitForCompletion();
        }
    }
}