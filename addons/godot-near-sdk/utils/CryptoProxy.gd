extends Node

var crypto_helper_script = load("res://addons/godot-near-sdk/utils/CryptoHelper.cs")
var crypto_helper = crypto_helper_script.new()

func create_keypair() -> Dictionary:
	# TODO: byte[][] not returned properly when converting from C# to GDScript
	var pair = crypto_helper.CreateKeyPair()
	var keypair = {
		"public_key": "",
		"private_key": ""
	}
	return keypair
