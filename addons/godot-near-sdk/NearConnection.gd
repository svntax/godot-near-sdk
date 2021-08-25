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

func get_network_id() -> String:
	return network_id

func get_node_url() -> String:
	return node_url

func get_wallet_url() -> String:
	return wallet_url

func save_user_data() -> void:
	user_config.save(USER_DATA_SAVE_PATH)
	emit_signal("user_data_updated")
