extends Node

@onready var clouds_particles_2d = $"../CloudsParticles2D"

func _ready():
	Input.mouse_mode = Input.MOUSE_MODE_HIDDEN;
	clouds_particles_2d.visibility_rect = Rect2(-200, -200, 1000000, 1000000);
