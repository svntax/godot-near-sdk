extends Reference
class_name WalletConnection

var _near_connection: NearConnection

var account_id: String setget ,get_account_id

func _init(near_connection: NearConnection):
	_near_connection = near_connection

func get_account_id() -> String:
	return account_id

func sign_in() -> void:
	# TODO
	pass

func sign_out() -> void:
	# TODO
	pass

func is_signed_in() -> bool:
	# TODO
	return false
