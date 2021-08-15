extends Reference
class_name WalletConnection

const LOGIN_WALLET_URL_SUFFIX = "/login/";

var _near_connection: NearConnection

var account_id: String setget ,get_account_id

func _init(near_connection: NearConnection):
	_near_connection = near_connection

func get_account_id() -> String:
	return account_id

func sign_in(contract_id: String) -> void:
	var keypair = CryptoProxy.create_keypair()
	var targetUrl = _near_connection.wallet_url + LOGIN_WALLET_URL_SUFFIX
	targetUrl += "?contract_id=" + contract_id
	targetUrl += "&public_key=ed25519:" + keypair.get("public_key")
	print(targetUrl)
	#OS.shell_open(targetUrl)

func sign_out() -> void:
	# TODO
	pass

func is_signed_in() -> bool:
	# TODO
	return false
