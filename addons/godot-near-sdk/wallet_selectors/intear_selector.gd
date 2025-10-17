extends Control
class_name IntearSelector

signal selector_closed()

enum WalletType {
	WEB,
	WEB_BETA,
	DESKTOP,
	SELF_HOSTED
}

export var close_button_path: NodePath

onready var web = $"%Web"
onready var web_beta = $"%WebBeta"
onready var desktop = $"%Desktop"
onready var self_hosted = $"%SelfHosted"
onready var retry = $"%Retry"

onready var wallet_types_container = $"%WalletTypesContainer"
onready var desktop_selected_content = $"%DesktopSelectedContent"

var wallet_connection
var contract_id: String
var contract_methods: Array

func _ready():
	desktop_selected_content.hide()
	wallet_types_container.show()
	var close_button = get_node(close_button_path)
	close_button.connect("pressed", self, "_on_close_pressed")
	web.connect("pressed", self, "_on_web_selected")
	web_beta.connect("pressed", self, "_on_web_beta_selected")
	desktop.connect("pressed", self, "_on_desktop_selected")
	self_hosted.connect("pressed", self, "_on_selfhosted_selected")
	retry.connect("pressed", self, "_on_retry_pressed")

func open(wallet_connection, contract_id: String = "", contract_methods: Array = []) -> void:
	self.wallet_connection = wallet_connection
	self.contract_id = contract_id
	self.contract_methods = contract_methods.duplicate()
	show()

func close() -> void:
	wallet_connection = null
	contract_id = ""
	contract_methods = []
	hide()
	desktop_selected_content.hide()
	wallet_types_container.show()
	emit_signal("selector_closed")

func _on_close_pressed() -> void:
	close()

func _on_retry_pressed() -> void:
	desktop_selected_content.hide()
	wallet_types_container.show()

func _on_web_selected() -> void:
	if OS.has_feature("JavaScript"):
		wallet_connection.set_intear_wallet_type(WalletType.WEB)
		wallet_connection.sign_in(contract_id, contract_methods.duplicate())
	else:
		push_error("Intear Web Wallet is currently not supported in desktop apps.")

func _on_web_beta_selected() -> void:
	if OS.has_feature("JavaScript"):
		wallet_connection.set_intear_wallet_type(WalletType.WEB_BETA)
		wallet_connection.sign_in(contract_id, contract_methods.duplicate())
	else:
		push_error("Intear Web Wallet (Beta) is currently not supported in desktop apps.")

func _on_desktop_selected() -> void:
	wallet_types_container.hide()
	desktop_selected_content.show()
	wallet_connection.set_intear_wallet_type(WalletType.DESKTOP)
	wallet_connection.sign_in(contract_id, contract_methods.duplicate())

func _on_selfhosted_selected() -> void:
	wallet_connection.set_intear_wallet_type(WalletType.SELF_HOSTED)
	push_warning("Self-hosted Intear Wallet is currently not implemented.")
