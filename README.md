# KoperPets
A pet system modification for the Secrets of Sosaria UO Shard.

This script adds a small chance to gain taming skill when your pet fights, decreasing in amount and frequency as you increase in taming skill. Also included is a battle cry for your pets, they will occasionally remark on the battle.

# Installation

WARNING: If you have modified your basecreature.cs script, do not replace it with this one. You will need to manually add the call for taming gain. Search my basecreature.cs script for KOPER.

(This has been save file safe to add and remove for me, as it serializes no data. BUT, back up your save just for that warm fuzzy)

1. Be running current version of SoS

2. Place KoperPets folder in Data/Scripts/Custom

3. Backup your original Data/Scripts/Mobiles/Base/BaseCreature.cs

4. Place BaseCreature.cs from this repositiory in Data/Scripts/Mobiles/Base, replacing the original
