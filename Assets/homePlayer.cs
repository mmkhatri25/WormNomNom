using UnityEngine;

public class homePlayer : MonoBehaviour
{
    public GameObject[] characters; // Child character models
    private GameObject activeCharacter;
    private Animator activeAnimator;
    private int currentCharacterIndex = -1;
    private float moveSpeed = 2f;
    private float direction = 1f;

    void Start()
    {
        currentCharacterIndex = PlayerPrefs.GetInt("CurrentCharacter", 0);
        ActivateCharacter(currentCharacterIndex);
        UpdateRotation();
        TriggerWalk();
    }

    void Update()
    {
        int selectedIndex = PlayerPrefs.GetInt("CurrentCharacter", 0);
        if (selectedIndex != currentCharacterIndex)
        {
            currentCharacterIndex = selectedIndex;
            ActivateCharacter(currentCharacterIndex);
            TriggerWalk();
        }

        // Move parent object
        Vector3 pos = transform.position;
        pos.x += moveSpeed * direction * Time.deltaTime;

        if (pos.x >= 5.5f || pos.x <= -5.5f)
        {
            direction *= -1f;
            UpdateRotation();
            pos.x = Mathf.Clamp(pos.x, -5.5f, 6f);
        }

        transform.position = pos;
    }

    void ActivateCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            bool isActive = i == index;
            characters[i].SetActive(isActive);
            if (isActive)
            {
                activeCharacter = characters[i];
                activeAnimator = activeCharacter.GetComponent<Animator>();
            }
        }

        direction = 1f;
        UpdateRotation();
    }

    void UpdateRotation()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.y = direction > 0 ? 135f : -135f;
        transform.eulerAngles = rotation;
    }

    void TriggerWalk()
    {
        if (activeAnimator != null)
        {
            activeAnimator.SetTrigger("Walk");
        }
    }
}
