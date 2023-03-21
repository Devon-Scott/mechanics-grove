To-Do list for Mechanic's Grove

- Basics:
    - <s>Install probuilder and progrids</s>
    
- Learning:
    - <s>Learn how to import assets</s>
    - <s>Learn how to modify animation controllers
        - Note: use animation controller visual scripting and<br>
            code additional state transitions into StarterAssetInputs
        - Tiny Hero Duo already comes with lots of useful animations </s>
    - Learn how to attach hitboxes to objects meant for attacking
        - Learn how objects register that they've been attacked
    - Learn how to use assets to build a level
    - Learn how to modify assets using code
    - Implementation of buttons in menus
    - Implementation of Controllers
        - Third person controller and Camera
        - <s>Adding inputs to controller</s>
    - Linking of models and animation
    - Implementing combat system
        - Cooldowns on enemy attacks
        - Hitbox registration and knockback on affected objects
   

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
        - (Ensure object is only modifiable outside of gameplay state)
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
            - Carry hammer or other tools to indicate ready to build
            - Combat mode ready for inputs related to attack, defense, and getting hit
            - Carry sword and shield ready for combat
            - Add animations and hitboxes for sword attacks and defending states
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
        - Also allow player to build blockades on the path
        - Push tower menu to player state stack, action continues in game state
        - Towers in mind so far:
            - Cannonball tower: launches bombs that deal splash damage and have particle effect
            - Mage tower: zaps single targets for high damage
            - Barracks: sends out mini soldiers to stand guard on the path

    - Pause menu
        - Push pause menu to stack
        - Transparent view, like main menu, with Resume, Save, Settings, Exit buttons


- Level end state
    - Buttons for Play Again and Exit
