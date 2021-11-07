extends Reference
class_name WalletConnection

const LOGIN_WALLET_URL_SUFFIX = "/login/"
const TX_SIGNING_URL_SUFFIX = "/sign"

signal user_signed_in()
signal user_signed_out()
signal transaction_hash_received(tx_hash)

var _near_connection: NearConnection

var account_id: String setget ,get_account_id
var access_key_nonce: int

func _init(near_connection: NearConnection):
	_near_connection = near_connection
	_check_signed_in()
	_near_connection.connect("user_data_updated", self, "_on_user_data_updated")

func get_account_id() -> String:
	return account_id

func get_private_key() -> String:
	return _near_connection.user_config.get_value("user", "private_key", "")

func get_public_key() -> String:
	return _near_connection.user_config.get_value("user", "public_key", "")

func _has_enough_allowance():
	var public_key = get_public_key()
	var result = Near.view_access_key(account_id, public_key)
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		return result.error
	else:
		var allowance: String = result.permission.FunctionCall.allowance
		return CryptoProxy.is_enough_allowance(allowance)

func sign_in(contract_id: String) -> void:
	if is_signed_in():
		emit_signal("user_signed_in", self)
		return
	_sign_in_process(contract_id)

func _sign_in_process(contract_id: String) -> void:
	var keypair = CryptoProxy.create_keypair()
	var config = _near_connection.user_config
	config.set_value("temp", "public_key", keypair.get("public_key"))
	config.set_value("temp", "private_key", keypair.get("private_key"))
	_near_connection.save_user_data()
	
	var target_url = _near_connection.wallet_url + LOGIN_WALLET_URL_SUFFIX
	target_url += "?contract_id=" + contract_id
	target_url += "&public_key=ed25519:" + keypair.get("public_key")
	
	if OS.has_feature("JavaScript"):
		var window_location = JavaScript.get_interface("location")
		var current_url = window_location.href
		target_url += "&success_url=" + current_url
		target_url += "&failure_url=" + current_url
	else:
		target_url += "&success_url=http://" + CryptoProxy.BIND_ADDRESS + ":" + str(CryptoProxy.port)
		target_url += "&failure_url=http://" + CryptoProxy.BIND_ADDRESS + ":" + str(CryptoProxy.port)
		CryptoProxy.listen_for_login()
	
	print(target_url)
	OS.shell_open(target_url)

func sign_out() -> void:
	if _near_connection.user_config.has_section("user"):
		_near_connection.user_config.erase_section("user")
		account_id = ""
		_near_connection.save_user_data()

func is_signed_in() -> bool:
	_check_signed_in()
	return account_id != null and !account_id.empty()

func _check_signed_in() -> void:
	if _near_connection.user_config.has_section_key("user", "account_id"):
		account_id = _near_connection.user_config.get_value("user", "account_id")

func _on_user_data_updated() -> void:
	if is_signed_in():
		emit_signal("user_signed_in", self)
	else:
		emit_signal("user_signed_out", self)

func call_change_method(contract_id: String, method_name: String, args: Dictionary, \
		gas: int = Near.DEFAULT_FUNCTION_CALL_GAS, deposit: float = 0) -> Dictionary:
	if not is_signed_in():
		var error_message = "Error calling '" + method_name + "' on '" + contract_id + "': user is not signed in."
		push_error(error_message)
		return Near.create_error_response(error_message)
	
	# If the user's access key is low on allowance, request a new one
	var enough_allowance = _has_enough_allowance()
	if enough_allowance is GDScriptFunctionState:
		enough_allowance = yield(enough_allowance, "completed")
	if enough_allowance is Dictionary:
		# Error occurred when trying to check the allowance
		return enough_allowance
	else:
		if not enough_allowance:
			_sign_in_process(contract_id)
			return {
				"warning": "NotEnoughAllowance"
			}
	
	# Get the access key's nonce
	var public_key = get_public_key()
	var response = yield(Near.view_access_key(account_id, public_key), "completed")
	if response.has("error"):
		var error_message = "Failed to view access key: " + response.error.message
		return response
	else:
		access_key_nonce = response.nonce
		access_key_nonce += 1
	
	var gas_amount: int = clamp(gas, 0, Near.MAX_GAS)
	
	var args_encoded = "e30="
	if not args.empty():
		var args_json_string = JSON.print(args)
		args_encoded = Marshalls.utf8_to_base64(args_json_string)
	var args_bytes = Marshalls.base64_to_raw(args_encoded)
	
	var encoded_transaction
	
	if deposit > 0:
		# Function call access keys cannot send tokens. Redirect to wallet url
		# with an unsigned encoded transaction.
		encoded_transaction = CryptoProxy.create_transaction(
			account_id, contract_id, method_name, args_bytes, \
			get_public_key(), access_key_nonce, gas_amount, deposit)
		
		if encoded_transaction is GDScriptFunctionState:
			encoded_transaction = yield(encoded_transaction, "completed")
		
		if encoded_transaction.empty():
			var error_message = "Error when creating NEAR transaction."
			push_error(error_message)
			return Near.create_error_response(error_message)
		
		var target_url = _near_connection.wallet_url + TX_SIGNING_URL_SUFFIX
		target_url += "?transactions=" + encoded_transaction.http_escape()
		if OS.has_feature("JavaScript"):
			pass # No redirects back to the game in web builds
		else:
			var callback_url = "http://" + CryptoProxy.BIND_ADDRESS + ":" + str(CryptoProxy.port)
			target_url += "&callbackUrl=" + callback_url.http_escape()
			CryptoProxy.listen_for_change_call()
			CryptoProxy.connect("transaction_hash_response", self, "_on_transaction_hash_received")
		
		print(target_url)
		OS.shell_open(target_url)
		
		return { "message": "Transaction sent." }
	else:
		# Create a signed, encoded transaction to send using the JSON RPC endpoint.
		encoded_transaction = CryptoProxy.create_signed_transaction(
			account_id, contract_id, method_name, args_bytes, \
			get_private_key(), get_public_key(), access_key_nonce, gas_amount, deposit)
		
		if encoded_transaction is GDScriptFunctionState:
			encoded_transaction = yield(encoded_transaction, "completed")
		
		if encoded_transaction.empty():
			var error_message = "Error when creating signed NEAR transaction."
			push_error(error_message)
			return Near.create_error_response(error_message)
		
		var data_to_send = {
			"jsonrpc": "2.0",
			"id": "dontcare",
			"method": "broadcast_tx_commit",
			"params": [
				encoded_transaction
			]
		}
		var query = JSON.print(data_to_send)
		var url = _near_connection.node_url
		var headers = ["Content-Type: application/json"]
		var use_ssl = false
		
		var rpc_result = yield(Near.query_rpc(url, headers, use_ssl, HTTPClient.METHOD_POST, query), "completed")
		
		return rpc_result

func _on_transaction_hash_received(tx_hash: String) -> void:
	emit_signal("transaction_hash_received", tx_hash)
