using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 100.0f;

	private AnimatedSprite2D _sprite;
	private Area2D _interactionArea;
	private Node2D _currentInteractable;
	private Panel _interactionMenu;
	private bool _isInteracting = false;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		// Create interaction area
		_interactionArea = new Area2D();
		CollisionShape2D shape = new CollisionShape2D();
		CircleShape2D circle = new CircleShape2D();
		circle.Radius = 20;
		shape.Shape = circle;
		_interactionArea.AddChild(shape);
		AddChild(_interactionArea);
		
		// Create interaction menu
		_interactionMenu = new Panel();
		_interactionMenu.Size = new Vector2(120, 100);
		_interactionMenu.Position = new Vector2(-60, -120);
		_interactionMenu.Visible = false;
		
		VBoxContainer container = new VBoxContainer();
		container.Size = _interactionMenu.Size;
		
		// Add menu items based on interaction type
		CreateInteractionMenuItems(container);
		
		_interactionMenu.AddChild(container);
		AddChild(_interactionMenu);
		
		// Connect signals
		_interactionArea.AreaEntered += OnAreaEntered;
		_interactionArea.AreaExited += OnAreaExited;
	}

	private void CreateInteractionMenuItems(VBoxContainer container)
	{
		// Plant interactions
		Button waterButton = new Button();
		waterButton.Text = "Water";
		waterButton.Pressed += () => InteractWithObject("water");
		
		Button fertilizeButton = new Button();
		fertilizeButton.Text = "Fertilize";
		fertilizeButton.Pressed += () => InteractWithObject("fertilize");
		
		Button harvestButton = new Button();
		harvestButton.Text = "Harvest";
		harvestButton.Pressed += () => InteractWithObject("harvest");
		
		// Device interactions
		Button useButton = new Button();
		useButton.Text = "Use";
		useButton.Pressed += () => InteractWithObject("use");
		
		// Door interactions
		Button enterButton = new Button();
		enterButton.Text = "Enter";
		enterButton.Pressed += () => InteractWithObject("enter");
		
		container.AddChild(waterButton);
		container.AddChild(fertilizeButton);
		container.AddChild(harvestButton);
		container.AddChild(useButton);
		container.AddChild(enterButton);
		
		// Hide all buttons by default
		foreach (Button button in container.GetChildren())
		{
			button.Visible = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isInteracting) return;

		Vector2 velocity = Vector2.Zero;

		// Handle movement input
		if (Input.IsActionPressed("ui_right"))
			velocity.X += 1;
		if (Input.IsActionPressed("ui_left"))
			velocity.X -= 1;
		if (Input.IsActionPressed("ui_down"))
			velocity.Y += 1;
		if (Input.IsActionPressed("ui_up"))
			velocity.Y -= 1;

		velocity = velocity.Normalized() * Speed;
		Velocity = velocity;
		MoveAndSlide();

		// Update animation
		if (velocity != Vector2.Zero)
		{
			_sprite.Play("walk");
			_sprite.FlipH = velocity.X < 0;
		}
		else
		{
			_sprite.Play("idle");
		}
	}

	public override void _Input(InputEvent @event)
	{
		// Handle interaction input (E key or left mouse click)
		if (_currentInteractable != null && 
			(@event.IsActionPressed("interact") || 
			 (@event is InputEventMouseButton mouseEvent && 
			  mouseEvent.ButtonIndex == MouseButton.Left && 
			  mouseEvent.Pressed)))
		{
			_isInteracting = true;
			_interactionMenu.Visible = true;
			
			// Show/hide appropriate buttons based on interactable type
			UpdateInteractionMenu();
			
			// Position menu at mouse location if clicked
			if (@event is InputEventMouseButton)
			{
				Vector2 mousePos = GetViewport().GetMousePosition();
				_interactionMenu.GlobalPosition = new Vector2(
					mousePos.X - _interactionMenu.Size.X / 2,
					mousePos.Y - _interactionMenu.Size.Y - 10
				);
			}
			else
			{
				// Default position above the player when using E key
				_interactionMenu.Position = new Vector2(-60, -120);
			}
		}
		
		// Close interaction menu when pressing Escape or right-clicking
		if (_isInteracting && 
			(@event.IsActionPressed("ui_cancel") || 
			 (@event is InputEventMouseButton mouseEvent && 
			  mouseEvent.ButtonIndex == MouseButton.Right && 
			  mouseEvent.Pressed)))
		{
			CloseInteractionMenu();
		}
	}

	private void UpdateInteractionMenu()
	{
		VBoxContainer container = _interactionMenu.GetNode<VBoxContainer>(".");
		
		foreach (Button button in container.GetChildren())
		{
			button.Visible = false;
		}
		
		if (_currentInteractable is Plant plant)
		{
			container.GetNode<Button>("Water").Visible = true;
			container.GetNode<Button>("Fertilize").Visible = true;
			container.GetNode<Button>("Harvest").Visible = true;
		}
		else if (_currentInteractable.Name.Contains("tv", StringComparison.OrdinalIgnoreCase) ||
				 _currentInteractable.Name.Contains("computer", StringComparison.OrdinalIgnoreCase))
		{
			container.GetNode<Button>("Use").Visible = true;
		}
		else if (_currentInteractable.Name.Contains("door", StringComparison.OrdinalIgnoreCase) ||
				 _currentInteractable.Name.Contains("outside", StringComparison.OrdinalIgnoreCase) ||
				 _currentInteractable.Name.Contains("inside", StringComparison.OrdinalIgnoreCase))
		{
			container.GetNode<Button>("Enter").Visible = true;
		}
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.IsInGroup("interactable"))
		{
			_currentInteractable = area.GetParent<Node2D>();
		}
	}

	private void OnAreaExited(Area2D area)
	{
		if (area.IsInGroup("interactable") && area.GetParent<Node2D>() == _currentInteractable)
		{
			_currentInteractable = null;
			CloseInteractionMenu();
		}
	}

	private void CloseInteractionMenu()
	{
		_isInteracting = false;
		_interactionMenu.Visible = false;
	}

	private void InteractWithObject(string action)
	{
		if (_currentInteractable == null) return;

		switch (action)
		{
			case "water":
				if (_currentInteractable is Plant plant)
					plant.Water();
				break;
			case "fertilize":
				if (_currentInteractable is Plant plant)
					plant.Fertilize();
				break;
			case "harvest":
				if (_currentInteractable is Plant plant)
					plant.Harvest();
				break;
			case "use":
				if (_currentInteractable.Name.Contains("tv", StringComparison.OrdinalIgnoreCase))
					GD.Print("Using TV...");
				else if (_currentInteractable.Name.Contains("computer", StringComparison.OrdinalIgnoreCase))
					GD.Print("Using Computer...");
				break;
			case "enter":
				if (_currentInteractable.Name.Contains("door", StringComparison.OrdinalIgnoreCase) ||
					_currentInteractable.Name.Contains("outside", StringComparison.OrdinalIgnoreCase) ||
					_currentInteractable.Name.Contains("inside", StringComparison.OrdinalIgnoreCase))
				{
					GD.Print($"Entering through {_currentInteractable.Name}...");
					// You can emit a signal here to handle scene transitions
				}
				break;
		}

		CloseInteractionMenu();
	}
} 
