public interface IMineable
{
    string Name { get; set; }
    string Description { get; set; }
    ToolType CanBeMinedWith { get; }
    int MinDropAmount { get; }
    int MaxDropAmount { get; }
    
    int Sound { get; }

    void DropLoot();
}