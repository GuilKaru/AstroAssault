using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AstroAssault
{
    public class ScrollingBackground : MonoBehaviour
    {
        public float speed;

        [SerializeField]
        private Renderer _bgRenderer;

        private void Update()
        {
            if (!GameManager.gameManager.gameStarted) return;
            if (GameManager.gameManager.gamePaused) return;
            
            _bgRenderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
        }
    }
}
