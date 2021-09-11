extends Node

var crypto_helper_script = load("res://addons/godot-near-sdk/utils/CryptoHelper.cs")

signal transaction_hash_response(tx_hash)

var _server
var port = 3560
const BIND_ADDRESS = "127.0.0.1"

func create_keypair() -> Dictionary:
	var crypto_helper = crypto_helper_script.new()
	crypto_helper.CreateKeyPair()
	var public_key = crypto_helper.publicKey
	var private_key = crypto_helper.privateKey
	
	var keypair = {
		"public_key": public_key,
		"private_key": private_key
	}
	return keypair

func create_transaction(account_id: String, receiver_id: String, \
		method_name: String, args: PoolByteArray, \
		public_key: String, nonce: int, gas: int, deposit: int) -> String:
	
	var block = yield(Near.block_query_latest(), "completed")
	if block.has("error"):
		push_error("Failed to get latest block.")
		return ""
	
	var block_hash = block.header.hash

	var crypto_helper = crypto_helper_script.new()
	var transaction: String = crypto_helper.CreateTransaction(
		account_id, receiver_id, method_name, args, public_key, \
		block_hash, nonce, gas, deposit
	)
	
	return transaction

func create_signed_transaction(account_id: String, receiver_id: String, \
		method_name: String, args: PoolByteArray, private_key: String, \
		public_key: String, nonce: int, gas: int, deposit: int) -> String:
	
	var block = yield(Near.block_query_latest(), "completed")
	if block.has("error"):
		push_error("Failed to get latest block.")
		return ""
	
	var block_hash = block.header.hash

	var crypto_helper = crypto_helper_script.new()
	var signed_transaction: String = crypto_helper.CreateSignedTransaction(
		account_id, receiver_id, method_name, args, private_key, public_key, \
		block_hash, nonce, gas, deposit
	)
	
	return signed_transaction

func save_account_id(account_id: String) -> void:
	# Move the keys from temporary to stored state and add the account id
	var config = Near.near_connection.user_config
	var public_key = config.get_value("temp", "public_key", "")
	var private_key = config.get_value("temp", "private_key", "")
	if !public_key.empty() and !private_key.empty():
		config.set_value("user", "public_key", public_key)
		config.set_value("user", "private_key", private_key)
		config.set_value("user", "account_id", account_id)
		config.erase_section("temp")
		Near.near_connection.save_user_data()
	else:
		push_error("Error retrieving temporary key pair.")

func receive_transaction_hash(tx_hash: String) -> void:
	emit_signal("transaction_hash_response", tx_hash)

# Start a local server to capture wallet login
func listen_for_login():
	stop_server()
	_server = load("res://addons/godot-near-sdk/utils/LoginCapturer.gd").new()
	# TODO: search for open ports if default is unavailable
	var err = _server.listen(port, BIND_ADDRESS)
	if err == OK:
		pass

func listen_for_change_call():
	stop_server()
	_server = load("res://addons/godot-near-sdk/utils/ChangeCallCapturer.gd").new()
	# TODO: search for open ports if default is unavailable
	var err = _server.listen(port, BIND_ADDRESS)
	if err == OK:
		pass

func stop_server() -> void:
	if _server:
		_server.stop()
		_server = null
