public interface IMineable
{
    string Name { get; set; }
    string Description { get; set; }
    ToolType CanBeMinedWith { get; }
    DroppedItem DropPrefab { get; }
    int MinDropAmount { get; }
    int MaxDropAmount { get; }

    void DropLoot();
}