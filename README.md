# Time and Weather Controller

## By Daft Apple Games

# What is it

This is a Time of Day and Weather Controller for Unity. It sits on top of other assets, called "Providers", that actually provide Time of Day and Dynamic Sky functionality. The controller aims to combine the functions of a Time Provider and a Weather Provider, and add some unique touches of it's own, to give developers a single, unified place to control time and weather in their game.

The implementation currently comes with components to support the "Expanse" HDRP asset, and has been developed and tested with Expanse 1.7.3.

# Getting started

The repository has everything you need to get started. Just put the code into your Assets folder, check the pre-requisites below, and open the demo scene.

## Pre-requisites

To get the demo scenes to work, you'll need to add:

### Third party assets

- Expanse (obviously)
- Odin Inspector (optional) - if you have Odin Inspector installed, you'll get a slightly improved editor experience. This is optional, and if not installed then the editor inspectors will fall back to a variant of Naughty Attributes that's included in the code.

### Unity packages

- Text Mesh Pro
- High Definition RP > Samples > Particle System Shader Samples

The rain and thunder audio is included under the "Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)" license, and is attributed to Alexander. You can find more of his work here: https://orangefreesounds.com/author/alexander/.

# How it works

From a technical perspective, the system works off of a small number of core classes:

- **`TimeAndWeatherManager:MonoBehaviour`** - this is the main manager class, implemented as a Singleton (whether you like it or not!), providing the main functionality of the controller. That is, public methods to control time and start new weather.
- **`WeatherProviderBase:MonoBehaviour`** - this is a MonoBehavior abstract class that provides common weather methods, as well as setting the abstract method patterns for specific providers.
- **`TimeProviderBase:MonoBehaviour`** - same as above, but for time methods.
- **`WeatherPresetSettingsBase:ScriptableObject`** - this is a ScriptableObject class that provides a common data structure and methods for storing "weather presets". These are the core inputs into the WeatherProvider, and contain definitions of a weather behaviour, as well as a list of other weather presets that can be transitioned to.
- **`TimePresetSettingsBase:ScriptableObject`** - same as above, but for time methods.

We then have the Expanse implementation classes:

- **`ExpanseWeatherProvider:WeatherProviderBase`** - this class provides abstract method definitions, and any custom methods specific to Expanse, to implement and apply weather profiles in Expanse.
- **`ExpanseTimeProvider:TimeProviderBase`** - same as above, but for time methods.
- **`ExpanseWeatherPresetSettings:WeatherPresetSettingsBase`** - this ScriptableObject class extends the base class to provide Expanse specific settings, required to define a specific weather preset.
- **`ExpanseTimePresetSettings:TimePresetSettingsBase`** - you guessed it, same as above but for time.

The idea here is that you could create a new set of "Provider" classes, implementing different Time of Day and Sky / Weather providers, and it would all "just work" with the TimeAndWeatherManager.

The Sync Manager concept works in much the same way. These classes provide functionality to react to a change in time and weather, and examples are included that synchronise The Vegetation Studio and MicroSplat material shaders.

- **`TimeAndWeatherSyncBase:MonoBehaviour`** - this is the base component class that implements shared, lerp focussed, functionality for time and weathering sync.
- **`WeatherSyncBase:TimeAndWeatherSyncBase`** - this class provides functionality for sync'ing wetness, overlay (typically snow or sand / dust), and wind. It defines an abstract template that can be implemented by specific 3rd party tools.
- **`MicrosplatWeatherSync:WeatherSyncBase`** - implements weather sync functionality for Microsplat. Note that the MicroSplat "Dynamic Snow", "Paintable Puddles, Streams, Lava, Wetness", and "Wind and Glitter" modules are required for overlay / snow, wetness and wind respectively. No errors will be logged in the console if you don't have the modules - the effect will just not be visible on the terrain.
- **`TVEWeatherSync:WeatherSyncBase`** - implements weather sync functionality for The Vegetation Engine.

# Setting up presets

Presets are just instances of the provider Scriptable Objects, and can be created via the Assets > Create > Daft Apple Games > Weather menu item. If you look in `Assets\DaftAppleGames\TimeAndWeatherController\Presets`, you'll find a number of these already created. The most interesting one is likely "HeavyRainPreset", which shows the extent of what's configurable for a weather preset. The "MidnightPreset" time preset shows how you can link time and weather, effectively enforcing a clear night sky as the time transitions to midnight.

Each preset has a Wetness, Overlay, and Wind setting that will influence any attached Sync components.

Most presets come with a "Transition Duration", which is a value in seconds used to Lerp from current settings to new preset settings. These can be adjusted individually, to allow you to get the effect you want.

You'll also see that each weather preset has a list of other weather presets that it can transition to. This is to prevent "weird" transitions, like clear weather going straight to heavy snow. Though you can permit that transition if you so wish.

Note that the "Audio Effect" and "Particle Effect" prefabs are all instantiated at Start(), and placed as children of the main camera. The particle effects are in world space, and so follow the camera around. The code comes with an example `AudioMixer`, with a separate `AudioMixerGroup` for the ambient sounds associated to the weather sound effects.

# Setting up sync

The code comes with sync components for TVE and MicroSplat. These can be added as components to an empty Game Object in the scene, and connected to the main Time and Weather Manager component via the `OnWeatherPresetAppliedEvent` event. Note that you must explicitly connect the component via the event.

## The Vegetation Engine

TVE works right out of the box, though you will of course needs a TVE Manager component in your scene. I'd advise letting TVE create everything you need via:

Window > BOXOPHOBIC > The Vegetation Engine > Asset Converter > Create Scene Manager

You can tweak the settings in Global Motion and Global Control as you see fit.

## MicroSplat

MicroSplat setup requires a few additional steps:

- Set these shader parameters to "Global" via the "G" button next to them in the "Settings" tab of your MicroSplat material:
  - Snow > Amount
  - Streams & Lava > Wetness Range
  - Wind > Strength
- Configure the Height Range under Snow. Don't forget that the first number is the terrain height at which snow will start, while the second number is when the snow will reach it's max strength.
- If you're using wetness sync, then you'll want to create a wetness mask to prevent covered terrain from getting wet:
  - Window > Microsplat > Terrain FX Painter > Raycast Wetness > Calculate

# Current Features

The latest code has these features:

- Smooth time transition to any time using `TimeAndWeatherManager.GotoTime()`
- Smooth time transition using `TimeAndWeatherManager.ApplyTimePreset()`
- Smooth weather transition using `TimeAndWeatherManager.ApplyWeatherPreset()`
- Example audio and particle effects for a rain preset.
- Example audio and particle effects for a snow preset.
- Example time and weather presets.
- Time simulation, with parameters to control rate of progression and evaluation frequency.
- Random weather transitions with parameters to control allowed target presets, interpolation duration between presets, and evaluation frequency for changes in weather.
- Sync components for The Vegetation Engine and MicroSplat, along with a demo scene showing the components in action.

# Other components

There are a number of components that provide debugging functionality, and also demonstrate how the Time and Weather Manager can be used. 

Some notable components include:

- **`TimeAndWeatherEvents`** - this component that can be used across multiple scenes, and that provides some useful Unity Events to hook into other parts of your scene. For example, these can be used to send NPCs to bed as night approaches, or for farm animals to come to life as dawn breaks.

# Coming up

These features are currently in the pipeline:

- Weather volumes - weather presets triggered when the player or camera enters a trigger collider area.
- Indoor weather volumes - particle and ambient audio effects when the player or camera enters or exits a building.

# Some caveats

I am a hobby developer, and have only just started to scratch the surface of Unity and coding. As such, the code presented here may not be the cleanest or the most practical, and I suspect that in many ways I have overcomplicated things! Feedback, and of course PRs, are very welcome!

