# Snake-Multiplayer
This is a Multiplayer implementation attempt of the original snake game, using Unity and UDP multicast.

The game should be restartable once the server is running.
If the server crashes then a restart should let you play again.

## To start the server
---
Running in Visual Studio IDE:
1. Open the `/GameNetwork/GameNetwork.sln` in Visual Studio.
2. Either run Start by selecting `Debug/Start Debugging` or `Debug/Start Without Debugging` from the Toolbar.

Running Build executable:
1. Run `ServerBuild/GameNetwork.exe`.

## To Run the game
---
This requires the Server to run.
1. Open Unity Editor.
2. Select `File/Build And Run` from the Toolbar.
3. If it asks select a folder where you want the build to be put. (e.x.: `C:/temp/`)
4. After the game have opened, click on `Start > Multiplayer`.
5. Wait for other players to join.
6. Click `Start` to start the game.
