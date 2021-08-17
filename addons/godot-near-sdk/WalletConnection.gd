extends Reference
class_name WalletConnection

const LOGIN_WALLET_URL_SUFFIX = "/login/";

var _near_connection: NearConnection

var account_id: String setget ,get_account_id

func _init(near_connection: NearConnection):
	_near_connection = near_connection
	if _near_connection.user_config.has_section_key("user", "account_id"):
		account_id = _near_connection.user_config.get_value("user", "account_id")

func get_account_id() -> String:
	return account_id

func sign_in(contract_id: String) -> void:
	var keypair = CryptoProxy.create_keypair()
	var config = _near_connection.user_config
	config.set_value("temp", "public_key", keypair.get("public_key"))
	config.set_value("temp", "private_key", keypair.get("private_key"))
	_near_connection.save_user_data()
	
	var targetUrl = _near_connection.wallet_url + LOGIN_WALLET_URL_SUFFIX
	targetUrl += "?contract_id=" + contract_id
	targetUrl += "&public_key=ed25519:" + keypair.get("public_key")
	print(targetUrl)
	#OS.shell_open(targetUrl)
	
	# TODO: if web build, check url directly using JavaScript singleton
	CryptoProxy.listen_for_login()

func sign_out() -> void:
	# TODO
	pass

func is_signed_in() -> bool:
	var config = _near_connection.user_config
	return config.has_section_key("user", "account_id")
