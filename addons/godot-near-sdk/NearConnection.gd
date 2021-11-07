extends Reference
class_name NearConnection

const USER_DATA_SAVE_PATH = "user://user_data.cfg"

signal user_data_updated()

var network_id: String setget ,get_network_id
var node_url: String setget ,get_node_url
var wallet_url: String setget ,get_wallet_url

var user_config = ConfigFile.new()

func _init(config: Dictionary):
	assert(config.has_all(["network_id", "node_url", "wallet_url"]))
	
	network_id = config.get("network_id")
	node_url = config.get("node_url")
	wallet_url = config.get("wallet_url")
	
	var dir = Directory.new()
	if dir.file_exists(USER_DATA_SAVE_PATH):
		var err = user_config.load(USER_DATA_SAVE_PATH)
		if err != OK:
			push_error("Failed to load user data. Error code %s" % err)
	
	# If the NEAR web wallet has redirected the user to this game with a query parameter account_id,
	# save that id as the current active user.
	if OS.has_feature("JavaScript"):
		var window_location = JavaScript.get_interface("location")
		var query_string = window_location.search
		if query_string.begins_with("?"):
			query_string = query_string.substr(1)
			var values = query_string.split("&")
			for param in values:
				if param.begins_with("account_id="):
					var account_id = param.trim_prefix("account_id=")
					# Move the keys from temporary to stored state and add the account id
					var public_key = user_config.get_value("temp", "public_key", "")
					var private_key = user_config.get_value("temp", "private_key", "")
					if !public_key.empty() and !private_key.empty():
						user_config.set_value("user", "public_key", public_key)
						user_config.set_value("user", "private_key", private_key)
						user_config.set_value("user", "account_id", account_id)
						user_config.erase_section("temp")
						save_user_data()
					else:
						var prev_account_id = user_config.get_value("user", "account_id", "")
						if account_id == prev_account_id:
							# User is already signed in
							pass
						else:
							push_error("Error retrieving temporary key pair.")
			# Clean the url by removing the url query
			var window_history = JavaScript.get_interface("history")
			var base_url = window_location.href.split("?")[0]
			window_history.replaceState({}, "", base_url)

func get_network_id() -> String:
	return network_id

func get_node_url() -> String:
	return node_url

func get_wallet_url() -> String:
	return wallet_url

func save_user_data() -> void:
	user_config.save(USER_DATA_SAVE_PATH)
	emit_signal("user_data_updated")
