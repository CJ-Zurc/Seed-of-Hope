using Godot;
using System.Collections.Generic;

public partial class GameWorld : Node2D
{
	[Export] public Node2D WorldRoot;
	[Export] public CharacterBody2D Player;

	[Export] public PackedScene FarmScene;
	[Export] public PackedScene HouseScene;

	private Dictionary<string, Node2D> _locations = new();
	private Node2D _currentLocation;

	public override void _Ready()
	{
		LoadLocations();
		TeleportTo("Farm", new Vector2(100, 200));
	}

	private void LoadLocations()
	{
		_locations["Farm"] = FarmScene.Instantiate<Node2D>();
		_locations["House"] = HouseScene.Instantiate<Node2D>();

		foreach (var scene in _locations.Values)
		{
			scene.Visible = false;
			WorldRoot.AddChild(scene);
		}

		_currentLocation = _locations["Farm"];
		_currentLocation.Visible = true;
	}

	public void TeleportTo(string locationName, Vector2 position)
	{
		if (!_locations.ContainsKey(locationName)) return;

		_currentLocation.Visible = false;
		_currentLocation.RemoveChild(Player);

		_currentLocation = _locations[locationName];
		_currentLocation.Visible = true;

		_currentLocation.AddChild(Player);
		Player.GlobalPosition = position;
	}
}
