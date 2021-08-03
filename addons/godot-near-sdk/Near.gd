extends Node

onready var http = $HTTPRequest

#func test_view_account():
#	var data_to_send = {
#		"jsonrpc": "2.0",
#		"id": "dontcare",
#		"method": "query",
#		"params": {
#			"request_type": "view_account",
#			"finality": "final",
#			"account_id": "nearkat.testnet"
#		}
#	}
#	var query = JSON.print(data_to_send)
#	var url = "https://rpc.testnet.near.org"
#	var headers = ["Content-Type: application/json"]
#	var use_ssl = false
#	http.request(url, headers, use_ssl, HTTPClient.METHOD_POST, query)

func test_get_levels() -> String:
	var data_to_send = {
		"jsonrpc": "2.0",
		"id": "dontcare",
		"method": "query",
		"params": {
			"request_type": "call_function",
			"finality": "final",
			"account_id": "levels-browsing.svntax.testnet",
			"method_name": "getLevels",
			"args_base64": "e30="
		}
	}
	var query = JSON.print(data_to_send)
	var url = "https://rpc.testnet.near.org"
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

	# https://docs.near.org/docs/api/rpc#call-a-contract-function
	# The json returned has an array of bytes called "result", convert this to a string
	# Note: first result var is from JSONParseResult.result, the next two are from NEAR
	var array = json.result.result.result
	var byte_array = PoolByteArray(array)
	var string_result = byte_array.get_string_from_utf8()
	return string_result
