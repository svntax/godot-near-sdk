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

func _on_Button_pressed():
	#var level_data = yield(Near.test_get_levels(), "completed")
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
	wallet_connection.sign_in()
