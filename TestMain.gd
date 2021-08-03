extends Control

onready var label = $Label

func _on_Button_pressed():
	var level_data = yield(Near.test_get_levels(), "completed")
	label.set_text(level_data)

func _on_ClearButton_pressed():
	label.set_text("")
