# LightAmp-Custom
Based on published [BMP2.0 libraries](https://github.com/BardMusicPlayer/BardMusicPlayer) and updated further by GiR-Zippo (Kalle). 
Personal fork including minor fixes and changes ahead of upstream.

For further information or a quick start, visit the [wiki page](https://github.com/GiR-Zippo/LightAmp/wiki).

## Hypnotoad

LightAmp can be used in conjunction with the [Hypnotoad](https://github.com/GiR-Zippo/Hypnotoad-Plugin) dalamud plugin for enhanced functionality.
* Output lyrics.
* Chat while playing.
* Direct instrument & ensemble ready / accept.
* Set graphics toggle.

Copy this url to your dalamud setting repositories and search for the hypnotoad plugin.

`https://raw.githubusercontent.com/GiR-Zippo/Hypnotoad-Plugin/master/PluginDir/pluginmaster.json`

## Libraries Info
```
coffer:       litedb catalog storage of midi files
grunt:        pushes buttons in game and pastes lyrics in game
jamboree:     cloud sync system (won't be used for 2.0)
maestro:      midi sequencer, consumes transmogrify processed midi files, outputs to grunt
pigeonhole:   json settings storage system. *all* settings are stored here. autosaves on variable write.
quotidian:    general utilities/enum/struct used by other modules
seer:         memory/network packet/.dat keybind reading system. event driven and has partial failover if a patch breaks a network read but not a memory read or vice versa
siren:        alphatab synthesizer sequencer, consumes transmogrify processed midi file for *highly* accurate replication of in game playback.
transmogrify: processes opened midi files into the format used by siren and maestro. an evolution of the bmp track naming conventions + lyric events.
ui:           mvvm frontend
```
