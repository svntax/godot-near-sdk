extends "res://addons/godot-near-sdk/utils/httpserver.gd"

func _respond(request: Request) -> Response:
	var response_message = "Error while trying to sign in."
	if request.method == "GET" and request.request_path == "/":
		var values = request.request_query.split("&")
		var account_id: String = ""
		var public_key: String = ""
		for param in values:
			if param.begins_with("account_id="):
				account_id = param.trim_prefix("account_id=")
				response_message = "Signed in as: " + account_id + "\nYou may now close this window."
				#CryptoProxy.save_account_id(account_id)
			elif param.begins_with("public_key="):
				public_key = param.trim_prefix("public_key=")
		CryptoProxy.save_account_data(account_id, public_key)
		CryptoProxy.stop_server()
	
	var body := PoolByteArray()
	body.append_array(response_message.to_ascii())
	var response_obj := Response.new()
	response_obj.body = body
	return response_obj
