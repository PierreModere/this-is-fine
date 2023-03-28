using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class Scroller : MonoBehaviour {
    [SerializeField] private SpriteRenderer _spriterRenderer;
    [SerializeField] private float _x, _y;
 
    void Update()
    {
        _spriterRenderer.size = new Vector2(_spriterRenderer.size.x + _x * Time.deltaTime,_spriterRenderer.size.y + _y * Time.deltaTime);
    }
}
 