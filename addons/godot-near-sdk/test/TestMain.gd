extends Control

onready var result_label = $"%ResultLabel"
onready var user_label = $UserLabel
onready var message_input = $MessageInput
onready var change_message_button = $ChangeMessageButton
onready var login_button = $LoginButton
onready var view_access_key_button = $ViewAccessKeyButton
onready var sign_message_button = $SignMessageButton
onready var donation_label = $DonationLabel
onready var donation_slider = $DonationSlider
onready var intear_selector = $"%IntearSelector"

var config = {
	"network_id": "testnet",
	"node_url": "https://rpc.testnet.fastnear.com",
	"wallet_provider": WalletProviders.Wallet.INTEAR, # TODO: hard-coded to Intear until a proper wallet selector is added
}
const CONTRACT_ID: String = "dev-1629177227636-26182141504774"
const CONTRACT_METHODS: Array = ["write"]

var wallet_connection

func _ready():
	Near.start_connection(config)
	wallet_connection = WalletConnection.new(Near.near_connection)
	wallet_connection.connect("user_signed_in", self, "_on_user_signed_in")
	wallet_connection.connect("user_signed_out", self, "_on_user_signed_out")
	wallet_connection.connect("transaction_hash_received", self, "_on_tx_hash_received")
	wallet_connection.connect("connected_response", self, "_on_connected_response")
	wallet_connection.connect("signed_message_response", self, "_on_signed_message_response")
	wallet_connection.connect("sent_transactions_response", self, "_on_sent_transactions_response")
	wallet_connection.connect("error_response", self, "_on_error_response")
	wallet_connection.connect("websocket_closed", self, "_on_wallet_websocket_closed")
	intear_selector.connect("selector_closed", self, "_on_intear_selector_closed")
	if wallet_connection.is_signed_in():
		wallet_connection.app_contract_id = CONTRACT_ID
		wallet_connection.app_contract_method_names = CONTRACT_METHODS.duplicate()
		_on_user_signed_in(wallet_connection)
	view_access_key_button.disabled = !wallet_connection.is_signed_in()
	change_message_button.disabled = !wallet_connection.is_signed_in()
	sign_message_button.disabled = !wallet_connection.is_signed_in()

# Enable/disable the main buttons that need to await for a response
func set_enabled_main_buttons(value: bool) -> void:
	view_access_key_button.disabled = !value
	change_message_button.disabled = !value
	message_input.editable = value
	sign_message_button.disabled = !value

func _on_user_signed_in(wallet: WalletConnection):
	user_label.text = "Signed in as: " + wallet.account_id
	login_button.text = "Sign Out"
	set_enabled_main_buttons(true)
	login_button.disabled = false
	intear_selector.close()
	# Note: when using Intear Wallet, it's up to the developer to handle if functionCallKeyAdded was false

func _on_user_signed_out(_wallet: WalletConnection):
	user_label.text = "Not signed in"
	login_button.text = "Sign In"
	set_enabled_main_buttons(false)

func _on_wallet_websocket_closed() -> void:
	# Note: This doesn't run if the user closed the desktop wallet (did NOT click on Connect nor Cancel)
	login_button.disabled = false
	intear_selector.close()
	set_enabled_main_buttons(true)

func _on_intear_selector_closed() -> void:
	login_button.disabled = false

func _on_tx_hash_received(tx_hash: String) -> void:
	result_label.set_text("Transaction hash: " + tx_hash)

func _on_connected_response(response: Dictionary) -> void:
	result_label.set_text(JSON.print(response, "  "))
	set_enabled_main_buttons(true)

func _on_signed_message_response(response: Dictionary) -> void:
	result_label.set_text(JSON.print(response, "  "))
	set_enabled_main_buttons(true)

func _on_sent_transactions_response(response: Dictionary) -> void:
	result_label.set_text(JSON.print(response, "  "))
	set_enabled_main_buttons(true)

func _on_error_response(response: Dictionary) -> void:
	result_label.set_text(JSON.print(response, "  "))
	set_enabled_main_buttons(true)

func _on_Button_pressed():
	var result = Near.call_view_method(CONTRACT_ID, "helloWorld")
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		if result.error.has("message"):
			result_label.set_text(result.error.message)
		else:
			result_label.set_text(JSON.print(result.error, "  "))
	else:
		result_label.set_text(result.data)

func _on_SignMessageButton_pressed():
	set_enabled_main_buttons(false)
	
	var message = "Hello, this is a test message."
	var recipient = "Test app"
	var result = wallet_connection.sign_message(message, recipient)
	
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	
	var success = false
	if result.has("error"):
		result_label.set_text(JSON.print(result.error, "  "))
	else:
		success = true
		result_label.set_text(JSON.print(result, "  "))
	
	if success:
		set_enabled_main_buttons(true)

func _on_ClearButton_pressed():
	result_label.set_text("")

func _on_LoginButton_pressed():
	if wallet_connection == null:
		wallet_connection = WalletConnection.new(Near.near_connection)
	if wallet_connection.is_signed_in():
		wallet_connection.sign_out()
	else:
		login_button.disabled = true
		# Test contract has helloWorld(), read(key: string), write(key: string, value: string)
		intear_selector.open(wallet_connection, CONTRACT_ID, CONTRACT_METHODS)

func _on_ReadMessageButton_pressed():
	var result = Near.call_view_method(CONTRACT_ID, "read", {"key": "message"})
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		if result.error.has("message"):
			result_label.set_text(result.error.message)
		else:
			result_label.set_text(JSON.print(result.error, "  "))
	else:
		result_label.set_text(result.data)

func _on_ChangeMessageButton_pressed():
	var input_text = message_input.text
	if input_text.empty():
		result_label.set_text("Message should not be empty.")
		return
	
	set_enabled_main_buttons(false)
	
	var attached_deposit = donation_slider.value
	var write_transaction = Near.createTransaction(wallet_connection.account_id, CONTRACT_ID,
		[
			Near.functionCallAction(
				"write",
				{"key": "message", "value": input_text},
				str(Near.DEFAULT_FUNCTION_CALL_GAS),
				Near.from_near(str(attached_deposit))
			)
		]
	)
	var transactions = [write_transaction]
	var result = wallet_connection.send_transactions(transactions)
	
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	
	var success = false
	if result.has("error"):
		result_label.set_text(JSON.print(result.error, "  "))
	elif result.has("message"):
		success = true
		result_label.set_text(result.message)
	else:
		success = true
		result_label.set_text(JSON.print(result.status) + JSON.print(result.transaction))
	
	if success:
		set_enabled_main_buttons(true)

func _on_BlockButton_pressed():
	var result = Near.block_query_latest()
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		result_label.set_text("Failed to get latest block.")
	else:
		result_label.set_text(result.header.hash)

func _on_ViewAccessKeyButton_pressed():
	set_enabled_main_buttons(false)
	
	var account_id = wallet_connection.account_id
	var public_key = wallet_connection.get_app_public_key()
	var result = Near.view_access_key(account_id, public_key)
	if result is GDScriptFunctionState:
		result = yield(result, "completed")
	if result.has("error"):
		if result.error.has("message"):
			result_label.set_text(result.error.message)
		else:
			result_label.set_text(JSON.print(result.error, "  "))
	else:
		result_label.set_text(JSON.print(result, "  "))
	
	set_enabled_main_buttons(true)

func _on_DonationSlider_value_changed(value):
	donation_label.set_text("Deposit\n(NEAR): " + str(value))
