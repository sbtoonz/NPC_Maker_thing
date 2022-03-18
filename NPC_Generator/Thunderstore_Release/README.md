<center><img src="https://cdn.discordapp.com/attachments/950033209427636265/954487038340649000/unknown.png"></center>

# NPC Generator

NPC Generator is a tool that will allow you to generate humanoid NPC's for your Valheim world. 
These NPC's have quite a few configuration options for you to fill out. If you have used RRR some of these will look familiar to you.

* ### This mod is configured via a YML file and is server sync
* ### This mod MUST be installed both on the client and the server. 
* ### This mod is equipped with a file watcher. Any changes to the Configuration YML should be reflected live in game.


I have included an example configuration YML for this mod for you to build your NPC's off of. 

If you have any questions please find me in the Odin+ discord

# **PLEASE NOTE IF YOU HAVE ANY OF THE SPECIAL VILLAGER FLAGS OTHER THAN BASIC VILLAGER YOU CANNOT HAVE THE NPC SET AS TAMEABLE**

There are some special flags for "Villagers" 
<ul>
<li><h4>Is Basic Villager => This will flag your NPC as a basic player factioned villager who will say hello when you interact with them. If you turn PVP on and strike they will strike back</h4></li>
<li><h4> Villager Builder => This villager requires some building materials to hopefully one day finish their project. If you give them the materials they ask for you will get a coin reward.</h4></li>
<img src="https://cdn.discordapp.com/attachments/950033209427636265/954494587328667728/ezgif.com-gif-maker_6.gif">
<li><h4> Villager Farmer => This Villager requires some seeds for their fields. If you give them the requested seeds you will be rewarded with coins.</h4></li>
<img src="https://cdn.discordapp.com/attachments/950033209427636265/954493795699924992/ezgif.com-gif-maker_2.gif">
<li><h4>  Villager RaidMaster => This villager warns of an impending raid from a neighboring tribe. Interacting with them will trigger the raid. If you ward off the raid and keep this NPC alive they will reward you with coins</h4></li>
<img src="https://cdn.discordapp.com/attachments/950033209427636265/954491366732357724/ezgif.com-gif-maker.gif">
<img src="https://cdn.discordapp.com/attachments/950033209427636265/954492000114192485/ezgif.com-gif-maker_1.gif">
<li><h4>  Villager SkillMaster => This Villager will take an item (configurable) in exchange for a skill point boost (Configurable) you can only interact with this NPC 1 time. </h4></li>
<img src="https://cdn.discordapp.com/attachments/950033209427636265/954494121039499274/ezgif.com-gif-maker_3.gif">
</ul>

## There are some other things to note when deploying your NPC's 
_________________________

### Damage Resists:
* Normal
  * This Means that the NPC takes Normal damage
* Resistant
  * This Means that the NPC takes less than normal damage
* Weak
  * This means that the NPC takes slightly more damage from this type
* Immune
  * This means the NPC is immune to this damage type
* Ignore
  * This means the NPC doesnt ever acknowledge this type of damage
* Very Resistant
  * This means the NPC takes a bit less damage than Resistant from this type
* Very Weak
  * This means the NPC takes slightly greater damage from this type than Weak
______________

### NPC Factions

* Player
* AnimalsVeg
* ForestMonsters
* Undead
* Demon
* MountainMonsters
* SeaMonsters
* PlainsMonsters




# Release Notes:

## V0.0.1
 * Initial Release
