public class PlayerHealth
{
    private int _poisonWormsEaten = 0;
    private const int MaxHealth = 4;

    public int CurrentHealth => MaxHealth - _poisonWormsEaten;
    public int Max => MaxHealth;

    public event System.Action<int> OnHealthChanged;
    public event System.Action OnDeath;

    public void EatPoisonousWorm()
    {
        _poisonWormsEaten++;
        OnHealthChanged?.Invoke(CurrentHealth);

        if (_poisonWormsEaten >= MaxHealth)
        {
            OnDeath?.Invoke();
        }
    }

    public void Reset()
    {
        _poisonWormsEaten = 0;
        OnHealthChanged?.Invoke(CurrentHealth);
    }
}
