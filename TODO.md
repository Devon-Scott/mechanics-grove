To-Do list for Mechanic's Grove

- Basics:
    - <s>Install probuilder and progrids</s>
    - Learn how to import assets
    - Learn how to use assets to build a level
    - Learn how to modify assets using code

- Learning:
    - Implementation of buttons in menus
    - Implementation of Controllers
        - Third person controller and Camera
        - Adding inputs to controller
    - Linking of models and animation
   

- Structural:
    - MainMenu button class
    - GameMenu button class
    - Enemy class 
    - Tower class


- Main Menu State
    - Have blurry image of gameplay in background
    - Make buttons for:
        - Play
        - Settings
        - Leaderboard
        - Exit
    - If Play: switch to Gameplay Scene
    - If Settings: push Settings Menu onto Stack
    - Have settings object that persists indefinitely (preserve settings until program ends)
        - Difficulty
        - Music
        - Audio

- Gameplay state
    - LevelMaker
        - Make first level in editor, then future levels make with code
        - No procedural level generation, fixed level design for each level
        - Maybe procedural for trees and rocks and cosmetics
    
    - Gameplay
        - Build mode and combat mode for player
            - Build mode activates small circle showing build location, ready for build menu
            - Build mode can build blockades on path and towers off of path
            - Combat mode ready for inputs related to attack, defense, and getting hit
        - Keeping settings object from main menu
            - Render music if music enabled
            - Render sound if sound enabled
            - Difficulty: number of monsters and/or health of monsters and/or player health affected

    - Spawn tower: springy deform animation before tower becomes active with matching sound effect
    
    - Enemy pathfinding
        - Series of empty game objects for enemies to walk towards? Split path and make them alternate between targets?
        - Implementation of graph theory: A series of nodes placed at splits/joins in the path
        - Levelmaker could pass these nodes in to the enemy class and make the enemy choose a path
        - LevelGraph object representing the nodes of the path: used for both level making and enemy pathfinding 

    - Tower Menu
        - When in a suitable build location, allow player to open tower build menu
        - When in a suitable build location, allow player to open tower build menu
        - Push tower menu to player state stack, action continues in game state

    - Pause menu
        - Push pause menu to stack
        - Transparent view, like main menu, with Resume, Save, Settings, Exit buttons


- Level end state
    - Buttons for Play Again and Exit
