using Godot;

public partial class PlantingSystem : Node
{
    [Export]
    public PackedScene PlantScene { get; set; }
    
    private Inventory _inventory;
    private PlayerStamina _playerStamina;
    private TileMap _farmlandTiles;
    
    public override void _Ready()
    {
        _inventory = GetNode<Inventory>("/root/PlayerInventory");
        _playerStamina = GetNode<PlayerStamina>("/root/MainGame/PlayerStamina");
        _farmlandTiles = GetNode<TileMap>("/root/MainGame/Outside/Farmland");
    }
    
    public bool CanPlantAt(Vector2 position)
    {
        // Convert world position to tile position
        Vector2I tilePos = _farmlandTiles.LocalToMap(position);
        
        // Check if there's a farmland tile at this position
        var cellSourceId = _farmlandTiles.GetCellSourceId(0, tilePos);
        return cellSourceId != -1; // -1 means no tile at this position
    }
    
    public bool PlantSeed(PlantType type, Vector2 position)
    {
        if (!CanPlantAt(position))
        {
            GD.Print("Cannot plant here - not a farmland tile!");
            return false;
        }
        
        if (_playerStamina == null || !_playerStamina.CanPlantSeed())
        {
            GD.Print("Not enough stamina to plant!");
            return false;
        }
        
        // Create and setup the new plant
        var plant = PlantScene.Instantiate<Plant>();
        plant.Initialize(type, _inventory);
        plant.Position = position;
        AddChild(plant);
        
        // Consume stamina for planting
        _playerStamina.ConsumePlantStamina();
        
        GD.Print($"Planted {type} at position {position}");
        return true;
    }
} 