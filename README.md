# NEAR SDK for Godot
A lightweight NEAR SDK for the Godot game engine.

*This version of the SDK is for Godot 3.6.1*

## Features
- Sign in/sign out with [Intear Wallet](https://wallet.intear.tech/).
- Call view methods on smart contracts.
- Sign messages
- Send transactions

## Getting Started
Download the C# Mono version of Godot 3.6.1. Then either clone this repository and import the project, or just copy the `addons/godot-near-sdk` directory into your own project's `addons` directory.

If you're copying the SDK over, also add `Near.tscn` and `CryptoProxy.gd` as singletons through Godot's AutoLoad, and make sure that your `.csproj` file has the following elements in `<PropertyGroup>` and `<ItemGroup>`:
```xml
<PropertyGroup>
  <TargetFramework>net472</TargetFramework>
  <LangVersion>latest</LangVersion>
  <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
</PropertyGroup>
```
```xml
<ItemGroup>
  <PackageReference Include="Rebex.Elliptic.Ed25519" Version="1.2.1" />
  <PackageReference Include="SimpleBase" Version="2.1.0" />
</ItemGroup>
```

### Connect to NEAR
To start working with NEAR, first you must connect to the NEAR network. Use the global `Near` singleton to call the `start_connection()` method, and pass in a Dictionary with values for the network (mainnet or testnet), RPC provider, and wallet provider (Intear only for now). You should use testnet until your app or game is production ready.
```GDScript
# Testnet
var config = {
	"network_id": "testnet",
	"node_url": "https://rpc.testnet.fastnear.com", # See other providers in https://docs.near.org/api/rpc/providers
	"wallet_provider": WalletProviders.Wallet.INTEAR # Only Intear wallet is fully supported. MyNearWallet is partially working, but not recommended.
}
# Mainnet
var config = {
	"network_id": "mainnet",
	"node_url": "https://free.rpc.fastnear.com",
	"wallet_provider": WalletProviders.Wallet.INTEAR
}

# Create the connection
Near.start_connection(config)
```

### Create a wallet connection
After connecting to a NEAR network, you can connect to it with a wallet by creating a new `WalletConnection` object and passing in the newly created NEAR connection.
```GDScript
var wallet_connection = WalletConnection.new(Near.near_connection)
```

### Sign In/Sign Out
To interact with the NEAR network, users need to sign in with their NEAR account using `WalletConnection`'s `sign_in()` method, and optionally pass in the name of the smart contract and methods.

With Intear Wallet, users can pick a wallet connection method (web, web beta (staging), desktop), so we use a separate menu to prompt the user to select one.

**NOTE: Desktop builds currently support Intear's desktop wallet only. Web builds can use all connection methods.**

See `intear_selector.tscn` for the default menu example. Feel free to create your own variation of this scene, as long as you use the same script for the scene and call `open()` and `close()` in the right places, and listen for the `selector_closed` signal.
```GDScript
onready var intear_selector = $"%IntearSelector" # Reference to an IntearSelector instance

func _ready():
	intear_selector.connect("selector_closed", self, "_on_intear_selector_closed") # Listen for the signal emitted when the selector menu is closed

# Open the selector menu
intear_selector.open(wallet_connection, "contract ID here", ["method1", "method2"])

# Close the selector menu
intear_selector.close()
```

To sign out the user:
```GDScript
wallet_connection.sign_out()
```

### Wallet events
`WalletConnection` has signals you can connect to:
- `user_signed_in` and `user_signed_out` emit whenever the user has successfully signed in or signed out.
- `connected_response` emits after a sign-in request is sent.
- `signed_message_response` emits after a sign-message request is sent.
- `sent_transactions_response` emits after a send-transactions request is sent.
- `error_response` emits if any of the wallet requests respond with an error type.
- `websocket_closed` emits when a `WalletConnection`'s connection to Intear Wallet's websockets bridge is closed (see https://github.com/INTEARnear/wallet/blob/main/POSTMESSAGE_PROTOCOL.md for how websockets is used with Intear).

### Get user and app data
After signing in, a new key pair is created for your app and saved in `NearConnection.USER_DATA_SAVE_PATH` as a config file, along with the user's account ID.

Here are methods to access the saved user data:

To check if a user is signed in or not:
```GDScript
if wallet_connection.is_signed_in():
    pass # Your code here
```

To get the user's account ID:
```GDScript
wallet_connection.account_id
```

To get your app's public key:
```GDScript
wallet_connection.get_app_public_key()
```

To get your app's private key:
```GDScript
wallet_connection.get_app_private_key()
```

To check if a FunctionCall key was added on sign-in:
```GDScript
wallet_connection.is_function_call_key_added()
```

### Call View Methods
To call a smart contract's view methods, use `Near.call_view_method()` and pass in the contract name, the method name, and if required, a Dictionary with arguments matching the view method.

The return value will have a "data" field with a decoded string of the return value from the smart contract, so depending on the smart contract, it could be a normal string, or a JSON string.

If an error occurs, the return value will contain an "error" field.
```GDScript
# View method without args
var result = Near.call_view_method("example-contract.testnet", "someMethod")
if result is GDScriptFunctionState:
    result = yield(result, "completed")
if result.has("error"):
    pass # Error handling here
else:
    var data = result.data
    # Your code here
```
```GDScript
# View method with args
var result = Near.call_view_method("example-contract.testnet", "someMethodWithArgs", {"arg1": "value1"})
if result is GDScriptFunctionState:
    result = yield(result, "completed")
if result.has("error"):
    pass # Error handling here
else:
    var data = result.data
    # Your code here
```

### Sign messages
To prompt the user to sign a message:
```GDScript
var message = "Hello, this is a test message."
var recipient = "Test app"
var result = wallet_connection.sign_message(message, recipient)
if result is GDScriptFunctionState:
	result = yield(result, "completed")
if result.has("error"):
	pass # Error handling here
else:
	pass # Response handling here, see https://github.com/INTEARnear/wallet/blob/main/POSTMESSAGE_PROTOCOL.md#3-sign-message-flow-sign-message
```

### Sign and send transactions
To send transactions, you need to construct a transaction object and array of actions for each transaction. Then pass an array of transaction objects to the `send_transactions()` method:
```GDScript
# Send a function call transaction to a smart contract with a "write" method
var write_transaction = Near.createTransaction(wallet_connection.account_id, "contract ID here",
	[
		Near.functionCallAction(
			"write",
			{"key": "message", "value": input_text},
			str(Near.DEFAULT_FUNCTION_CALL_GAS),
			"0"
		)
	]
)
var transactions = [write_transaction]
var result = wallet_connection.send_transactions(transactions)
if result is GDScriptFunctionState:
	result = yield(result, "completed")
if result.has("error"):
	pass # Error handling here
else:
	pass # Response handling here, see https://github.com/INTEARnear/wallet/blob/main/POSTMESSAGE_PROTOCOL.md#4-send-transactions-flow-send-transactions
```

See `Near.gd` for a list of helper methods to construct the different types of actions.

If you need to set the gas and/or deposit yourself, you must convert to yoctoNEAR when passing in the values.
```GDScript
# Deposit of 0.1 NEAR
var deposit = "0.1"
var deposit_yocto = Near.from_near(deposit)
```

### (DEPRECATED) Call Change Methods
To call a smart contract's change methods, use `WalletConnection`'s `call_change_method()` method and pass in the contract name, the method name, a Dictionary with arguments matching the change method (or an empty Dictionary if no arguments), and optionally, an attached gas amount and deposit for the transaction.

If no deposit is attached (default is zero), the return value will be a Dictionary containing data on the transaction (see https://docs.near.org/docs/api/rpc/transactions#send-transaction-await for more info.)

If a deposit greater than zero is passed in, the user will be redirected to the NEAR web wallet instead to confirm the transaction, and the return value will be a Dictionary containing a "message" field only. In this case, once the user confirms the transaction, one of the following will happen:
- **For desktop/mobile builds**: The wallet connection will emit a `transaction_hash_received` signal with the hash of the confirmed transaction as a string.
- **For web builds**: No signal will be emitted. You'll have to handle what to do next after the transaction yourself.

If the user's access key is low on allowance, the result will contain a "warning" field with a value of "NotEnoughAllowance", and the user will be redirected to sign in again to create a new access key.

If an error occurs, the result will contain an "error" field. 
```GDScript
# Listening for the transaction_hash_received signal (For desktop/mobile)
wallet_connection.connect("transaction_hash_received", self, "_on_tx_hash_received")
```
```GDScript
# Function to handle receiving the transaction hash (For desktop/mobile)
func _on_tx_hash_received(tx_hash: String):
    pass # Your code here
```
```GDScript
# Calling the change method
var result = wallet_connection.call_change_method("example-contract.testnet", "someMethod", {}, gas_amount, deposit_amount)
if result is GDScriptFunctionState:
    result = yield(result, "completed")
if result.has("error"):
    pass # Error handling here
elif result.has("warning"):
    pass # User's access key was low on allowance
elif result.has("message"):
    pass # Transaction with a deposit was made
else:
    pass # Transaction without a deposit was made
```

## Notes
- Once Godot 4.0 is out, the SDK will need to be updated due to changes to coroutines and the replacement of `yield` with `await`. In the meantime, any calls to `sign_message()`, `send_transactions()`, and `call_view_method()` require checking if the return value is a GDScriptFunctionState, and if so, yield until completed.
