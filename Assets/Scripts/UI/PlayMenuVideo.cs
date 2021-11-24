using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace UI
{
    public class PlayMenuVideo : MonoBehaviour
    {
        [SerializeField] private RawImage _image;
        [SerializeField] private VideoPlayer _player;
    
        void Start()
        {
            StartCoroutine(PlayVideo());
        }

        IEnumerator PlayVideo()
        {
            _player.Prepare();
            WaitForSeconds wait = new WaitForSeconds(1);
            while (!_player.isPrepared)
            {
                yield return wait;
                break;
            }

            _image.texture = _player.texture;
            _player.Play();
        }
    }
}
