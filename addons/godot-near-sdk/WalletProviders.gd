extends Node
class_name WalletProviders

enum Wallet {
	MY_NEAR_WALLET,
	INTEAR
}

const INTEAR_LOGOUT_BRIDGE_SERVICE_URL = "wss://logout-bridge-service.intear.tech"

static func get_login_url_endpoint(wallet_provider: int) -> String:
	if wallet_provider == Wallet.INTEAR:
		return "/connect"
	elif wallet_provider == Wallet.MY_NEAR_WALLET:
		return "/login/"
	
	return ""

static func get_sign_transaction_url_endpoint(wallet_provider: int) -> String:
	if wallet_provider == Wallet.INTEAR:
		return "/send-transactions"
	elif wallet_provider == Wallet.MY_NEAR_WALLET:
		return "/sign"
	
	return ""
