public interface IDamageable
{
    int MaxHealth { get; set; }
    int CurrentHealth { get; set; }
    
    void TakeDamage(int amount);
}