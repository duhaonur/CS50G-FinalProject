# Project Description
It's a simple FPS game. We have an AI system. The AI is capable of performing several actions: going to a valid position to shoot the player, entering a fire state where it starts shooting the player, running to cover if it's getting hit, searching for valid cover and taking cover, healing itself, and then going back to finding a valid position to shoot the player. Lastly, we have a dead state where the AI dies and waits for some time before returning to the enemy pool.

As for the player, the objective is to shoot as many enemies as possible to achieve the highest score. We have enemy spawners that continuously spawn enemies within a given time frame. The player starts with 500 health, and each enemy bullet costs them 10 health. If the player kills an enemy, they gain 25 health back. The game continues until the player's health reaches 0.

# Enemy AI States

### GoToPlayerState
To determine the path towards the player, the enemy generates a random target position around the player within a certain range. It checks if there is a clear line of sight to that position, ensuring there are no obstacles blocking its path. Once a valid position is found, the enemy calculates a path using NavMesh and starts moving towards it.

The enemy constantly checks if it's being hit by the player. If its health drops below a certain point (50% in this case) and luck is on its side (30% chance), it decides to change its state and run for cover instead of continuing to move towards the player.

It checks if it has successfully reached its destination. If it has reached the destination, it transitions to the state of shooting at the player.

With this state, the enemy has intelligent movement behavior. The enemy navigates towards the player, reacts to being hit, and strategically decides when to switch states. The randomized path generation adds unpredictability to its movement, making the gameplay more engaging and challenging.
### FireToTargetState
The enemy continuously looks at the player.

While shooting, the enemy occasionally changes its aim position to add unpredictability to its shots. It gradually adjusts its aim over time, creating a more dynamic shooting pattern.

It manages the process of shooting and reloading. If the enemy runs out of bullets, it reloads to get a fresh magazine. If it's in the middle of reloading or has finished reloading, it won't shoot.

It checks if the enemy is being hit by the player. If the enemy's health is below a certain point (50% in this case) and luck is on its side, it might decide to run for cover instead of continuing to shoot.

There's a timer that keeps track of time. If the timer reaches a certain point (1 second in this case), it checks if the enemy has a clear line of sight to the player. If it doesn't, it starts counting how many times in a row it has lost sight of the player. If it loses sight of the player three times in a row, it decides to stop shooting and goes back to a state where it goes to the player.

With this, we're trying to add realistic shooting behavior for the enemy. It aims at the player, manages shooting and reloading, reacts to being hit or losing sight of the player, and even changes its aim position for more challenging gameplay. This makes the enemy more interesting and engaging to encounter in the game.
### RunToCoverState
When the enemy enters the state of RunToCover, it searches for nearby objects that can serve as cover, specifically walls, by performing an OverlapSphere check around itself. It selects the closest wall as the cover destination.

To calculate the hiding position, the enemy considers the player's position and orientation. It determines the angle between the player's forward direction and the vector from the enemy to the player. Using trigonometry, it calculates a position offset based on this angle and applies it to the cover position.

Once the hiding position is determined, it sets the destination to the hiding position.

We check continuously if the agent has reached its destination. If the agent has reached the destination, it transitions to the state of taking cover.

This adds complexity and distinctiveness to the game by providing the enemy with intelligent behavior to find cover strategically. The enemy dynamically selects the closest wall as cover, calculates an optimal hiding position based on the player's position, and navigates towards it. The inclusion of these features enhances the enemy's ability to evade the player's attacks and adds depth to the gameplay experience.
### TakeCoverState
If the timer reaches a certain threshold (5 seconds in this case) and the enemy is not currently being hit, it means the enemy has successfully taken cover and can now reset its health. It triggers a method to restore the enemy's health and transitions to the state of going towards the player.

However, if the enemy is still being hit and a random value exceeds 0.5, the enemy decides to abandon its current cover and transitions back to the state of running to find new cover.

This adds complexity and distinctiveness to the game by allowing the enemy to actively assess its situation while in cover. Additionally, it introduces the possibility of the enemy reconsidering its cover choice and dynamically opting to find new cover if it continues to be under attack. These features contribute to the enemy's adaptive behavior and enhance the overall gameplay experience.
### PlayDeadState
When the enemy enters the play dead state, several actions are performed. Firstly, an event called "IncreaseScore" is triggered.

During the update phase, the timer starts. If the timer exceeds a specific threshold, the enemy's object is released back into the object pool. Releasing an object means it becomes available for reuse elsewhere in the game, improving performance and memory management.

This adds realism to the game by allowing defeated enemies to play a death animation and remain inactive for a certain period before being released back into the object pool. It also provides a mechanism for rewarding the player by increasing their score when an enemy is defeated.
# Player
Movement: The player can move in different directions using the arrow keys or WASD, and there is an option to run faster by holding down the left Shift key.

Camera Control: The player's view is controlled by the camera. The character's rotation smoothly follows the camera's view, ensuring that the character always faces the same direction as the camera.

Shooting: The player can shoot using the left mouse button. Shooting triggers animation. There is a limit to how quickly the player can shoot to maintain balance.

Reloading: The player can reload their weapon by pressing the "R" key. Reloading triggers an animation.

Health Management: The player has a health bar that decreases when they are hit. If the health reaches zero, the player dies, triggering a death animation and ending the game. The player can also increase their score and partially restore their health by killing enemies.

User Interface: The player's health is displayed using the Slider. The number of bullets remaining in the player's weapon is shown as text. The player's score is also shown as text.

These provide the player with the ability to move, shoot, reload, and manage health, creating an immersive gameplay experience.
