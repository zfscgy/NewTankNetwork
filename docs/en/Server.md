# Preview

In this game, I use PhotonServer to do the Networking. Here, the server I mean, you can just take it as a **MasterClient**, which is in charge of most of the game logic and physics.

For exampe: 

* At the LobbyScene, the server will create a room, waiting for player's joining.
* Once a player joins, it will call server with RPC method provided by PhotonServer to get one seat.

