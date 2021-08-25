extends Reference
class_name WalletConnection

const LOGIN_WALLET_URL_SUFFIX = "/login/";

signal user_signed_in()
signal user_signed_out()

var _near_connection: NearConnection

var account_id: String setget ,get_account_id

func _init(near_connection: NearConnection):
	_near_connection = near_connection
	_check_signed_in()
	_near_connection.connect("user_data_updated", self, "_on_user_data_updated")

func get_account_id() -> String:
	return account_id

func sign_in(contract_id: String) -> void:
	if is_signed_in():
		emit_signal("user_signed_in", self)
		return
	
	var keypair = CryptoProxy.create_keypair()
	var config = _near_connection.user_config
	config.set_value("temp", "public_key", keypair.get("public_key"))
	config.set_value("temp", "private_key", keypair.get("private_key"))
	_near_connection.save_user_data()
	
	var target_url = _near_connection.wallet_url + LOGIN_WALLET_URL_SUFFIX
	target_url += "?contract_id=" + contract_id
	target_url += "&public_key=ed25519:" + keypair.get("public_key")
	
	if OS.has_feature("JavaScript"):
		# TODO: url parameters on web builds cause a 404 error, but in Godot 3.4
		# this will be fixed.
		# See https://github.com/godotengine/godot/pull/49889
		# Furthermore, 3.4 will have a new JavaScriptObject interface that should
		# be used instead of eval()
		# See https://github.com/godotengine/godot/pull/48691
#		var current_url = JavaScript.eval(""" 
#            var url_string = window.location.href;
#            url_string;
#        """)
#		target_url += "&success_url=" + current_url
#		target_url += "&failure_url=" + current_url
		# TODO: add temporary handling by manually asking for the account id
		pass
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

func call_change_method(contract_id: String, method_name: String, args: Dictionary) -> void:
	if not is_signed_in():
		push_error("Error calling '" + method_name + "' on '" + contract_id + "': user is not signed in.")
		return
	
	# TODO: stub, attached gas and deposit parameters
	var args_encoded = "e30="
	if not args.empty():
		var args_json_string = JSON.print(args)
		args_encoded = Marshalls.utf8_to_base64(args_json_string)
	
	print("Change method \"" + method_name + "\" on " + contract_id + " with args: " + str(args))
