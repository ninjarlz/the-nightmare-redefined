
using UnityEngine;

public class GridPoint : MonoBehaviour
{

    private const string RED_COLOUR = "red";
    private const string GREEN_COLOUR = "green";

    [SerializeField] private Sprite _red;
    private Sprite _green;
    [SerializeField] private SpriteRenderer _sprite;

    [SerializeField] private bool _buildable = true;
    public bool Buildable
    {
        get { return _buildable; }
        set
        {
            _buildable = value;
            changeSprite(value);
        }
    }

    private void Start()
    {
        InitGridSprites();
    }

    public void setSpriteRenderer(GameObject gridRender)
    {
        _sprite = gridRender.GetComponent<SpriteRenderer>();
    }

    public SpriteRenderer getSpriteRenderer()
    {
        return _sprite;
    }

    public void changeSprite(bool isGreen)
    {
        Debug.Log("Changing colour");
        if (isGreen) 
        {
            _sprite.sprite = _green;
        }
        else
        {
            Debug.Log("Changing to red");
            _sprite.sprite = _red;
        }

    }

    private void InitGridSprites()
    {
        _red = (Sprite)Resources.Load(RED_COLOUR, typeof(Sprite));
        _green = (Sprite)Resources.Load(GREEN_COLOUR, typeof(Sprite));
    }
}
