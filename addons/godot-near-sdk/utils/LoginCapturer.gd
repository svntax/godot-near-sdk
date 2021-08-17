extends "res://addons/godot-near-sdk/utils/httpserver.gd"

func _respond(request: Request) -> Response:
#	var body := PoolByteArray()
#	body.append_array(("Method: %s\n" % request.method).to_ascii())
#	body.append_array(("Path: %s\n" % request.request_path).to_ascii())
#	body.append_array(("Query: %s\n" % request.request_query).to_ascii())
#	for header in request.headers:
#		body.append_array((header + "\n").to_ascii())
#	body.append_array("Body: ".to_ascii())
#	body.append_array(request.request_data)

	var response = ""
	if request.method == "GET" and request.request_path == "/":
		var values = request.request_query.split("&")
		for param in values:
			if param.begins_with("account_id="):
				var account_id = param.trim_prefix("account_id=")
				CryptoProxy.save_account_id(account_id)
				var response_message = "Signed in as: " + account_id
				request.peer.put_data(response_message.to_ascii())
				CryptoProxy.stop_login_server()
	
	return null
