extends Reference
class_name NearConnection

var network_id: String setget ,get_network_id
var node_url: String setget ,get_node_url
var wallet_url: String setget ,get_wallet_url

func _init(config: Dictionary):
	assert(config.has_all(["network_id", "node_url", "wallet_url"]))
	
	network_id = config.get("network_id")
	node_url = config.get("node_url")
	wallet_url = config.get("wallet_url")

func get_network_id() -> String:
	return network_id

func get_node_url() -> String:
	return node_url

func get_wallet_url() -> String:
	return wallet_url
