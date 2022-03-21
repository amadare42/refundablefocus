# Refundable Focus

This mod allow you to refund Focus spent on movement. Other players aren't required to have it.

### How to use

Every Focus point spent on movement will marked as semi-transparent to show that it is refundable in focus panel. When you click on focus hex, it will be refunded and movement point received will be removed.

**Demo**:

![demo](./readme/demo.gif)

## Installation

### Through Thunderstore (soon)

1. Install [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager)
2. Install `Refundable Focus` mod
3. Start game by using Start Modded button

### Manual
1. Install latest [Bepinex](https://github.com/BepInEx/BepInEx/releases) 5.* version. You can refer to installation instructions [here](https://docs.bepinex.dev/articles/user_guide/installation/index.html)
2. Install [HookGenPatcher](https://github.com/harbingerofme/Bepinex.Monomod.HookGenPatcher) 
   - OR put `MMHOOK_Assembly-CSharp.dll` from [this repo](https://github.com/ftk-modding/stripped-binaries) to `BepInEx\plugins` folder from 
3. Start game normally

## Build
1. You'll need to provide stripped and publicized binaries. Those are included as git submodule. In order to get those you can:
    - When cloning this repository use `git clone --recurse-submodules`
    - OR If repository is already checked out, use `git submodule update --init --recursive`
    - OR Download or build them yourself using instructions from [this repo](https://github.com/ftk-modding/stripped-binaries)
2. (optional) Set `BuiltPluginDestPath` property in RefundableFocus.csproj file so after build binaries will be copied to specified location.
3. Run `dotnet restore`
4. You should be able to build solution now

## License
MIT