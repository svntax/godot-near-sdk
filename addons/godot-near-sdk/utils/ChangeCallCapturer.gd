extends "res://addons/godot-near-sdk/utils/httpserver.gd"

func _respond(request: Request) -> Response:
	var response_message = "Error getting transaction hash."
	if request.method == "GET" and request.request_path == "/":
		var values = request.request_query.split("&")
		for param in values:
			if param.begins_with("transactionHashes="):
				# Note: assumes only 1 transaction hash
				var tx_hash = param.trim_prefix("transactionHashes=")
				response_message = "Transaction hash: " + tx_hash + "\nYou may now close this window."
				CryptoProxy.receive_transaction_hash(tx_hash)
		CryptoProxy.stop_server()
	
	var body := PoolByteArray()
	body.append_array(response_message.to_ascii())
	var response_obj := Response.new()
	response_obj.body = body
	return response_obj
