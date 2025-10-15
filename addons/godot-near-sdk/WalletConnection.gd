extends Reference
class_name WalletConnection

signal user_signed_in()
signal user_signed_out()
signal transaction_hash_received(tx_hash)
signal signed_message_response(response)
signal websocket_closed()

var _near_connection: NearConnection
var _websocket_client: WebSocketClient

# Intear-specific data
var intear_wallet_type: int
var _intear_ws_session_id: String = ""
var _intear_request_type: String
var _intear_sign_message_request: Dictionary = {}
var function_call_key_added: bool = false setget ,is_function_call_key_added

var account_id: String setget ,get_account_id
var access_key_nonce: int
var app_contract_id: String
var app_contract_method_names: Array
const JS_GODOT_BRIDGE = "window.godotBridge"

var _js_message_callback_ref = JavaScript.create_callback(self, "_on_js_message_event")

func _init(near_connection: NearConnection):
	_near_connection = near_connection
	_check_signed_in()
	_near_connection.connect("user_data_updated", self, "_on_user_data_updated")

func get_account_id() -> String:
	return account_id

func get_app_private_key() -> String:
	return _near_connection.user_config.get_value("user", "app_private_key", "")

func get_app_public_key() -> String:
	return _near_connection.user_config.get_value("user", "app_public_key", "")

func get_user_public_key() -> String:
	return _near_connection.user_config.get_value("user", "user_public_key", "")

func is_function_call_key_added() -> bool:
	return function_call_key_added

func set_intear_wallet_type(wallet_type: int) -> void:
	intear_wallet_type = wallet_type
	if wallet_type == IntearSelector.WalletType.WEB:
		_near_connection.wallet_url = "https://wallet.intear.tech"
	elif wallet_type == IntearSelector.WalletType.WEB_BETA:
		_near_connection.wallet_url = "https://staging.wallet.intear.tech"

func _has_enough_allowance():
	var public_key = get_app_public_key()
	var result = Near.view_access_key(account_id, public_key)
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		return result.error
	else:
		var allowance: String = result.permission.FunctionCall.allowance
		return CryptoProxy.is_enough_allowance(allowance)

func get_login_url_endpoint(wallet: int) -> String:
	return "/login"

func sign_in(contract_id: String, method_names: Array = []) -> void:
	if is_signed_in():
		emit_signal("user_signed_in", self)
		return
	_sign_in_process(contract_id, method_names)

func _sign_in_process(contract_id: String, method_names: Array) -> void:
	var keypair = CryptoProxy.create_keypair()
	var config = _near_connection.user_config
	config.set_value("temp", "public_key", keypair.get("public_key"))
	config.set_value("temp", "private_key", keypair.get("private_key"))
	_near_connection.save_user_data()
	
	match _near_connection.wallet_provider:
		WalletProviders.Wallet.MY_NEAR_WALLET:
			_sign_in_with_my_near_wallet(keypair, contract_id)
		WalletProviders.Wallet.INTEAR:
			_sign_in_with_intear(keypair, contract_id, method_names)

func _sign_in_with_my_near_wallet(keypair: Dictionary, contract_id: String) -> void:
	var target_url = _near_connection.wallet_url + WalletProviders.get_login_url_endpoint(_near_connection.wallet_provider)
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

func _sign_in_with_intear(keypair: Dictionary, contract_id: String = "", method_names: Array = []) -> void:
	app_contract_id = contract_id
	app_contract_method_names = method_names
	_intear_request_type = "connect"
	if OS.has_feature("JavaScript"):
		_send_intear_login_request()
	else:
		_start_intear_websocket_connection()

# Connect to Intear's logout bridge service to start a session for a new request
func _start_intear_websocket_connection():
	if _websocket_client != null:
		if _websocket_client.is_connected("connection_closed", self, "_websocket_closed"):
			_websocket_client.disconnect("connection_closed", self, "_websocket_closed")
		
		if _websocket_client.is_connected("connection_error", self, "_websocket_closed"):
			_websocket_client.disconnect("connection_error", self, "_websocket_closed")
		
		if _websocket_client.is_connected("connection_established", self, "_websocket_connected"):
			_websocket_client.disconnect("connection_established", self, "_websocket_connected")
		
		if _websocket_client.is_connected("data_received", self, "_websocket_on_data"):
			_websocket_client.disconnect("data_received", self, "_websocket_on_data")
	
	_websocket_client = WebSocketClient.new()
	_websocket_client.connect("connection_closed", self, "_websocket_closed")
	_websocket_client.connect("connection_error", self, "_websocket_closed")
	_websocket_client.connect("connection_established", self, "_websocket_connected")
	_websocket_client.connect("data_received", self, "_websocket_on_data")
	
	var ws_url = WalletProviders.INTEAR_LOGOUT_BRIDGE_SERVICE_URL + "/api/session/create"
	var err = _websocket_client.connect_to_url(ws_url, [])
	if err == OK:
		Near.websocket_client = _websocket_client
		Near.set_process(true)
	else:
		push_error("Unable to connect")
		_intear_request_type = ""
		Near.set_process(false)
		Near.websocket_client = null

func _websocket_closed(was_clean = false):
	print("Closed, clean: ", was_clean)
	Near.set_process(false)
	_intear_ws_session_id = ""
	emit_signal("websocket_closed")

func _websocket_connected(proto = ""):
	print("Connected with protocol: ", proto)

func _websocket_on_data():
	var data: String = _websocket_client.get_peer(1).get_packet().get_string_from_utf8()
	print("Got data from server: ", data)
	var response: Dictionary = JSON.parse(data).result
	if response.has("session_id"):
		# Response from Intear's session create endpoint
		_intear_ws_session_id = response.get("session_id")
		match _intear_request_type:
			"connect":
				_send_intear_login_request()
			"sign-message":
				_send_intear_sign_message_request()
	else:
		var response_type = response.get("type")
		match response_type:
			"connected":
				print("Connected successfully!")
				_handle_intear_sign_in_response(response)
			"signed":
				print("Signed message successfully!")
				_handle_intear_sign_message_response(response)
			"error":
				if response.has("message"):
					print(response.get("message"))
				else:
					push_error("Unknown error from wallet popup")

func _generate_intear_sign_in_request(params: Dictionary = {}) -> Dictionary:
	var config = _near_connection.user_config
	var public_key = config.get_value("temp", "public_key")
	var nonce = int(Time.get_unix_time_from_system() * 1000)
	var origin = ""
	if OS.has_feature("JavaScript"):
		origin = JavaScript.eval("window.location.origin")
	elif intear_wallet_type == IntearSelector.WalletType.DESKTOP:
		# TODO: what should the origin be if this is a desktop app?
		origin = "http://tauri.localhost"
	var message_text = "{\"origin\":\"%s\"}" % origin
	var signature = CryptoProxy.create_intear_connect_signature(nonce, message_text)
	var sign_in_request = {
	  "type": "signIn",
	  "data": {
		"publicKey": "ed25519:" + public_key,
		"contractId": params.get("contractId", ""),
		"methodNames": params.get("methodNames", []),
		"networkId": _near_connection.network_id,
		"nonce": nonce,
		"message": message_text,
		"signature": signature,
		"version": "V2",
		"actualOrigin": origin
	  }
	}
	return sign_in_request

func _generate_intear_sign_message_request(params: Dictionary = {}) -> Dictionary:
	var auth_nonce = int(Time.get_unix_time_from_system() * 1000)
	var message = params.get("message")
	var message_payload: Dictionary = CryptoProxy.create_nep413_payload(
		message, params.get("recipient")
	)
	var message_payload_json = JSON.print(message_payload)
	var signature = CryptoProxy.create_intear_sign_message_signature(auth_nonce, message_payload_json) 
	var sign_message_request = {
	  "type": "signMessage",
	  "data": {
		"message": message_payload_json,
		"accountId": account_id,
		"publicKey": "ed25519:" + get_app_public_key(),
		"nonce": auth_nonce,
		"signature": signature
	  }
	}
	return sign_message_request

func _send_intear_login_request() -> void:
	if OS.has_feature("JavaScript"):
		if intear_wallet_type in [IntearSelector.WalletType.WEB, IntearSelector.WalletType.WEB_BETA]:
			var js_window := JavaScript.get_interface("window")
			# Open popup
			var POPUP_FEATURES = "opener,width=400,height=700"
			var wallet_url = _near_connection.wallet_url + "/connect"
			JavaScript.eval("%s = window.open('%s', '_blank', '%s')" % [JS_GODOT_BRIDGE, wallet_url, POPUP_FEATURES])
			js_window.addEventListener("message", _js_message_callback_ref)
		elif intear_wallet_type == IntearSelector.WalletType.DESKTOP:
			if _intear_ws_session_id.empty():
				# Selected desktop wallet from web app, so we need to start a new websockets connection
				_start_intear_websocket_connection()
			else:
				_send_intear_ws_sign_in_request()
				OS.shell_open("intear://connect?session_id=" + _intear_ws_session_id)
	else:
		if intear_wallet_type == IntearSelector.WalletType.DESKTOP:
			_send_intear_ws_sign_in_request()
			OS.shell_open("intear://connect?session_id=" + _intear_ws_session_id)
		else:
			push_error("Only the Intear Desktop Wallet is supported in desktop apps at this time.")

func _send_intear_sign_message_request() -> void:
	if OS.has_feature("JavaScript"):
		if intear_wallet_type in [IntearSelector.WalletType.WEB, IntearSelector.WalletType.WEB_BETA]:
			var js_window := JavaScript.get_interface("window")
			# Open popup
			var POPUP_FEATURES = "opener,width=400,height=700"
			var wallet_url = _near_connection.wallet_url + "/sign-message"
			JavaScript.eval("%s = window.open('%s', '_blank', '%s')" % [JS_GODOT_BRIDGE, wallet_url, POPUP_FEATURES])
			js_window.addEventListener("message", _js_message_callback_ref)
		elif intear_wallet_type == IntearSelector.WalletType.DESKTOP:
			if _intear_ws_session_id.empty():
				# Selected desktop wallet from web app, so we need to start a new websockets connection
				_start_intear_websocket_connection()
			else:
				_send_intear_ws_sign_message_request()
				OS.shell_open("intear://sign-message?session_id=" + _intear_ws_session_id)
	else:
		if intear_wallet_type == IntearSelector.WalletType.DESKTOP:
			_send_intear_ws_sign_message_request()
			OS.shell_open("intear://sign-message?session_id=" + _intear_ws_session_id)
		else:
			push_error("Only the Intear Desktop Wallet is supported in desktop apps at this time.")

func _send_intear_ws_sign_in_request() -> void:
	var sign_in_request: Dictionary = _generate_intear_sign_in_request({
		"contractId": app_contract_id,
		"methodNames": app_contract_method_names
	})
	var sign_in_request_json = JSON.print(sign_in_request)
	var packet = sign_in_request_json.to_utf8()
	_websocket_client.get_peer(1).set_write_mode(WebSocketPeer.WRITE_MODE_TEXT)
	_websocket_client.get_peer(1).put_packet(packet)

func _handle_intear_sign_in_response(response: Dictionary) -> void:
	_intear_request_type = ""
	if intear_wallet_type != IntearSelector.WalletType.DESKTOP:
		_near_connection.wallet_url = response.walletUrl # Currently connected wallet origin. Always use this for subsequent requests, since the user might be running on staging.wallet.intear.tech or a self-hosted instance of the wallet, which should be respected
	var account = response.accounts[0]
	CryptoProxy.save_account_data(account.accountId, account.publicKey)
	CryptoProxy.save_intear_wallet_type(intear_wallet_type)
	function_call_key_added = response.functionCallKeyAdded

func _send_intear_ws_sign_message_request() -> void:
	var sign_message_request_json = JSON.print(_intear_sign_message_request)
	var packet = sign_message_request_json.to_utf8()
	_websocket_client.get_peer(1).set_write_mode(WebSocketPeer.WRITE_MODE_TEXT)
	_websocket_client.get_peer(1).put_packet(packet)

func _handle_intear_sign_message_response(response: Dictionary) -> void:
	emit_signal("signed_message_response", response)

func _on_js_message_event(args):
	print("Message listener received data:")
	var js_event = args[0]
	var js_json = JavaScript.get_interface("JSON")
	var json_string = js_json.stringify(js_event.data)
	print(json_string)
	var parsed_data = JSON.parse(json_string)
	if parsed_data.error == OK:
		var response = parsed_data.result
		match response.type:
			"ready":
				# Handshake complete, send request using postMessage()
				if _intear_request_type == "connect":
					_intear_web_post_message_sign_in()
				elif _intear_request_type == "sign-message":
					_intear_web_post_message_sign_message()
				else:
					push_error("Invalid request type: " + _intear_request_type)
			"connected":
				print("Connected successfully! Closing wallet popup and removing event listener...")
				JavaScript.eval("%s.close()" % [JS_GODOT_BRIDGE])
				var js_window := JavaScript.get_interface("window")
				js_window.removeEventListener("message", _js_message_callback_ref)
				_handle_intear_sign_in_response(response)
			"signed":
				print("Signed message! Closing wallet popup and removing event listeners...")
				JavaScript.eval("%s.close()" % [JS_GODOT_BRIDGE])
				var js_window := JavaScript.get_interface("window")
				js_window.removeEventListener("message", _js_message_callback_ref)
				_handle_intear_sign_message_response(response)
			"error":
				push_error("Unknown error from connect popup")
				JavaScript.eval("%s.close()" % [JS_GODOT_BRIDGE])
				var js_window := JavaScript.get_interface("window")
				js_window.removeEventListener("message", _js_message_callback_ref)
	else:
		push_error("Error parsing response data as JSON")

func _intear_web_post_message_sign_in() -> void:
	print("Wallet popup ready. Sending signIn request:")
	var sign_in_request: Dictionary = _generate_intear_sign_in_request({
		"contractId": app_contract_id,
		"methodNames": app_contract_method_names
	})
	var sign_in_request_json = JSON.print(sign_in_request)
	print(sign_in_request_json)
	var data = sign_in_request.get("data")
	var method_names: Array = data.get("methodNames")
	var sign_in_request_js = """{
		"type": "signIn",
		"data": {
			"publicKey": "%s",
			"contractId": "%s",
			"methodNames": %s,
			"networkId": "%s",
			"nonce": %s,
			"message": '%s',
			"signature": "%s",
			"version": "%s",
			"actualOrigin": "%s"
		}
	}""" % [
		data.get("publicKey"),
		data.get("contractId"),
		JSON.print(method_names),
		data.get("networkId"),
		data.get("nonce"),
		data.get("message"),
		data.get("signature"),
		data.get("version"),
		data.get("actualOrigin")
	]
	var target_origin = _near_connection.wallet_url
	var eval_string = "%s.postMessage(%s, '%s')" % [JS_GODOT_BRIDGE, sign_in_request_js, target_origin]
	JavaScript.eval(eval_string)

func _intear_web_post_message_sign_message() -> void:
	print("Wallet popup ready. Sending signMessage request:")
	var data = _intear_sign_message_request.get("data")
	var sign_message_request_js = """{
		"type": "signMessage",
		"data": {
			"message": '%s',
			"accountId": "%s",
			"publicKey": "%s",
			"nonce": %s,
			"signature": "%s"
		}
	}""" % [
		data.get("message"),
		data.get("accountId"),
		data.get("publicKey"),
		data.get("nonce"),
		data.get("signature")
	]
	var target_origin = _near_connection.wallet_url
	var eval_string = "%s.postMessage(%s, '%s')" % [JS_GODOT_BRIDGE, sign_message_request_js, target_origin]
	JavaScript.eval(eval_string)

func sign_out() -> void:
	account_id = ""
	app_contract_id = ""
	app_contract_method_names = []
	function_call_key_added = false
	if OS.has_feature("JavaScript"):
		JavaScript.eval("%s = null;" % JS_GODOT_BRIDGE)
	
	if _near_connection.user_config.has_section("user"):
		_near_connection.user_config.erase_section("user")
		_near_connection.save_user_data()

func is_signed_in() -> bool:
	_check_signed_in()
	return account_id != null and !account_id.empty()

func _check_signed_in() -> void:
	if _near_connection.user_config.has_section_key("user", "account_id"):
		account_id = _near_connection.user_config.get_value("user", "account_id")
	if _near_connection.user_config.has_section_key("user", "intear_wallet_type"):
		intear_wallet_type = _near_connection.user_config.get_value("user", "intear_wallet_type")

func _on_user_data_updated() -> void:
	if is_signed_in():
		emit_signal("user_signed_in", self)
	else:
		emit_signal("user_signed_out", self)

func sign_message(message: String, recipient: String) -> Dictionary:
	if not is_signed_in():
		var error_message = "User is not signed in."
		push_error(error_message)
		return Near.create_error_response(error_message)
	
	if _near_connection.wallet_provider == WalletProviders.Wallet.INTEAR:
		_intear_request_type = "sign-message"
		_intear_sign_message_request = _generate_intear_sign_message_request({
			"message": message, "recipient": recipient
		})
		if OS.has_feature("JavaScript"):
			_send_intear_sign_message_request()
		else:
			_start_intear_websocket_connection()
		
		return {"message": "Requesting user to sign message: %s" % message}
	else:
		var error_message = "Signing messages is currently supported in Intear Wallet only."
		push_error(error_message)
		return Near.create_error_response(error_message)

func call_change_method(contract_id: String, method_name: String, args: Dictionary, \
		gas: int = Near.DEFAULT_FUNCTION_CALL_GAS, deposit: float = 0) -> Dictionary:
	if not is_signed_in():
		var error_message = "Error calling '" + method_name + "' on '" + contract_id + "': user is not signed in."
		push_error(error_message)
		return Near.create_error_response(error_message)
	
	# Prompt for function call key if missing
	if _near_connection.wallet_provider == WalletProviders.Wallet.INTEAR:
		if not function_call_key_added:
			_sign_in_process(contract_id, app_contract_method_names)
			return {
				"warning": "No function call key found. Prompting user for new key..."
			}
	
	# If the user's access key is low on allowance, request a new one
	var enough_allowance = _has_enough_allowance()
	if enough_allowance is GDScriptFunctionState:
		enough_allowance = yield(enough_allowance, "completed")
	if enough_allowance is Dictionary:
		# Error occurred when trying to check the allowance
		return enough_allowance
	else:
		if not enough_allowance:
			_sign_in_process(contract_id, app_contract_method_names)
			return {
				"warning": "NotEnoughAllowance"
			}
	
	# Get the access key's nonce
	var public_key = get_app_public_key()
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
		# TODO: handle calls with deposit in Intear wallet
		# Function call access keys cannot send tokens. Redirect to wallet url
		# with an unsigned encoded transaction.
		encoded_transaction = CryptoProxy.create_transaction(
			account_id, contract_id, method_name, args_bytes, \
			get_app_public_key(), access_key_nonce, gas_amount, deposit)
		
		if encoded_transaction is GDScriptFunctionState:
			encoded_transaction = yield(encoded_transaction, "completed")
		
		if encoded_transaction.empty():
			var error_message = "Error when creating NEAR transaction."
			push_error(error_message)
			return Near.create_error_response(error_message)
		
		var target_url = _near_connection.wallet_url + WalletProviders.get_sign_transaction_url_endpoint(_near_connection.wallet_provider)
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
			get_app_private_key(), get_app_public_key(), access_key_nonce, gas_amount, deposit)
		
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
