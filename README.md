# Fundamentals of modeling and character animation
# Delivery3

## Team Description
- Group ID: 22
- Name: Josep Adrià Vilaplana Miret
- Mail: josepadriavilaplanamiret@enti.cat

![alt text](https://s3-eu-west-1.amazonaws.com/classlife/profile/student/2018/10/thumbnail/201810031413IMG_20181003_141153.jpg)


## Introduction
You are asked to finish a prototype of a game where you shoot a ball against a goal-keeper, which
is an octopus.

## Exercise 1
Implement the basic mechanics of shooting the ball and the octopus picking it.

The formulas used on the three axis in this exercise for the movement of the ball are the following:

![equation](http://www.sciweavers.org/upload/Tex2Img_1611351341/render.png)

![equation](http://www.sciweavers.org/upload/Tex2Img_1611351103/render.png)

![equation](http://www.sciweavers.org/upload/Tex2Img_1611351450/render.png)

Where: 

- ![equation](http://www.sciweavers.org/upload/Tex2Img_1611351634/render.png) is the normalized vector that points to where the player wants and that has the origin in the center of the ball. All this multiplied by the value of the power bar.
- ![equation](http://www.sciweavers.org/upload/Tex2Img_1611351962/render.png) is ![equation](http://www.sciweavers.org/upload/Tex2Img_1611352049/render.png)
- ![equation](http://www.sciweavers.org/upload/Tex2Img_1611352175/render.png) is the elapsed time since the kick of the ball

## Exercise 2
We will implement the Magnus effect.

## Exercise 3
In this exercise we will upgrade the animations of our scorpion, making him more reactive to its
enviroment by placing it's legs on the scene floor by using a raycast. 

## Exercise 4
In this exercise you have to take the animations provided and use motion builder transfer them to
the Robot character. The task is to have at least 2 robot characters that react to the ball entering
or not the goal (see the optoins of happy or sad animations provided).

# SCRIPTS
In the folder Scripts of the project there are 9 C# scripts:
- **AnimatorControl**
  - You can find it in: SceneRoot/AnimatorController
  - Description: This script takes the animators of the four models from the audience in the bank and the MovingBall object of the game. Its main function is to reproduce the animations of victory and disbelief on the robots depending on whether the ball has entered the net or not.
- **IK_Scorpion**
  - You can find it in: *SceneRoot/Scorpion*
  - Description: This script controls the movements of the scorpion. It connects to the OctopusController library to initialize the scorpion's leg and tail joints as well as the scorpion's movement around the stage. It is also in charge of calculating the position of the targets of the legs through ray casts to overcome obstacles and contains a function to reset the necessary values each time the ball is shot again
- **IK_tentacles**
  - You can find it in: *SceneRoot/Blue_team/OctopusBlueTeam*
  - Description: This script controls the tentacles of the octopus. It is connected to the OctopusController library and manages the movement of its limbs that follow various targets as well as try to stop the ball on some occasions.
- **LightBlink**
  - You can find it in: *SceneRoot/DirectionalLight*
  - Description: This is a simple script that only takes care of flashing a light on the screen when the scorpion is about to kick the ball.
- **MovingBall**
  - You can find it in: *SceneRoot/Ball*
  - Description: The Moving Ball script takes care of displaying the power bar on the right side of the screen every time the scorpion is in position to kick the ball. It is also in charge of restarting the level once 5 seconds have passed since the player has shot.
Within this script we also find the necessary calculations for the ball movement that are executed when the tip of the scorpion's tail contacts with the ball. The movement is executed according to the values given both by the power bar and the direction selected by the player (formula explained in the upper section).
- **MovingTarget**
  - You can find it in: 
    - *SceneRoot/Blue_team/BlueTarget*
    - *SceneRoot/Blue_team/GoalWithColliders/region1/RandomTargetRegion1*
    - *SceneRoot/Blue_team/GoalWithColliders/region2/RandomTargetRegion2*
    - *SceneRoot/Blue_team/GoalWithColliders/region3/RandomTargetRegion3*
    - *SceneRoot/Blue_team/GoalWithColliders/region4/RandomTargetRegion4*
  - Description: This script is located in two types of objects with different functionalities, so it has two modes. The first is USERTARGET, which is the target where the player wants to aim. It is in charge of showing on the screen the direction in which it is going to shoot and modify with the predetermined movement keys. That position will be used to send to the octopus controller in which region of the four is going to shoot in order to move the appropriate tentacle.
The second mode is RANDOM, which is applied to four objects that will move in circles through their set region.
- **Net**
  - You can find it in: SceneRoot/ScoreTrigger
  - Description: This script is inside a gameObject with a Collider located behind the octopus that spans the entire goal. It is responsible for notifying the AnimatorControl script that the ball has entered and the public should celebrate.
- **NotifyRegion**
  - You can find it in: 
    - *SceneRoot/Blue_team/OctopusBlueTeam/tentacle_right/.../Bone.001_end/BallRegion*
    - *SceneRoot/Blue_team/OctopusBlueTeam/tentacle_right2/.../Bone.001_end/BallRegion*
    - *SceneRoot/Blue_team/OctopusBlueTeam/tentacle_left/.../Bone.001_end/BallRegion*
    - *SceneRoot/Blue_team/OctopusBlueTeam/tentacle_left2/.../Bone.001_end/BallRegion*
  - Description: This script is placed at the tip of the tentacles and is used to save in which region it is located.
- **ScoreControl**
  - You can find it in: 
    - *SceneRoot/Blue_team/OctopusBlueTeam/tentacle_right/.../Bone.001_end/BallRegion*
    - *SceneRoot/Blue_team/OctopusBlueTeam/tentacle_right2/.../Bone.001_end/BallRegion*
    - *SceneRoot/Blue_team/OctopusBlueTeam/tentacle_left/.../Bone.001_end/BallRegion*
    - *SceneRoot/Blue_team/OctopusBlueTeam/tentacle_left2/.../Bone.001_end/BallRegion*
  - Description: This script is placed at the tip of the tentacles and is used to notify that the ball has been stopped by one of the octopus's tentacles. Through this, the movement of the ball stops calculating and the AnimatorControl plays the animation of Disbelief.
