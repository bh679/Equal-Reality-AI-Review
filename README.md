# PUN-Discord-Webhooks
Discord Webhooks for PUN2.
 - Updates when a player joins a room with players in room

## Requirements
Unity (most recent tested on 2021.3.16f1) <br />
[PUN2](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922) (free) 

## License
[Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)](https://creativecommons.org/licenses/by-sa/4.0/)

## Installing with Unity Package Manager
First install [PUN2](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922)

To install this project as a dependency using the Unity Package Manager,
Install requirements above.
add the following line to your project's `manifest.json`:

```
"com.brennanhatton.discord": "git+https://github.com/bh679/Unity-Discord-Webhook-Tools.git"
```

of 
Windows -> Package Manager -> '+' -> `add package from git URL...` ->
```
https://github.com/bh679/Unity-Discord-Webhook-Tools.git
```

Join the [Discord](https://discord.gg/VC8gZ2GNHs "Join Discord server") server to leave feedback or get support.

## Using
How to make a webhook link:

1 - Make new channels for the webhooks (a dev one and default)

2 - Make the webbooks that point to each
Discord/ServerSettings/Intergrations/Create Webhook

3 - Save webhooks in TextAsset files in PRIVATE (so they do not sync with public version control)

4 - Add Discord Logging prefab to scene
 - Reference the webhook text assets
 
