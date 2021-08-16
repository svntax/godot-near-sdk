extends Node

var crypto_helper_script = load("res://addons/godot-near-sdk/utils/CryptoHelper.cs")

func create_keypair() -> Dictionary:
	var crypto_helper = crypto_helper_script.new()
	crypto_helper.CreateKeyPair()
	var public_key = crypto_helper.publicKey
	var private_key = crypto_helper.privateKey
	
	print("Public key GDScript: " + public_key)
	print("Private key GDScript: " + private_key)
	
	var keypair = {
		"public_key": public_key,
		"private_key": private_key
	}
	return keypair
