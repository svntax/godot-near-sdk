extends Node

onready var http = $HTTPRequest

var near_connection: NearConnection

func start_connection(config: Dictionary) -> NearConnection:
	near_connection = NearConnection.new(config)
	return near_connection

func call_view_method(account_id: String, method_name: String, args: Dictionary = {}) -> String:
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
	http.request(url, headers, use_ssl, HTTPClient.METHOD_POST, query)
	
	# [result, status code, response headers, body]
	var response = yield(http, "request_completed")
	if response[0] != OK:
		push_error("An error occurred in the HTTP request.")
		return
	
	var body = response[3]
	var json = JSON.parse(body.get_string_from_utf8())
	if json.error != OK:
		push_error("Error when parsing JSON.")
		return

	var json_result = json.result
	if json_result.has("error"):
		var error = json_result.error
		var message = error.message + " " + str(error.code) + ": " + error.cause.name
		push_error(message)
		return message
	
	var rpc_result = json_result.result
	if rpc_result.has("error"):
		push_error(rpc_result.error)
		return rpc_result.error
	
	var result_bytes = rpc_result.result
	var byte_array = PoolByteArray(result_bytes)
	var string_result = byte_array.get_string_from_utf8()
	return string_result
