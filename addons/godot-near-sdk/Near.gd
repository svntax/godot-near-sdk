extends Node

# Gas units are in yoctoNEAR
const MAX_GAS = 300000000000000 # 300 TGas
const DEFAULT_FUNCTION_CALL_GAS = 30000000000000 # 30 Tgas

onready var http = $HTTPRequest

var near_connection: NearConnection
var websocket_client: WebSocketClient

func start_connection(config: Dictionary) -> void:
	near_connection = NearConnection.new(config)

func _process(delta):
	if websocket_client:
		websocket_client.poll()

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
			"public_key": public_key
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

# Converts a given amount of NEAR into yoctoNEAR
func from_near(amount: String) -> String:
	if amount.empty() or not amount.is_valid_float():
		return "0"
	
	var parts = amount.split(".")
	var whole = parts[0]
	var fraction = parts[1] if parts.size() > 1 else ""
	
	# Pad fractional part with zeroes up to 24 digits
	while fraction.length() < 24:
		fraction += "0"
	# Trim to exactly 24 digits if it was longer
	if fraction.length() > 24:
		fraction = fraction.substr(0, 24)
	
	# Combine whole and fraction (fraction is now exactly 24 digits)
	if whole == "0" and fraction.empty():
		return "0"
	
	return whole + fraction

# Converts a given amount of yoctoNEAR into NEAR 
func from_yoctonear(amount: String) -> String:
	if amount.empty() or not amount.is_valid_integer():
		return "0"
	
	var padded = amount
	# Pad with leading zeroes if needed to make it at least 25 digits (1 + 24 fractional)
	while padded.length() <= 24:
		padded = "0" + padded
	
	var split_pos = padded.length() - 24
	var whole_part = padded.substr(0, split_pos)
	var fraction_part = padded.substr(split_pos, 24)
	
	# Trim trailing zeros from fraction
	while fraction_part.ends_with("0"):
		fraction_part = fraction_part.substr(0, fraction_part.length() - 1)
	
	if fraction_part.empty():
		return whole_part
	else:
		return whole_part + "." + fraction_part

# Methods to construct NEAR transactions

func createAccountAction() -> Dictionary:
	return {
		"type": "CreateAcction"
	}

func deployContractAction(code: PoolByteArray) -> Dictionary:
	return {
		"type": "DeployContract",
		"params": {
			"code": code
		}
	}

func functionCallAction(method_name: String, args: Dictionary, gas: String, deposit: String) -> Dictionary:
	return {
		"type": "FunctionCall",
		"params": {
			"methodName": method_name,
			"args": args,
			"gas": gas,
			"deposit": deposit
		}
	}

func transferAction(deposit: String) -> Dictionary:
	return {
		"type": "Transfer",
		"params": {
			"deposit": deposit
		}
	}

func stakeAction(stake: String, public_key: String) -> Dictionary:
	return {
		"type": "Stake",
		"params": {
			"stake": stake,
			"publicKey": public_key
		}
	}

func addKeyAction() -> Dictionary:
	return {}
	# TODO AddKeyAction with AddKeyPermission interface
#	type: "AddKey";
#	params: {
#		publicKey: string;
#		accessKey: {
#		  nonce?: number;
#		  permission: AddKeyPermission;
#		};
#	};

func deleteKeyAction(public_key: String) -> Dictionary:
	return {
		"type": "DeleteKey",
		"params": {
			"publicKey": public_key
		}
	}

func deleteAccountAction(beneficiary_id: String) -> Dictionary:
	return {
		"type": "DeleteAccount",
		"params": {
			"beneficiaryId": beneficiary_id
		}
	}

func createTransaction(signer_id: String, receiver_id: String, actions: Array) -> Dictionary:
	return {
		"signerId": signer_id,
		"receiverId": receiver_id,
		"actions": actions
	}
