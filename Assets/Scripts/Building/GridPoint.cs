
using UnityEngine;

namespace Building
{
    public class GridPoint : MonoBehaviour
    {

        private const string RedColour = "red";
        private const string GreenColour = "green";

        [SerializeField] private Sprite _red;
        private Sprite _green;
        
        [SerializeField] private SpriteRenderer _sprite;
        public SpriteRenderer SpriteRenderer
        {
            get => _sprite;
            set => _sprite = value;
        }
        
        [SerializeField] private bool _buildable = true;
        public bool Buildable
        {
            get => _buildable;
            set
            {
                _buildable = value;
                ChangeSprite(value);
            }
        }

        private void Start()
        {
            InitGridSprites();
        }

        private void ChangeSprite(bool isBuildable)
        {
            if (isBuildable) 
            {
                _sprite.sprite = _green;
                return;
            }
            _sprite.sprite = _red;
        }

        private void InitGridSprites()
        {
            _red = (Sprite) Resources.Load(RedColour, typeof(Sprite));
            _green = (Sprite) Resources.Load(GreenColour, typeof(Sprite));
        }
    }
}
