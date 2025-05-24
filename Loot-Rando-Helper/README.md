# Loot Rando Helper

GUI Helper for [Loot Randomizer](https://bl-sdk.github.io/willow2-mod-db/mods/lootrandomizer/) mod for Borderlands 2

## Using pre-built version

1. Open [CptnFooFoo's twitch chat](https://twitch.tv/CptnFooFoo) and send `!rando`
2. Download and unpack the zip file

### Windows

3. Run `Loot-Rando-Helper.exe`

### Linux

3. You can run `Loot-Rando-Helper.exe` with the help of Proton / Wine  
   which would be required for Loot Rando to work anyway, because SDK mods aren't supported on native builds of BL2

However, you'd need to use the same `wine prefix` as the one used for BL2  
Otherwise Loot Rando Helper won't find your seed files  
You may want to use something like [SteamTinkerLaunch](https://github.com/sonic2kk/steamtinkerlaunch) for easier prefix management

Alternatively you can:

## Build from source

This project uses .NET 8.0, install it prior to proceeding

### Platform Requirements

#### Windows

None known

#### Linux

At least these, install with your package manager:

- libx11 (libx11-dev on Ubuntu)
- libgdiplus (libgdiplus on Ubuntu)

> [!NOTE]
> These are required because building for Linux uses a fork of [DanielVanNoord's project](https://github.com/DanielVanNoord/System.Windows.Forms/tree/master)  
> [Fork used](https://bitbucket.org/mfeemster/system.windows.forms/) is by mfeemster, who targets .Net 8.0 in their project instead of 6.0

#### Mac

Not planned

### Build Instructions

1. Clone project:

```bash
git clone https://github.com/NickPoirier/Loot-Rando-Helper.git
cd Loot-Rando-Helper
git submodule update --init --recursive
```

2. Build:

```sh
dotnet publish -c Release -f net8.0 -o build
```

> [!TIP]
> Replace net8.0 with net8.0-windows above for local build testing on windows

3. Run:  
Execute `Loot-Rando-Helper` inside `build` folder, i.e.

```sh
./build/Loot-Rando-Helper
```