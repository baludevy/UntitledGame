public interface IMineable : IDamageable
{
    string Name { get; set; }
    string Description { get; set; }
    ToolType CanBeMinedWith { get; }
    DroppedItem DropPrefab { get; }
    int MinDropAmount { get; }
    int MaxDropAmount { get; }
    
    bool CanBeMined { get; }

    void DropLoot();
}