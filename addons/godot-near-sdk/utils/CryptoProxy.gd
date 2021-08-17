extends Node

var crypto_helper_script = load("res://addons/godot-near-sdk/utils/CryptoHelper.cs")

var _server

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

func save_account_id(account_id: String) -> void:
	# Move the keys from temporary to stored state and add the account id
	var config = Near.near_connection.user_config
	var public_key = config.get_value("temp", "public_key", null)
	var private_key = config.get_value("temp", "private_key", null)
	if public_key != null and private_key != null:
		config.set_value("user", "public_key", public_key)
		config.set_value("user", "private_key", private_key)
		config.set_value("user", "account_id", account_id)
		config.erase_section("temp")
		Near.near_connection.save_user_data()
	else:
		push_error("Error retrieving temporary key pair.")

# Start a local server to capture wallet login
# https://github.com/xaltaq/GDHTTPServer
func listen_for_login():
	stop_login_server()
	var port = 3560
	var ip = "127.0.0.1"
	_server = load("res://addons/godot-near-sdk/utils/LoginCapturer.gd").new()
	var err = _server.listen(port, ip)
	if err == OK:
		pass

func stop_login_server() -> void:
	if _server:
		_server.stop()
		_server = null
