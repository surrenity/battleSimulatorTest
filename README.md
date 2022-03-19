# battleSimulatorTest
A battle will start with two armies  
A battle will end when all units in one army is defeated  
The battle will take place on a 2 dimensional grid.   
The grid can be scaled up and down, but needs to support enough room for all the units to be placed evenly on both sides.  
It will place units on 2 teams.
It will place them at the start and end row.


The battle will go through "Rounds"  
A round will be defined as: all units get to perform an action and/or move.  
When a round starts every unit will roll for initiative.  
If there are ties, there will be a tie resolution algorithm, so we will have no ties.  

Each unit will have these attributes  
-Initiative  
-Movement Points  
-Health Points  
-Mana points  
-Min Damage  
-Max Damage  
-Attack Range  
[Actions]  
 -Attack  
 -Move and Attack  
 -Move  
 -Skip  

Units will never retreat.  
Units will always try to move forward to attack an enemy.  
When attacking, A unit will analyze all possible targets in it's attack range and attack one at random  

This project is licensed under the terms of the MIT license.
