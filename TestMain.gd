extends Control

onready var label = $Label
onready var user_label = $UserLabel
onready var message_input = $MessageInput
onready var login_button = $LoginButton

var config = {
	"network_id": "testnet",
	"node_url": "https://rpc.testnet.near.org",
	"wallet_url": "https://wallet.testnet.near.org",
}

var wallet_connection

func _ready():
	Near.start_connection(config)
	wallet_connection = WalletConnection.new(Near.near_connection)
	wallet_connection.connect("user_signed_in", self, "_on_user_signed_in")
	wallet_connection.connect("user_signed_out", self, "_on_user_signed_out")
	if wallet_connection.is_signed_in():
		_on_user_signed_in(wallet_connection)

func _on_user_signed_in(wallet: WalletConnection):
	user_label.text = "Signed in as: " + wallet.account_id
	login_button.text = "Sign Out"

func _on_user_signed_out(wallet: WalletConnection):
	user_label.text = "Not signed in"
	login_button.text = "Sign In"

func _on_Button_pressed():
	var result = yield(Near.call_view_method("dev-1629177227636-26182141504774", "helloWorld"), "completed")
	label.set_text(result)

func _on_ClearButton_pressed():
	label.set_text("")

func _on_InvalidMethodButton_pressed():
	var result = yield(Near.call_view_method("dev-1629177227636-26182141504774", "blank"), "completed")
	label.set_text(result)

func _on_InvalidAccountButton_pressed():
	var result = yield(Near.call_view_method("blank.dev-1629177227636-26182141504774", "blank"), "completed")
	label.set_text(result)

func _on_LoginButton_pressed():
	if wallet_connection == null:
		wallet_connection = WalletConnection.new(Near.near_connection)
	if wallet_connection.is_signed_in():
		wallet_connection.sign_out()
	else:
		# Test contract has helloWorld(), read(key: string), write(key: string, value: string)
		wallet_connection.sign_in("dev-1629177227636-26182141504774")

func _on_ReadMessageButton_pressed():
	var result = yield(Near.call_view_method("dev-1629177227636-26182141504774", \
		"read", {"key": "message"}), "completed")
	label.set_text(result)

func _on_ChangeMessageButton_pressed():
	var input_text = message_input.text
	message_input.clear()
	message_input.editable = false
	
	# TODO: yield until complete
	wallet_connection.call_change_method("dev-1629177227636-26182141504774", \
		"write", {"key": "message", "value": input_text})
	
	message_input.editable = true
