extends Node

var crypto_helper_script = load("res://addons/godot-near-sdk/utils/CryptoHelper.cs")

func create_keypair() -> Dictionary:
	var crypto_helper = crypto_helper_script.new()
	crypto_helper.CreateKeyPair()
	var public_key_bytes = crypto_helper.publicKey
	var private_key_bytes = crypto_helper.privateKey
	
	# TODO: convert keys to NEAR-compatible strings through base58 encoding
	
	var output = ""
	for b in public_key_bytes:
		output += str(b) + ","
	print("Public key GDScript: " + output)
	output = ""
	for b in private_key_bytes:
		output += str(b) + ","
	print("Private key GDScript: " + output)
	
	var keypair = {
		"public_key": "",
		"private_key": ""
	}
	return keypair
