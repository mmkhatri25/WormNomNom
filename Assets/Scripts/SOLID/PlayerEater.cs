using UnityEngine;

public class PlayerEater
{
    private readonly Transform _playerTransform;
    private readonly Animator _animator;
    private readonly float _eatRange = 1.5f;
    private float _currentFatness = 1f;
    private readonly float _maxFatness = 2f;
    private readonly float _fatnessStep = 0.1f;
   // [SerializeField] private LayerMask wormLayer; // Assign in inspector only to "Worm" layer
    public PlayerEater(Transform playerTransform, Animator animator)
    {
        _playerTransform = playerTransform;
        _animator = animator;
    }

    public void CheckAndEatNearbyWorm(LayerMask wormLayer)
    {

        RaycastHit hit;
        if (Physics.Raycast(_playerTransform.position, Vector3.down, out hit, _eatRange))
        {

            PlayEatAnimation();

            IWorm worm = hit.collider.GetComponent<IWorm>();
            if (worm != null)
            {
                Debug.Log("hit layer - " + hit.transform.gameObject.name);

                worm.Eat();
                GrowFat();
            }
        }
    }

    public void PlayEatAnimation()
    {
        //_animator.Play("Eat");
        Debug.Log("PlayEatAnimation - ");

        _animator.SetTrigger("Jump");
    }

    private void GrowFat()
    {
        if (_currentFatness < _maxFatness)
        {
            _currentFatness += _fatnessStep;
            _playerTransform.localScale = Vector3.one * _currentFatness;
        }
    }
}
