extends Node

# Gas units are in yoctoNEAR
const MAX_GAS = 300000000000000 # 300 TGas
const DEFAULT_FUNCTION_CALL_GAS = 30000000000000 # 30 Tgas

onready var http = $HTTPRequest

var near_connection: NearConnection

func start_connection(config: Dictionary) -> void:
	near_connection = NearConnection.new(config)

func call_view_method(account_id: String, method_name: String, args: Dictionary = {}) -> Dictionary:
	assert(near_connection != null)
	
	var args_encoded = "e30="
	if not args.empty():
		var args_json_string = JSON.print(args)
		args_encoded = Marshalls.utf8_to_base64(args_json_string)
	
	var data_to_send = {
		"jsonrpc": "2.0",
		"id": "dontcare",
		"method": "query",
		"params": {
			"request_type": "call_function",
			"finality": "final",
			"account_id": account_id,
			"method_name": method_name,
			"args_base64": args_encoded
		}
	}
	var query = JSON.print(data_to_send)
	var url = near_connection.node_url
	var headers = ["Content-Type: application/json"]
	var use_ssl = false
	
	var rpc_result = query_rpc(url, headers, use_ssl, HTTPClient.METHOD_POST, query)
	
	if rpc_result is GDScriptFunctionState:
		rpc_result = yield(rpc_result, "completed")
	
	if rpc_result.has("error"):
		return rpc_result
	
	var result_bytes = rpc_result.result
	var byte_array = PoolByteArray(result_bytes)
	var string_result = byte_array.get_string_from_utf8()
	return {
		"data": string_result
	}

func block_query_latest() -> Dictionary:
	var data_to_send = {
		"jsonrpc": "2.0",
		"id": "dontcare",
		"method": "block",
		"params": {
			"finality": "final"
		}
	}
	var query = JSON.print(data_to_send)
	var url = near_connection.node_url
	var headers = ["Content-Type: application/json"]
	var use_ssl = false
	
	var rpc_result = query_rpc(url, headers, use_ssl, HTTPClient.METHOD_POST, query)
	
	if rpc_result is GDScriptFunctionState:
		rpc_result = yield(rpc_result, "completed")
	
	return rpc_result

func view_access_key(account_id: String, public_key: String) -> Dictionary:
	var data_to_send = {
		"jsonrpc": "2.0",
		"id": "dontcare",
		"method": "query",
		"params": {
			"request_type": "view_access_key",
			"finality": "final",
			"account_id": account_id,
			"public_key": "ed25519:" + public_key
		}
	}
	var query = JSON.print(data_to_send)
	var url = near_connection.node_url
	var headers = ["Content-Type: application/json"]
	var use_ssl = false
	
	var rpc_result = query_rpc(url, headers, use_ssl, HTTPClient.METHOD_POST, query)
	
	if rpc_result is GDScriptFunctionState:
		rpc_result = yield(rpc_result, "completed")
	
	return rpc_result

func query_rpc(url: String, headers: Array, use_ssl: bool, method: int, query: String) -> Dictionary:
	http.request(url, headers, use_ssl, method, query)
	
	# [result, status code, response headers, body]
	var response = yield(http, "request_completed")
	if response[0] != OK:
		var message = "An error occurred in the HTTP request."
		push_error(message)
		return create_error_response(message)
	
	var body = response[3]
	var json = JSON.parse(body.get_string_from_utf8())
	if json.error != OK:
		var message = "Error when parsing JSON response."
		push_error(message)
		return create_error_response(message)

	var json_result = json.result
	if json_result.has("error"):
		var error = json_result.error
		var message = error.message + " " + str(error.code) + ": " + error.cause.name
		message += "\n" + JSON.print(error.cause.info)
		push_error(message)
		return json_result
	
	var rpc_result = json_result.result
	if rpc_result.has("error"):
		push_error(rpc_result.error)
		return create_error_response(rpc_result.error)
	
	return rpc_result

# Format for error messages
func create_error_response(message: String) -> Dictionary:
	return {
		"error": {
			"message": message
		}
	}
