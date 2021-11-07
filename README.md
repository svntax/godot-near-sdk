# NEAR SDK for Godot
A lightweight NEAR SDK for the Godot game engine.

## Features
- User login/logout through the [NEAR web wallet](https://wallet.near.org/).
- Calling view methods on smart contracts.
- Calling change methods on smart contracts.

## Getting Started
Download the C# Mono version of Godot 3.4. Then either clone this repository and import the project, or just copy the `addons/godot-near-sdk` directory into your own project's `addons` directory.

If you're copying the SDK over, also add `Near.gd` and `CryptoProxy.gd` as singletons through Godot's AutoLoad, and make sure that your `.csproj` file has the following elements in `<PropertyGroup>` and `<ItemGroup>`:
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
To start working with NEAR, first you must connect to the NEAR network. Use the global `Near` singleton to call the `start_connection()` method, and pass in a Dictionary with values for the network you want to connect to (mainnet or testnet). You should use testnet until your app or game is production ready.
```GDScript
# Testnet
var config = {
	"network_id": "testnet",
	"node_url": "https://rpc.testnet.near.org",
	"wallet_url": "https://wallet.testnet.near.org",
}
# Mainnet
var config = {
	"network_id": "mainnet",
	"node_url": "https://rpc.mainnet.near.org",
	"wallet_url": "https://wallet.mainnet.near.org",
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
To interact with a smart contract's change methods, users need to sign in with their NEAR wallet using `WalletConnection`'s `sign_in()` method and passing in the name of the smart contract. Calling this method will redirect the user to the NEAR web wallet to sign in.
```GDScript
wallet_connection.sign_in("example-contract.testnet")
```
To sign out:
```GDScript
wallet_connection.sign_out()
```
Note that `WalletConnection` has signals `user_signed_in` and `user_signed_out`, which emit whenever the user has successfully signed in or signed out.

To check if a user is signed in or not:
```GDScript
if wallet_connection.is_signed_in():
    pass # Your code here
```

To get the user's account ID:
```GDScript
wallet_connection.get_account_id() # Returns the account ID as a string, or an empty string if not signed in
```

### Call View Methods
To call a smart contract's view methods, use `Near.call_view_method()` and pass in the contract name, the method name, and if required, a Dictionary with arguments matching the view method.

The return value will have a "data" field with a decoded string of the return value from the smart contract, so depending on the smart contract, it could be a normal string or a JSON string.

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

### Call Change Methods
To call a smart contract's change methods, use `WalletConnection`'s `call_change_method()` method and pass in the contract name, the method name, a Dictionary with arguments matching the change method (or an empty Dictionary if no arguments), and optionally, an attached gas amount and deposit for the transaction.

If no deposit is attached (default is zero), the return value will be a Dictionary containing data on the transaction (see https://docs.near.org/docs/api/rpc/transactions#send-transaction-await for more info.)

If a deposit greater than zero is passed in, the user will be redirected to the NEAR web wallet instead to confirm the transaction, and the return value will be a Dictionary containing a "message" field only. In this case, once the user confirms the transaction, the wallet connection will emit a `transaction_hash_received` signal with a string representing the hash of the confirmed transaction.

If the user's access key is low on allowance, the result will contain a "warning" field with a value of "NotEnoughAllowance", and the user will be redirected to sign in again to create a new access key.

If an error occurs, the result will contain an "error" field. 
```GDScript
# Listening for the transaction_hash_received signal
wallet_connection.connect("transaction_hash_received", self, "_on_tx_hash_received")
```
```GDScript
# Function to handle receiving the transaction hash
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
- Once Godot 4.0 is out, the SDK will need to be updated due to changes to coroutines and the replacement of `yield` with `await`. In the meantime, any calls to `call_change_method()` and `call_view_method()` require checking if the return value is a GDScriptFunctionState, and if so, yield until completed.
- There is no handling for nonce collisions when sending change call transactions due to relying on the `data` field from the error returned, which is considered legacy and may be deprecated (see https://docs.near.org/docs/api/rpc/transactions#what-could-go-wrong-1 for more info.)
