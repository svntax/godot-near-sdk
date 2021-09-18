extends "res://addons/godot-near-sdk/utils/httpserver.gd"

func _respond(request: Request) -> Response:
	var response_message = "Error while trying to sign in."
	if request.method == "GET" and request.request_path == "/":
		var values = request.request_query.split("&")
		for param in values:
			if param.begins_with("account_id="):
				var account_id = param.trim_prefix("account_id=")
				response_message = "Signed in as: " + account_id + "\nYou may now close this window."
				CryptoProxy.save_account_id(account_id)
		CryptoProxy.stop_server()
	
	var body := PoolByteArray()
	body.append_array(response_message.to_ascii())
	var response_obj := Response.new()
	response_obj.body = body
	return response_obj
