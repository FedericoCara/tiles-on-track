using System;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace Mimic.Effects {

    public class TexturePanner : MonoBehaviour {

        [SerializeField]
        private Vector2 speed = Vector2.one;

        [SerializeField]
        private Renderer renderer;

        protected virtual void Update() {
            renderer.material.mainTextureOffset += speed * Time.deltaTime;
        }
    }

}