# Equal Reality Review AI
AI Review Scene for Equal Reality & People Tech Revolution Projects

## Table of Contents
1. [Requirements](#requirements)
2. [License](#license)
3. [Installation](#installation)
4. [Setup](#setup)
5. [Edit Prompts & Script](#edit-prompts--script)
6. [Internal Documentation](#internal-documentation)
7. [Events](#events)
8. [Feedback & Community](#feedback--community)

## License

Equal Reality Review AI is licensed under [Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)](https://creativecommons.org/licenses/by-sa/4.0/), which allows you to copy and redistribute the material in any medium or format with unlimited allowance for adaptation and transformation. 

Equal Reality must be properly credited, a link to the license provided within the material itself, the changes made properly indicated, and the edited material likewise distributed under the same license.

More details are available in the above link.


## Requirements
The following assets and extensions are necessary or highly recommended:

- [Unity](https://unity.com/releases/editor/archive) (most recently tested on 2021.3.20f1)
- [SpeechBlend](https://assetstore.unity.com/packages/tools/animation/speechblend-lipsync-149023) - Paid ($14.99 USD)
- [GPT AI Integration](https://assetstore.unity.com/packages/tools/ai-ml-integration/gpt-ai-integration-243729) - Paid ($25.00 USD)
- [Unity GPT Tools](https://github.com/bh679/Unity-GPT-Tools) v0.0.2 (Free)
- [Unity Tools](https://github.com/bh679/Unity-Tools) v1.6.2 (Free)
- [Unity Discord Webhooks](https://github.com/bh679/Unity-Discord-Webhook-Tools) v1.2.3 (Free)
- [CleanFlatIcon](https://assetstore.unity.com/packages/2d/gui/icons/clean-flat-icons-98117) ($19.99 USD, Optional)


## Installation
Reminder: Do **NOT** move any installed folders until installation is finished and properly tested.

1. Import the following assets:
- UnityAssets
- SpeechBlend
- GPT AI Integration
- CleanFlatIcon

2. To install this project as a dependency using the Unity Package Manager add the following lines to the *manifest.json*:
```
   "com.brennanhatton.discord": "https://github.com/bh679/Unity-Discord-Webhook-Tools.git",
   "com.brennanhatton.gpt": "https://github.com/bh679/Unity-GPT-Tools.git",
   "com.brennanhatton.unitytools": "https://github.com/bh679/Unity-Tools.git",
   "com.equalreality.reviewai": "https://github.com/bh679/Equal-Reality-AI-Review.git"
```
Or access 'Windows -> Package Manager -> '+' -> add package from git URL…' and add the following lines:
```
   https://github.com/bh679/Unity-Tools.git
   https://github.com/bh679/Unity-GPT-Tools.git
   https://github.com/bh679/Unity-Discord-Webhook-Tools.git
   https://github.com/bh679/Equal-Reality-AI-Review.git
```

Pictured below is a known error that will occur upon installation:

<img width="966" alt="Screenshot 2023-06-30 at 3 00 55 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/ed71bbec-4441-4602-acbb-809bdb8f7b7e">

Finish the installation by importing the Assembly Definitions .unitypackage to fix the error.

<img width="720" alt="Screenshot 2023-07-04 at 11 29 26 am" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/d21789b1-405c-4b27-9547-0e6213209dec">

## Setup
### 1. Add Prefabs

- Add `AI Manager.prefab` to the root of the hierarchy:

<img width="263" alt="Screenshot 2023-06-30 at 3 54 16 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/8b659943-bdfb-4d8b-bf76-16011dfa0777">

- Add `Ai Review.prefab` to replace the review knot:

<img width="216" alt="Screenshot 2023-06-30 at 3 54 58 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/e302c61d-0808-4bed-b4b9-6e333eb1dc78">

### 2. Setup Speechblend

- Add `Speechblend.cs` to the AI avatar at the same level as the *Animator*:

<img width="1686" alt="Screenshot 2023-06-30 at 3 55 21 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/37d8215e-c96d-4fc5-9772-296d5b1362f4">


- Set the Head mesh to the `CC_Base_Body` child:

<img width="1686" alt="Screenshot 2023-06-30 at 3 56 44 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/6be88248-954f-48c7-8ea2-3c4921a3f3b5">


- Set the tracking mode on the Head mesh to `Jaw and Visemes`:

<img width="789" alt="Screenshot 2023-06-30 at 3 57 26 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/4c1d426b-14e9-45ad-abf6-83e3f6256c01">


- Set the shape template to `CC3` and click *Auto-Detect*:

<img width="794" alt="Screenshot 2023-06-30 at 3 57 44 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/5cdf737d-7392-4750-8a28-49cd5c7a4297">


- In the AI Manager object find the `AiAnimationController.cs` component and set *VR Guide Animator* to reference the SpeechBlend AI Avatar as shown:
  
<img width="827" alt="Screenshot 2023-06-30 at 3 58 19 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/9a29d529-18b4-4849-98f2-49debc3776d2">

### 3. Setup PrivateKeys
   
- Set up your Eleven Labs key as shown under:
  
  `Assets/GPT AI Integration/ElevenLabls/Runtime/Config/ELAuthArgs`
  ![Screenshot 2023-06-30 at 3 59 38 pm](https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/621ba873-7af9-4ea4-8dcc-d4fb254bba19)
  
  You can receive a key from ElevenLabs [here](http://elevenlabs.io).

- Set up your OpenAI key as shown under:
  
  `Assets/GPT AI Intergration/OpenAi/Runtime/Config/DefaultAuthArgsV1`
  ![Screenshot 2023-06-30 at 4 00 28 pm](https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/71be09dc-cea3-4049-9e66-edde56f16ff6)
  
  You can receive a key from OpenAI [here](http://openai.com).


## Edit Prompts & Script
### 1. Copy Streaming assets from the package into the Assets folder at the root level
<img width="316" alt="Screenshot 2023-06-30 at 4 03 20 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/d5801b54-c953-4b24-a34f-7bd2b77254f6"><br>
<img width="569" alt="Screenshot 2023-06-30 at 4 04 08 pm" src="https://github.com/bh679/Equal-Reality-AI-Review/assets/2542558/0ab906e5-ba10-40d6-86a4-35c9408baef4">

### 2. Find ReviewData.txt and edit it with the following format:
```{
    "intro": "You’ve just experienced an everyday workplace scenario from the perspective of Tamara. Take a moment to reflect and think about the following questions...",
    "reviewQuestions": [
        {
            "Question": "how did that feel?"
        },
        {
            "Question": "Do you think your colleagues were inclusive?"
        },
        {
            "Question": "What could they have done differently?"
        }
    ],
    "conclusion": "thanks again for taking the time to take this experience and practise inclusive behaviour. TO learn more, try and new experience or run this one again. To exit, take off the headset.",
    "comments": 0
}
```
 
 
## Internal Documentation
 - [AI Review Feature Specifications](https://docs.google.com/document/d/1ccq_VnhZ-AAbUW-jhwOcOB_fO1q0LPr8/edit)<br>
 - [EqualReality.AI Dependencies Map](https://docs.google.com/presentation/d/1Y2eK51DfQYKYzhT-jNj3jHkHUAtdsLeV46JADhA2G_E/edit#slide=id.g2528c1e646d_0_17)<br>



## Events
Should everything work correctly, the following stages will play out in descending order:
- **OnMicOn** - Triggered when Mic is looked for (every time it is the the user's turn to speak).
- **OnMicRecording** - Triggered when the Mic has started recording.
- **OnMicStopped** - Triggered when the Mic has stopped recording.
- **OnSendGPT** - Triggered when voice to text has finished and the recording is sent to GPT.
- **OnResponseFromGPT** - Triggered when GPT has returned a response.
- **OnVoicing** - Triggered when the response voicing process has begun in the cloud.
- **OnVoiceDownloading** - Triggered when the voice has started downloading.
- **OnVoicePlaying** - Triggered when the voice has been downloaded and has started playing.
- **OnVoiceFinished** - Triggered when the voice has finished playing.

## Feedback & Community
[Join the Discord server](https://discord.com/invite/VC8gZ2GNHs) if you would like to leave feedback or receive direct support!
