using UnityEngine;

public class WormFaller : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float fallSpeed = 2f;

    [Header("Wiggle Settings")]
    [SerializeField] private float wiggleSpeed = 10f;
    [SerializeField] private float wiggleAmount = 0.15f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 30f;

    private float wiggleOffset;
    [SerializeField]
    private PlayerController _player;
   
    public void ResetWorm()
    {
        wiggleOffset = Random.Range(0f, Mathf.PI * 2f);
        if (this.gameObject.tag =="Vain")
        {
            fallSpeed = Random.Range(2f, 4f);

        }
        else
        {
            fallSpeed = Random.Range(1f, 3f);

        }
    }

    private void Update()
    {
        if (_player.isPause)
            return;
        float time = Time.time + wiggleOffset;
        float wiggle = Mathf.Sin(time * wiggleSpeed) * wiggleAmount;

        Vector3 move = new Vector3(wiggle, -fallSpeed * Time.deltaTime, 0f);
        transform.position += move;

        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // Return to pool if below screen
        if (Camera.main.WorldToViewportPoint(transform.position).y < -0.1f)
        {
            WormPool.Instance.ReturnWorm(gameObject);
        }
    }
}
