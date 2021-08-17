extends Control

onready var label = $Label
onready var user_label = $UserLabel

var config = {
	"network_id": "testnet",
	"node_url": "https://rpc.testnet.near.org",
	"wallet_url": "https://wallet.testnet.near.org",
}

var wallet_connection

func _ready():
	Near.start_connection(config)
	wallet_connection = WalletConnection.new(Near.near_connection)
	if wallet_connection.is_signed_in():
		user_label.text = "Signed in as: " + wallet_connection.get_account_id()
		# TODO: logout callback
		$LoginButton.disabled = true

func _on_Button_pressed():
	var result = yield(Near.call_view_method("levels-browsing.svntax.testnet", "getLevels"), "completed")
	label.set_text(result)

func _on_ClearButton_pressed():
	label.set_text("")

func _on_InvalidMethodButton_pressed():
	var result = yield(Near.call_view_method("levels-browsing.svntax.testnet", "blank"), "completed")
	label.set_text(result)

func _on_InvalidAccountButton_pressed():
	var result = yield(Near.call_view_method("blank.svntax.testnet", "blank"), "completed")
	label.set_text(result)

func _on_LoginButton_pressed():
	if wallet_connection == null:
		wallet_connection = WalletConnection.new(Near.near_connection)
	# TODO: need a smart contract for testing purposes
	wallet_connection.sign_in("some-contract.testnet")
