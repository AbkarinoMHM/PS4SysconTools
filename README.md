PS4 Syscon Tools By Abkarino & EgyCnq:
=======================================
PS4 Syscon Tools is a free solution (software & hardware) that allow you to manipulate original PlayStation 4 Syscon chip (Renesas RL78/G13).
It consists of 2 projects:
- PS4 Syscon Tools (PC client to allow you to control your PS4 Syscon Flasher hardware).
- PS4 Syscon Flasher (Hardware flasher based currently on Teensy++ 2.0 - Teensy 4.0 - Teensy 4.1).

![image_2025-05-15_23-58-09](https://github.com/user-attachments/assets/f2cd005a-468f-400f-9cb4-cec4c7f9befd)
![image_2025-05-16_00-13-37](https://github.com/user-attachments/assets/115e3981-ff19-4a86-8562-09145f9af104)

Features:
=======================================
- Syscon Flash Dumps: 
  -  Full (<50 sec). 
  -  Partial (dump any specific block or block range).
  -  NVS/SNVS only.  
Note : the read process here not use the infinty loop method that continue to dump the whole memory content and pass it to TOOL0 like some other tools we use more effecient way that can read immediately any block you want.  

- Syscon Flash Write: 
  - Full (<1.5 min). 
  - Partial (write any specific block or block range). 
  - NVS/SNVS only.    


- Syscon Flash Erase: 
  - Full (<4.3 sec). 
  - Partial (erase any specific block or block range).

- Enable Syscon Debug mode: 
	- Allow you to only connect 3 wires to read/write your syscon by enabling OCD flag.
	
- Get Syscon Firmware Dump Info:
	- Allow you to validate your syscon dump and get its firmware info (version, hash, debug mode status, magic validation).
	
Note : 	The process done on the fly so no need to dump the entire syscon and apply the patch then rewrite like some other tools.

Requirements:
=======================================
What you will need ?

- Teensy++ 2.0, Teensy 4.0 or Teensy 4.1. 
- (100 to 200)~ ohm resistor. 
- wires. 
- LQFP 64 - 100 Socket Adapter (Optional) 
- Soldering skills (Mandatory).

Connection Digrams:
=======================================
Please refer to diagrams directory to check the wiring digram for your hardware.

Usage - Tutorial:
=======================================
Please refer to PS4 Syscon Tool Tutorial on the following link:
https://www.youtube.com/watch?v=Abu-M_z_I-c&t=11s

Creadit:
========================================
- droogie (https://twitter.com/droogie1xp):
	For early syscon investigations & identifying original Syscon chip arch.
	
- fail0verflow (https://twitter.com/fail0verflow):
	For initial Writeup on the syscon attack, actually without their blog we will not be able to do any syscon hacks.
	
- wildcard (https://twitter.com/VVildCard777):
	For first public implementation for the Syscon glitcher that allow us to dump our Syscon flash and also for helping us a lot during this project development.

- DarkNESmonk:
	For original SysGlitch for Arduino.
	
For Updates and Release Info:
=========================================
Please follow us on twitter:	
- Abkarino (https://twitter.com/AbkarinoMHM)
- EgyCnq (https://twitter.com/EgyCnq)

As well as subscribe to our Youtube channle for more info and tutorials:

- https://www.youtube.com/@AbkarinoMHM

Donations:
=========================================
If you like our work and want to support us to get a new hardware to port our code to it or to be able to buy some damaged consoles to use it in reaserch, please donate to:

PayPal: mh.nasr@outlook.com (https://paypal.me/abkarinomhm)
