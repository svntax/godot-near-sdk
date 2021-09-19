extends Control

onready var label = $Label
onready var user_label = $UserLabel
onready var message_input = $MessageInput
onready var change_message_button = $ChangeMessageButton
onready var login_button = $LoginButton
onready var view_access_key_button = $ViewAccessKeyButton
onready var donation_label = $DonationLabel
onready var donation_slider = $DonationSlider

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
	wallet_connection.connect("transaction_hash_received", self, "_on_tx_hash_received")
	if wallet_connection.is_signed_in():
		_on_user_signed_in(wallet_connection)
	view_access_key_button.disabled = !wallet_connection.is_signed_in()

func _on_user_signed_in(wallet: WalletConnection):
	user_label.text = "Signed in as: " + wallet.account_id
	login_button.text = "Sign Out"
	view_access_key_button.disabled = false

func _on_user_signed_out(_wallet: WalletConnection):
	user_label.text = "Not signed in"
	login_button.text = "Sign In"
	view_access_key_button.disabled = true

func _on_tx_hash_received(tx_hash: String) -> void:
	label.set_text("Transaction hash: " + tx_hash)

func _on_Button_pressed():
	var result = Near.call_view_method("dev-1629177227636-26182141504774", "helloWorld")
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		if result.error.has("message"):
			label.set_text(result.error.message)
		else:
			label.set_text(JSON.print(result.error))
	else:
		label.set_text(result.data)

func _on_ClearButton_pressed():
	label.set_text("")

func _on_InvalidMethodButton_pressed():
	var result = Near.call_view_method("dev-1629177227636-26182141504774", "blank")
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		if result.error.has("message"):
			label.set_text(result.error.message)
		else:
			label.set_text(JSON.print(result.error))
	else:
		label.set_text(result.data)

func _on_InvalidAccountButton_pressed():
	var result = Near.call_view_method("blank.dev-1629177227636-26182141504774", "blank")
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		label.set_text(JSON.print(result.error))
	else:
		label.set_text(result.data)

func _on_LoginButton_pressed():
	if wallet_connection == null:
		wallet_connection = WalletConnection.new(Near.near_connection)
	if wallet_connection.is_signed_in():
		wallet_connection.sign_out()
	else:
		# Test contract has helloWorld(), read(key: string), write(key: string, value: string)
		wallet_connection.sign_in("dev-1629177227636-26182141504774")

func _on_ReadMessageButton_pressed():
	var result = Near.call_view_method("dev-1629177227636-26182141504774", \
		"read", {"key": "message"})
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		if result.error.has("message"):
			label.set_text(result.error.message)
		else:
			label.set_text(JSON.print(result.error))
	else:
		label.set_text(result.data)

func _on_ChangeMessageButton_pressed():
	var input_text = message_input.text
	message_input.clear()
	message_input.editable = false
	change_message_button.disabled = true
	
	var attached_deposit = donation_slider.value
	
	var result = wallet_connection.call_change_method("dev-1629177227636-26182141504774", \
		"write", {"key": "message", "value": input_text}, \
		Near.DEFAULT_FUNCTION_CALL_GAS, attached_deposit)
	
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	
	if result.has("error"):
		label.set_text(JSON.print(result.error))
	elif result.has("message"):
		label.set_text(result.message)
	elif result.has("warning"):
		label.set_text(result.warning)
	else:
		label.set_text(JSON.print(result.status) + JSON.print(result.transaction))
	
	message_input.editable = true
	change_message_button.disabled = false

func _on_BlockButton_pressed():
	var result = Near.block_query_latest()
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		label.set_text("Failed to get latest block.")
	else:
		label.set_text(result.header.hash)

func _on_ViewAccessKeyButton_pressed():
	var account_id = wallet_connection.account_id
	var public_key = wallet_connection.get_public_key()
	var result = Near.view_access_key(account_id, public_key)
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		if result.error.has("message"):
			label.set_text(result.error.message)
		else:
			label.set_text(JSON.print(result.error))
	else:
		label.set_text(JSON.print(result))

func _on_DonationSlider_value_changed(value):
	donation_label.set_text("Donation\n(NEAR): " + str(value))
