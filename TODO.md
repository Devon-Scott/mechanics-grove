To-Do list for Mechanic's Grove

- Basics:
    - <s>Install probuilder and progrids</s>
    
- Learning:
    - <s>Learn how to import assets</s>
    - <s>Learn how to modify animation controllers
        - Note: use animation controller visual scripting and<br>
            code additional state transitions into StarterAssetInputs
        - Tiny Hero Duo already comes with lots of useful animations </s>
    - <s>Learn how to attach hitboxes to objects meant for attacking
        - Learn how objects register that they've been attacked</s>
    - Learn how to use assets to build a level
    - Learn how to modify assets using code
    - Implementation of buttons in menus
    - Implementation of Controllers
        - Third person controller and Camera
        - <s>Adding inputs to controller</s>
    - <s>Linking of models and animation</s>
    - Implementing combat system:
        - Cooldowns on enemy attacks
        - <s>Hitbox and Hurtbox registration</s> and knockback on affected objects
        - <s>Hitbox has static dictionary so that all hitboxes can quickly reference colliders (hurtboxes) they hit</s>
        - Attacks attack hitboxes, different attacks produce different hitboxes
        - <s>Learn how to use animation events to pass arguments to EnableHitbox()</s>
    - Event handling for triggers such as onHealthZero for enemies

- Structural:
    - MainMenu button class
    - GameMenu button class
    - Enemy class 
        - Experiment with statemachine for enemy controller
            - <s>Move State
            - Attack State</s>
            - <s>Knockback State</s>
            - Death State
    - Player Class
        - Experiment with identifying owner of hitbox
        - Use starterAssets as jumping off point for a state machine
    - Tower Class
    - Level Manager
        - Contains list of LevelGraph objects
        - LevelGraph contains:
            - Player Spawn Point
            - Path Nodes
            - Enemy Spawn Point
            - End Point of Path
        - LevelGraph can be added as component to newly spawned enemy so they know where to go
        - Also contains information about number and types and timings of enemies to spawn
    - Map Maker
        - Instantiate path tiles from one node to next
        - Have set to determine visited tiles to avoid repeated instantiation
        - Method to determine size of map
        - Fill empty tiles with ground, fill those with random decorations
    - Enemy Spawner

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
            - (Ensure object is only modifiable outside of gameplay state)
        - Music
        - Audio

- Gameplay state
    - MapMaker
        - Make first level in editor, then future levels make with code
        - No procedural level generation, fixed level design for each level
        - Maybe procedural for trees and rocks and cosmetics
        - Use ProBuilder to make prefabs of path tiles 
        - Overlay path tiles on top of grid of ground tiles
    
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
        - <s>Implementation of graph theory: A series of nodes placed at splits/joins in the path</s>
        - Levelmaker could pass these nodes in to the enemy class and make the enemy choose a path
        - LevelGraph object representing the nodes of the path: used for both level making and enemy pathfinding 
        - Enemies in mind so far:
            - Spined turtle: high attack, knockback resistant, slow
            - Slime: medium in all respects
            - Floating enemy: low attack, easy to knockback, high speed, trouble cornering
            - Devil: On death, waits several seconds and spawns either itself or 2-4 other enemies

    - Tower Menu
        - When in a suitable build location, allow player to open tower build menu
        - Also allow player to build blockades on the path
        - Push tower menu to player state stack, action continues in game state
        - Towers in mind so far:
            - Cannonball tower: launches bombs that deal splash damage, knockback, and have particle effect
            - Mage tower: zaps single targets for high damage with lightning bolt effect
            - Mushroom Cloud tower: slows enemies, or deals static area damage
            - Barracks: sends out mini soldiers to stand guard on the path

    - Pause menu
        - Push pause menu to gameplay state machine stack
        - Transparent view, like main menu, with Resume, Save, Settings, Exit buttons


- Level end state
    - Buttons for Play Again and Exit
