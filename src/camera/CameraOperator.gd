extends Node3D
class_name CameraOperator

var zoom := 0.5

var target_zoom := 0.5

var rotations = [0, 90, 180, -90]
var rotation_index := 0
var target_rotation := 0

@export var camera_offset := Vector3(0, 1, 0)

@export var walk_speed := 10

@export var max_distance := 10

@export  var zoom_curve : Curve = null

@onready var gimbal_h := $HorizontalGimbal
@onready var gimbal_v := $HorizontalGimbal/VerticalGimbal
@onready var camera := $HorizontalGimbal/VerticalGimbal/Camera


func _ready() -> void:
	gimbal_h.transform.origin = camera_offset


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("camera_zoom_out"):
		target_zoom = clamp(target_zoom - 0.05, 0, 1)
	if event.is_action_pressed("camera_zoom_in"):
		target_zoom = clamp(target_zoom + 0.05, 0, 1)
	if event.is_action_pressed("camera_turn_right"):
		rotation_index = (rotation_index + 1) % rotations.size()
		target_rotation = rotations[rotation_index]
	if event.is_action_pressed("camera_turn_left"):
		rotation_index = rotations.size() - 1 if rotation_index - 1 == -1 else rotation_index - 1
		target_rotation = rotations[rotation_index]


func _process(delta: float) -> void:
	var walk_input := _get_relative_walk_input()

	var new_position := transform.origin + walk_input * walk_speed * delta

	transform.origin = new_position
	
	_process_gimbal_v(delta)


func _process_gimbal_v(_delta: float) -> void:
	zoom = lerp(zoom, target_zoom, 0.1)

	gimbal_v.rotation_degrees.x = lerp(0, -90, zoom_curve.interpolate(zoom))

	camera.translation = Vector3(0, 0, lerp(0, max_distance, zoom))

	rotation.y = lerp_angle(deg2rad(rotation_degrees.y), deg2rad(target_rotation), 0.08)


func _get_relative_walk_input() -> Vector3:
	var relative_input_direction = _get_walk_input().rotated(Vector3(0, 1, 0), rotation.y)
	return relative_input_direction


func _get_walk_input() -> Vector3:
	var left := int(Input.is_action_pressed("camera_left"))
	var right := int(Input.is_action_pressed("camera_right"))
	var forward := int(Input.is_action_pressed("camera_forward"))
	var back := int(Input.is_action_pressed("camera_back"))
	return Vector3(left - right, 0, forward - back).normalized()
