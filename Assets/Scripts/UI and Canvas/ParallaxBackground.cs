using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] private Vector2 parallaxEffectMultiplier;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;

    private SpriteRenderer spriteRenderer;

    [Header("Day/Night Sprites")]
    [SerializeField] private Sprite morningSprite;
    [SerializeField] private Sprite nightSprite;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();

        Sprite sprite = spriteRenderer.sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    void FixedUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        transform.position += new Vector3(
            deltaMovement.x * parallaxEffectMultiplier.x,
            deltaMovement.y * parallaxEffectMultiplier.y
        );

        lastCameraPosition = cameraTransform.position;

        if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX =
                (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;

            transform.position =
                new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
        }
    }

    // ======================
    // DAY / NIGHT SWITCH
    // ======================

    public void SetMorning()
    {
        if (morningSprite != null)
            spriteRenderer.sprite = morningSprite;
    }

    public void SetNight()
    {
        if (nightSprite != null)
            spriteRenderer.sprite = nightSprite;
    }
}