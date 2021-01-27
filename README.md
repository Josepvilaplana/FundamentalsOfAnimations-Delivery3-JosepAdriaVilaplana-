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