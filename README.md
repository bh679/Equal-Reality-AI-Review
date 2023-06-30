# Equal Reality Review AI
Ai Review Scene for Equal Reality & People Tech Revolution Projects

## Requirements
Unity (most recent tested on 2021.3.20f1) <br />
[SpeechBlend](https://assetstore.unity.com/packages/tools/animation/speechblend-lipsync-149023) - paid ($14.99)<br />
[GPT AI Integration](https://assetstore.unity.com/packages/tools/ai/gpt-ai-integration-243729) - paid ($25)<br />
[CleanFlatIcon](https://assetstore.unity.com/packages/2d/gui/icons/clean-flat-icons-98117) (optional) <br />
[Unity GPT Tools](https://github.com/bh679/Unity-GPT-Tools) - 0.0.2 <br />
[Unity Tools](https://github.com/bh679/Unity-Tools) - 1.6.2<br />
[Unity Discord Webhooks](https://github.com/bh679/Unity-Discord-Webhook-Tools) - 1.2.3<br />

## License
[Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)](https://creativecommons.org/licenses/by-sa/4.0/)

## Installation
Import UnityAssets (I find this is best done by searching the package manager):
- [SpeechBlend](https://assetstore.unity.com/packages/tools/animation/speechblend-lipsync-149023)<br />
- [GPT AI Integration](https://assetstore.unity.com/packages/tools/ai/gpt-ai-integration-243729)<br />
- [CleanFlatIcon](https://assetstore.unity.com/packages/2d/gui/icons/clean-flat-icons-98117) (optional) <br />

To install this project as a dependency using the Unity Package Manager,
Install the requirements. <br/ >
Add the following line to your project's `manifest.json`:

```
    "com.brennanhatton.discord": "https://github.com/bh679/Unity-Discord-Webhook-Tools.git",
    "com.brennanhatton.gpt": "https://github.com/bh679/Unity-GPT-Tools.git",
    "com.brennanhatton.unitytools": "https://github.com/bh679/Unity-Tools.git",
    "com.equalreality.reviewai": "https://github.com/bh679/Equal-Reality-AI-Review.git"
```
or
Windows -> Package Manager -> '+' -> `add package from git URL...` -> <br />
``https://github.com/bh679/Unity-Tools.git``<br />
``https://github.com/bh679/Unity-GPT-Tools.git``<br />
``https://github.com/bh679/Unity-Discord-Webhook-Tools.git``<br />
```
https://github.com/bh679/Equal-Reality-AI-Review.git
```

You will get this error
<img width="966" alt="Screenshot 2023-06-30 at 3 00 55 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/ed71bbec-4441-4602-acbb-809bdb8f7b7e">

Import Assembly Definions .unitypackage

## Setup
### Add Prefabs
Add ``AI Manager.prefab`` to the root of the hirarchy <br />
<img width="263" alt="Screenshot 2023-06-30 at 3 54 16 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/8b659943-bdfb-4d8b-bf76-16011dfa0777">

Add ``Ai Review.prefab`` to replace the review knot<br />
<img width="216" alt="Screenshot 2023-06-30 at 3 54 58 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/e302c61d-0808-4bed-b4b9-6e333eb1dc78">

### Setup Speechblend
Add ``Speechblend.cs`` Component to the ai avatar (at the same level as the ``Animator``.<br />
<img width="1686" alt="Screenshot 2023-06-30 at 3 55 21 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/37d8215e-c96d-4fc5-9772-296d5b1362f4">

Set the ``Head Mesh`` to the ``CC_Base_Body`` child.<br />
<img width="1686" alt="Screenshot 2023-06-30 at 3 56 44 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/6be88248-954f-48c7-8ea2-3c4921a3f3b5">

Tracking mode to ``Jaw and Visemes``<br />
<img width="789" alt="Screenshot 2023-06-30 at 3 57 26 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/4c1d426b-14e9-45ad-abf6-83e3f6256c01">

Shape Template to ``CC3`` and click **Auto-Detect**<br />
<img width="794" alt="Screenshot 2023-06-30 at 3 57 44 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/5cdf737d-7392-4750-8a28-49cd5c7a4297">

Setup the reference from the ``AI Manager`` object ``AiAnimationController.cs`` component ``VR Guide Animator`` to reference the SpeechBlend ai avatar.<br />
<img width="827" alt="Screenshot 2023-06-30 at 3 58 19 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/9a29d529-18b4-4849-98f2-49debc3776d2">

### Setup the PrivateKeys
- Elevent Labs ``Assets/GPT AI Intergration/ElevenLabs/Runtime/Config``
<img width="447" alt="Screenshot 2023-06-30 at 3 59 38 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/621ba873-7af9-4ea4-8dcc-d4fb254bba19">
- OpenAi ``Assets/GPT AI Intergration/OpenAi/Runtime/Config``
- <img width="408" alt="Screenshot 2023-06-30 at 4 00 28 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/71be09dc-cea3-4049-9e66-edde56f16ff6">

Join the [Discord](https://discord.gg/VC8gZ2GNHs "Join Discord server") server to leave feedback or get support.

## Using
Section to be written
 
 
## Documentation
 - [AI Review Feature Specifications](https://docs.google.com/document/d/1ccq_VnhZ-AAbUW-jhwOcOB_fO1q0LPr8/edit)<br />
 - [EqualReality.AI Dependencies Map](https://docs.google.com/presentation/d/1Y2eK51DfQYKYzhT-jNj3jHkHUAtdsLeV46JADhA2G_E/edit#slide=id.g2528c1e646d_0_17)<br />
