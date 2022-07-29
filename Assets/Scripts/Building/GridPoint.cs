
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

        public void SetSpriteRenderer(GameObject gridRender)
        {
            _sprite = gridRender.GetComponent<SpriteRenderer>();
        }

        public SpriteRenderer GetSpriteRenderer()
        {
            return _sprite;
        }

        private void ChangeSprite(bool isGreen)
        {
            Debug.Log("Changing colour");
            if (isGreen) 
            {
                Debug.Log("Changing to green");
                _sprite.sprite = _green;
                return;
            }
            Debug.Log("Changing to red");
            _sprite.sprite = _red;
        }

        private void InitGridSprites()
        {
            _red = (Sprite) Resources.Load(RedColour, typeof(Sprite));
            _green = (Sprite) Resources.Load(GreenColour, typeof(Sprite));
        }
    }
}
