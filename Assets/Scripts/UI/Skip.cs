using UnityEngine;
using UnityEngine.Video;

namespace UI
{
    public class Skip : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private AudioSource _source; 

        private void Start()
        {
            _player.GetComponent<VideoPlayer>().loopPointReached += Kill;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Kill(_player.GetComponent<VideoPlayer>());
            }
        }

        private void Kill(VideoPlayer vp)
        {
            Destroy(_player);
            _source.enabled = true;
            Destroy(gameObject);
        }
    }
}
