# discord-points-bot
A derpy bot for Discord. Give and take arbitrary points from channel members :)

## Bot
The Bot project runs in a continous Azure Web Job. Some configuration needed.

## CLI
Small project for directly dropping messages on the command queue.

## Function
An Azure function with a service bus trigger. Reads commands off of queue and event sources to Redis.
Also has HTTP triggers to handle Query side.

## Infrastructure
Holds dependencies related to infrastructure. Right now it has implementations for communication with Azure Service Bus and some contracts used in the Command and Query side of the application.